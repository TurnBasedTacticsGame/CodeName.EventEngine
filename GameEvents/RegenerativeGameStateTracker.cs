using System.Collections.Generic;
using System.Linq;
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
            // Save current event path
            // When a GameEventNode is popped, all queued events with a matching path will be completed
            var queuedEvent = new RaisedEvent
            {
                ParentId = Events.CurrentNode.Id,
                ParentPath = new List<int>(Events.PathToCurrentNode),
                CompletionSource = new StateTaskCompletionSource(),
                Event = gameEvent,
            };

            raisedEvents.Add(queuedEvent);

            return new StateTask(queuedEvent.CompletionSource);
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

                var task = ReplayNode(currentNode); // While no pop don't await, await when popping?
                replayedEvents.Add(new ReplayedEvent
                {
                    Id = Events.CurrentNode.Id,
                    Path = Events.PathToCurrentNode.ToList(),
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
                if (IsSamePath(Events.PathToCurrentNode, raisedEvent.ParentPath))
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

        private bool IsSamePath(List<int> a, List<int> b)
        {
            if (a.Count != b.Count)
            {
                return false;
            }

            for (var i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsParentPath(List<int> parent, List<int> path)
        {
            if (parent.Count >= path.Count)
            {
                return false;
            }

            for (var i = 0; i < parent.Count - 1; i++)
            {
                if (parent[i] != path[i])
                {
                    return false;
                }
            }

            return true;
        }

        private struct RaisedEvent
        {
            public EventId ParentId { get; set; }

            /// <summary>
            /// The path to the current node when the event was raised. This is the path to the parent, not the raised event.
            /// </summary>
            public List<int> ParentPath { get; set; }
            public StateTaskCompletionSource CompletionSource { get; set; }
            public GameEvent<TGameState> Event { get; set; }
        }

        private struct ReplayedEvent
        {
            public EventId Id { get; set; }

            /// <summary>
            /// The path of the replayed event.
            /// </summary>
            public List<int> Path { get; set; }
            public StateTask Task { get; set; }
        }
    }
}
