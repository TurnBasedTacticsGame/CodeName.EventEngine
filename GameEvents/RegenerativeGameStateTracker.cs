using System.Collections.Generic;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;

namespace CodeName.EventSystem.GameEvents
{
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private int currentNodeIndex;
        private readonly List<QueuedEvent> queuedEvents = new();

        private readonly GameStateTrackerConfig<TGameState> config;

        private readonly GameEventTracker<TGameState> original;

        public RegenerativeGameStateTracker(
            TGameState state,
            GameEventTracker<TGameState> tracker,
            GameStateTrackerConfig<TGameState> config) : base(state, config)
        {
            this.config = config;
            original = tracker;
        }

        public override StateTask RaiseEvent(GameEvent<TGameState> gameEvent)
        {
            // Save current event path
            // When a GameEventNode is popped, all queued events with a matching path will be completed
            var queuedEvent = new QueuedEvent
            {
                Path = new List<int>(Events.PathToCurrentNode),
                CompletionSource = new StateTaskCompletionSource(),
            };

            queuedEvents.Add(queuedEvent);

            return new StateTask(queuedEvent.CompletionSource);
        }

        public async StateTask ReplayToEnd()
        {
            // Skip root node --> i = 1
            for (var i = 1; i < original.List.Count; i++)
            {
                var originalNode = original.List[currentNodeIndex];
                currentNodeIndex++;

                PopToMatchingLevel(originalNode);

                var currentNode = Events.Push(State, originalNode.OriginalEvent);
                currentNode.ExpectedState = originalNode.ExpectedState;

                await ReplayNode(currentNode);
            }

            while (Events.PathToCurrentNode.Count != 0)
            {
                PopCurrentEventNode();
            }
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

        private void PopToMatchingLevel(GameEventNode<TGameState> originalNode)
        {
            while (!IsMatchingLevel(originalNode) && Events.PathToCurrentNode.Count != 0)
            {
                PopCurrentEventNode();
            }
        }

        private void PopCurrentEventNode()
        {
            Events.Pop();
            CompleteCompletedEvents();
        }

        private void CompleteCompletedEvents()
        {
            bool IsMatchingPath(List<int> a, List<int> b)
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

            for (var i = queuedEvents.Count - 1; i >= 0; i--)
            {
                var queuedEvent = queuedEvents[i];

                if (IsMatchingPath(queuedEvent.Path, Events.PathToCurrentNode))
                {
                    queuedEvents.RemoveAt(queuedEvents.Count - 1);
                    queuedEvent.CompletionSource.Complete();
                }
            }
        }

        /// <summary>
        /// Technically matching level - 1. A level is matching when pushing a new node will cause the new node to have the same path as the original node.
        /// </summary>
        private bool IsMatchingLevel(GameEventNode<TGameState> originalNode)
        {
            if (Events.PathToCurrentNode.Count != originalNode.Path.Count - 1)
            {
                return false;
            }

            for (var i = 0; i < originalNode.Path.Count - 1; i++)
            {
                if (originalNode.Path[i] != Events.PathToCurrentNode[i])
                {
                    return false;
                }
            }

            return true;
        }

        private struct QueuedEvent
        {
            public List<int> Path { get; set; }
            public StateTaskCompletionSource CompletionSource { get; set; }
        }
    }
}
