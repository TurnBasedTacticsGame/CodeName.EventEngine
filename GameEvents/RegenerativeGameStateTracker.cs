using System;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;
using UnityEngine.Assertions;

namespace CodeName.EventSystem.GameEvents
{
    /// <remarks>
    /// This implementation requires that the same events are raised during the replay compared to the original.
    /// This will need to be rewritten later on again.
    /// </remarks>
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private int currentNodeI = 1;

        private readonly GameStateTrackerConfig<TGameState> config;
        private readonly GameEventTracker<TGameState> originalTracker;

        public RegenerativeGameStateTracker(
            TGameState state,
            GameEventTracker<TGameState> tracker,
            GameStateTrackerConfig<TGameState> config) : base(state, config)
        {
            this.config = config;
            originalTracker = tracker;
            Events = new GameEventTracker<TGameState>(config.Serializer, new GameEventNode<TGameState>(new TrackerRootEvent<TGameState>(), Array.Empty<int>(), config.Serializer, tracker.Tree.Id));
        }

        public override async StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            // * Needs more analysis
            // This will break when more/less/different events are created on the server compared to the client
            await ReplayNextNode();
        }

        private async StateTask ReplayNextNode()
        {
            Assert.IsTrue(currentNodeI < originalTracker.List.Count);
            var originalNode = originalTracker.List[currentNodeI];
            var node = Events.Push(State, originalNode.Event, originalNode.Id);
            currentNodeI++;
            {
                await OnEventRaised(node);
                node.Lock();
                await OnEventConfirmed(node);
                await node.Event.Apply(this);
                await OnEventApplied(node);

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
    }
}
