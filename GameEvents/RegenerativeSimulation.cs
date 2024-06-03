using System;
using System.Collections.Generic;
using CodeName.EventEngine.Tasks;
using CodeName.EventEngine.Utility;
using CodeName.Serialization;
using UnityEngine.Assertions;

namespace CodeName.EventEngine.GameEvents
{
    /// <remarks>
    /// This implementation requires that the same events are raised during the replay compared to the original.
    /// This will need to be rewritten later on again.
    /// </remarks>
    public class RegenerativeSimulation<TState> : ISimulation<TState>
    {
        private int currentNodeI = 1;

        private readonly Config config;

        [Obsolete]
        private readonly List<GameEventNode<TState>> originalEventList;

        public TState State { get; private set; }
        public EventTracker<TState> Events { get; }
        public IReadOnlyList<IEventHandler<TState>> EventHandlers => config.EventHandlers;

        public RegenerativeSimulation(TState state, GameEventNode<TState> events, Config config)
        {
            this.config = config;

            State = config.Serializer.Clone(state);
            Events = new EventTracker<TState>(config.Serializer, new GameEventNode<TState>(new SimulationRootEvent<TState>(), Array.Empty<int>(), config.Serializer, events.Id));

            originalEventList = new List<GameEventNode<TState>>();
            FlattenEventTree(events, originalEventList);
        }

        public async StateTask RaiseEvent(GameEvent<TState> gameEvent)
        {
            // * Needs more analysis
            // This will break when more/less/different events are created on the server compared to the client
            await ReplayNextNode();
        }

        private async StateTask ReplayNextNode()
        {
            Assert.IsTrue(currentNodeI < originalEventList.Count);
            var originalNode = originalEventList[currentNodeI];
            currentNodeI++;

            while (Events.PathToCurrentNode.Count >= originalNode.Path.Count)
            {
                Events.Pop();
            }

            var node = Events.Push(State, originalNode.OriginalEvent, originalNode.Id);
            {
                await SimulationUtility.OnAnimationEventRaised(this, config.AnimationHandlers);
                await SimulationUtility.OnEventRaised(this, config.EventHandlers);
                node.Lock();
                await SimulationUtility.OnAnimationEventConfirmed(this, config.AnimationHandlers);
                await SimulationUtility.OnEventConfirmed(this, config.EventHandlers);
                await node.Event.Apply(this);
                await SimulationUtility.OnAnimationEventApplied(this, config.AnimationHandlers);
                await SimulationUtility.OnEventApplied(this, config.EventHandlers);

                var shouldValidate = config.IsDebugMode && node.ExpectedState != null;
                if (shouldValidate && !DiffUtility.ValidateState(config.Serializer, State, node))
                {
                    // State has de-synced, re-sync by replacing current state with expected.
                    State = config.Serializer.Clone(node.ExpectedState);
                }
            }
            Events.Pop();
        }

        public async StateTask ReplayToEnd()
        {
            await ReplayNextNode();

            Assert.AreEqual(currentNodeI, originalEventList.Count);
        }

        public class Config
        {
            public bool IsDebugMode { get; set; } = false;
            public ISerializer Serializer { get; set; }
            public IReadOnlyList<IAnimationHandler<TState>> AnimationHandlers { get; set; } = Array.Empty<IAnimationHandler<TState>>();
            public IReadOnlyList<IEventHandler<TState>> EventHandlers { get; set; } = Array.Empty<IEventHandler<TState>>();
        }

        private void FlattenEventTree(GameEventNode<TState> root, List<GameEventNode<TState>> results)
        {
            results.Add(root);
            foreach (var childEventNode in root.Children)
            {
                FlattenEventTree(childEventNode, results);
            }
        }
    }
}
