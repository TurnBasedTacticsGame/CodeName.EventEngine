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
    public class RegenerativeSimulation<TGameState> : ISimulation<TGameState>
    {
        private int currentNodeI = 1;

        private readonly Config config;

        [Obsolete]
        private readonly List<GameEventNode<TGameState>> originalEventList;

        public TGameState State { get; private set; }
        public EventTracker<TGameState> Events { get; }
        public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers => config.EventHandlers;

        public RegenerativeSimulation(TGameState state, GameEventNode<TGameState> events, Config config)
        {
            this.config = config;

            State = config.Serializer.Clone(state);
            Events = new EventTracker<TGameState>(config.Serializer, new GameEventNode<TGameState>(new SimulationRootEvent<TGameState>(), Array.Empty<int>(), config.Serializer, events.Id));

            originalEventList = new List<GameEventNode<TGameState>>();
            FlattenEventTree(events, originalEventList);
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
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
                if (shouldValidate && !DiffUtility.ValidateGameState(config.Serializer, State, node))
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
            public IReadOnlyList<IGameAnimationHandler<TGameState>> AnimationHandlers { get; set; } = Array.Empty<IGameAnimationHandler<TGameState>>();
            public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers { get; set; } = Array.Empty<IGameEventHandler<TGameState>>();
        }

        private void FlattenEventTree(GameEventNode<TGameState> root, List<GameEventNode<TGameState>> results)
        {
            results.Add(root);
            foreach (var childEventNode in root.Children)
            {
                FlattenEventTree(childEventNode, results);
            }
        }
    }
}
