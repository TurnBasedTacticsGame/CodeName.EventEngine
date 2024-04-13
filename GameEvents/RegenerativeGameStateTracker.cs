using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;
using UnityEngine.Assertions;

namespace CodeName.EventSystem.GameEvents
{
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private readonly List<RaisedEvent> raisedEvents = new();
        private readonly List<ReplayedEvent> replayedEvents = new();

        private readonly GameStateTrackerConfig<TGameState> config;
        private readonly GameEventTracker<TGameState> originalTracker;

        public RegenerativeGameStateTracker(
            TGameState state,
            GameEventTracker<TGameState> tracker,
            GameStateTrackerConfig<TGameState> config) : base(state, config)
        {
            this.config = config;
            originalTracker = tracker;
        }

        public override StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            var raisedEvent = new RaisedEvent
            {
                ParentId = Events.CurrentNode.Id,
                CompletionSource = new StateTaskCompletionSource(),
                Event = gameEvent,
            };

            raisedEvents.Add(raisedEvent);

            return new StateTask(raisedEvent.CompletionSource);
        }

        private async StateTask ReplayNode(GameEventNode<TGameState> node)
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

        public async StateTask ReplayToEnd()
        {
            // Skip root node --> i = 1
            for (var i = 1; i < originalTracker.List.Count; i++)
            {
                var originalNode = originalTracker.List[i];
                while (originalNode.Path.Count <= Events.PathToCurrentNode.Count)
                {
                    await PopCurrentNode();
                }

                var currentNode = Events.Push(State, originalNode.OriginalEvent);
                currentNode.ExpectedState = originalNode.ExpectedState;

                var task = ReplayNode(currentNode);
                replayedEvents.Add(new ReplayedEvent
                {
                    Id = Events.CurrentNode.Id,
                    Task = task,
                });
            }

            while (Events.PathToCurrentNode.Count != 0)
            {
                await PopCurrentNode();
            }

            Assert.AreEqual(0, raisedEvents.Count);
            Assert.AreEqual(0, replayedEvents.Count);
        }

        private async StateTask PopCurrentNode()
        {
            var removedEvent = Events.CurrentNode;
            Events.Pop();

            var raisedRemoved = new List<RaisedEvent>();
            for (var i = raisedEvents.Count - 1; i >= 0; i--)
            {
                var raisedEvent = raisedEvents[i];
                if (Events.CurrentNode.Id == raisedEvent.ParentId)
                {
                    raisedEvents.RemoveAt(raisedEvents.Count - 1);
                    raisedRemoved.Add(raisedEvent);
                    raisedEvent.CompletionSource.Complete();
                }
            }

            var replayedEventIndex = replayedEvents.FindIndex(e => e.Id == removedEvent.Id);
            Assert.IsTrue(replayedEventIndex >= 0);

            var replayedEvent = replayedEvents[replayedEventIndex];
            replayedEvents.RemoveAt(replayedEventIndex);
            await replayedEvent.Task;
        }

        private struct RaisedEvent
        {
            public EventId ParentId { get; set; }
            public StateTaskCompletionSource CompletionSource { get; set; }
            public GameEvent<TGameState> Event { get; set; }
        }

        private struct ReplayedEvent
        {
            public EventId Id { get; set; }
            public StateTask Task { get; set; }
        }
    }
}
