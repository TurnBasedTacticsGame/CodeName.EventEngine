using System;
using System.Collections.Generic;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;
using CodeName.EventSystem.Utility;

namespace CodeName.EventSystem
{
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private int currentNodeIndex;
        private readonly List<QueuedEvent> queuedEvents = new();

        private readonly GameStateTrackerConfig config;

        private readonly GameEventTracker<TGameState> originalEvents;

        public RegenerativeGameStateTracker(GameStateTracker<TGameState> tracker, ISerializer serializer, List<IGameEventHandler<TGameState>> gameEventHandlers, GameStateTrackerConfig config)
            : base(tracker.OriginalState, serializer, gameEventHandlers)
        {
            this.config = config;
            originalEvents = tracker.Events;
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

        public bool TryReplayNextEventNode(out GameEventNode<TGameState> node, out Action apply)
        {
            if (currentNodeIndex == 0)
            {
                // Skip root node
                currentNodeIndex++;
            }

            if (currentNodeIndex >= originalEvents.List.Count)
            {
                while (Events.PathToCurrentNode.Count != 0)
                {
                    PopCurrentEventNode();
                }

                node = null;
                apply = null;

                return false;
            }

            var originalNode = originalEvents.List[currentNodeIndex];
            currentNodeIndex++;

            PopToMatchingLevel(originalNode);

            var currentNode = Events.Push(State, originalNode.OriginalEvent);
            currentNode.ExpectedDebugState = originalNode.ExpectedDebugState;

            node = currentNode;
            apply = () =>
            {
                async StateTask Apply()
                {
                    await OnEventRaised(currentNode);
                    currentNode.Lock();
                    await OnEventConfirmed(currentNode);
                    await currentNode.Event.Apply(this);
                    await OnEventApplied(Events.CurrentNode);

                    if (config.IsDebugMode && currentNode.ExpectedDebugState != null)
                    {
                        DiffUtility.ValidateGameState(Serializer, State, currentNode);
                        State = Serializer.Clone(currentNode.ExpectedDebugState);
                    }
                }

                Apply().Forget();
            };

            return true;
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
        ///     Technically matching level - 1. A level is matching when pushing a new node will cause the new node to have the same path as the original node.
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
