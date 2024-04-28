using System;
using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;
using CodeName.Serialization;
using UnityEngine.Assertions;

namespace CodeName.EventSystem.GameEvents
{
    /// <remarks>
    /// This implementation requires that the same events are raised during the replay compared to the original.
    /// This will need to be rewritten later on again.
    /// </remarks>
    public class RegenerativeGameStateTracker<TGameState> : IGameStateTracker<TGameState>
    {
        private int currentNodeI = 1;

        private readonly Config config;
        private readonly GameEventTracker<TGameState> originalTracker;

        public TGameState State { get; private set; }
        public GameEventTracker<TGameState> Events { get; }
        public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers => config.EventHandlers;

        public RegenerativeGameStateTracker(TGameState state, GameEventTracker<TGameState> tracker, Config config)
        {
            this.config = config;
            originalTracker = tracker;

            State = state;
            Events = new GameEventTracker<TGameState>(config.Serializer, new GameEventNode<TGameState>(new TrackerRootEvent<TGameState>(), Array.Empty<int>(), config.Serializer, tracker.Tree.Id));
        }

        public async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            // * Needs more analysis
            // This will break when more/less/different events are created on the server compared to the client
            await ReplayNextNode();
        }

        private async StateTask ReplayNextNode()
        {
            Assert.IsTrue(currentNodeI < originalTracker.List.Count);
            var originalNode = originalTracker.List[currentNodeI];

            while (Events.PathToCurrentNode.Count >= originalNode.Path.Count)
            {
                Events.Pop();
            }

            var node = Events.Push(State, originalNode.Event, originalNode.Id);
            currentNodeI++;
            {
                await GameStateTrackerUtility.OnAnimationEventRaised(this, config.AnimationHandlers);
                await GameStateTrackerUtility.OnEventRaised(this, config.EventHandlers);
                node.Lock();
                await GameStateTrackerUtility.OnAnimationEventConfirmed(this, config.AnimationHandlers);
                await GameStateTrackerUtility.OnEventConfirmed(this, config.EventHandlers);
                await node.Event.Apply(this);
                await GameStateTrackerUtility.OnAnimationEventApplied(this, config.AnimationHandlers);
                await GameStateTrackerUtility.OnEventApplied(this, config.EventHandlers);

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

            Assert.AreEqual(currentNodeI, Events.List.Count);
        }

        public class Config
        {
            public bool IsDebugMode { get; set; } = false;
            public ISerializer Serializer { get; set; }
            public IReadOnlyList<IGameAnimationHandler<TGameState>> AnimationHandlers { get; set; }
            public IReadOnlyList<IGameEventHandler<TGameState>> EventHandlers { get; set; }
        }
    }
}
