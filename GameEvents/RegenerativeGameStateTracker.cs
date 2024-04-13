using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;
using UnityEngine.Assertions;

namespace CodeName.EventSystem.GameEvents
{
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private readonly List<QueuedEvent> queuedEvents = new();

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
            var queuedEvent = new QueuedEvent
            {
                PathRaisedDuring = new List<int>(Events.PathToCurrentNode),
                CompletionSource = new StateTaskCompletionSource(),
            };

            queuedEvents.Add(queuedEvent);

            return new StateTask(queuedEvent.CompletionSource);
        }

        public async StateTask ReplayToEnd()
        {
            // Skip root node --> i = 1
            for (var i = 1; i < originalTracker.List.Count; i++)
            {
                var originalNode = originalTracker.List[i];
                while (IsParentPath(originalNode.Path, Events.PathToCurrentNode))
                {
                    PopCurrentNode();
                }

                var currentNode = Events.Push(State, originalNode.OriginalEvent);
                currentNode.ExpectedState = originalNode.ExpectedState;

                var task = ReplayNode(currentNode); // While no pop don't await, await when popping?
            }

            while (Events.PathToCurrentNode.Count != 0)
            {
                PopCurrentNode();
            }

            Assert.AreEqual(0, queuedEvents.Count);
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

        private void PopCurrentNode()
        {
            Events.Pop();
            CompleteCompletedEvents();
        }

        private void CompleteCompletedEvents()
        {
            for (var i = queuedEvents.Count - 1; i >= 0; i--)
            {
                var queuedEvent = queuedEvents[i];

                if (IsSamePath(queuedEvent.PathRaisedDuring, Events.PathToCurrentNode))
                {
                    queuedEvents.RemoveAt(queuedEvents.Count - 1);
                    queuedEvent.CompletionSource.Complete();
                }
            }
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
            if (parent.Count > path.Count)
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

        private struct QueuedEvent
        {
            public List<int> PathRaisedDuring { get; set; }
            public StateTaskCompletionSource CompletionSource { get; set; }
        }
    }
}
