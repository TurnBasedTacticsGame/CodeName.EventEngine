using System;
using System.Collections.Generic;
using System.Web;
using CodeName.EventSystem.State.GameEvents;
using CodeName.EventSystem.State.Serialization;
using CodeName.EventSystem.State.Tasks;
using UnityEngine;

namespace CodeName.EventSystem.State
{
    public class RegenerativeGameStateTracker<TGameState> : GameStateTracker<TGameState>
    {
        private int currentNodeIndex;
        private readonly List<QueuedEvent> queuedEvents = new();

        private readonly GameStateTracker<TGameState> originalTracker;
        private readonly GameEventTracker<TGameState> originalEvents;

        public RegenerativeGameStateTracker(GameStateTracker<TGameState> tracker, GameStateSerializer serializer, List<IGameEventHandler<TGameState>> gameEventHandlers) : base(tracker.OriginalState, serializer, gameEventHandlers)
        {
            originalTracker = tracker;
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

                    ValidateCurrentGameState(Events.CurrentNode);
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

        public void ValidateTrackerState()
        {
            var current = State;
            var expected = originalTracker.State;

            if (HasDifferences(current, expected, out var currentJson, out var expectedJson))
            {
                Debug.LogWarning("Mismatch between current and expected game state:" +
                    $"\n\nDiff: {CreateDiffLink(currentJson, expectedJson)} (Current on left, expected on right)\n");
            }

            if (Events.Tree.ToString() != originalEvents.Tree.ToString())
            {
                Debug.LogWarning("Mismatch between current and expected event trees:" +
                    $"\n\nCurrent: {Events.Tree}" +
                    $"\n\nExpected: {originalEvents.Tree}");
            }
        }

        private void ValidateCurrentGameState(GameEventNode<TGameState> node)
        {
            if (Constants.IsDebugMode && node.ExpectedDebugState != null)
            {
                var current = State;
                var expected = node.ExpectedDebugState;

                if (HasDifferences(current, expected, out var currentJson, out var expectedJson))
                {
                    Debug.LogWarning("Setting current state to expected state. Mismatch between current and expected game state while replaying events:" +
                        $"\n\nDiff: {CreateDiffLink(currentJson, expectedJson)} (Current on left, expected on right)\n");

                    State = Serializer.Clone(expected);
                }
            }
        }

        private bool HasDifferences(TGameState current, TGameState expected, out string currentJson, out string expectedJson)
        {
            currentJson = Serializer.Serialize(current);
            expectedJson = Serializer.Serialize(expected);

            return currentJson != expectedJson;
        }

        private string CreateDiffLink(string currentJson, string expectedJson)
        {
            var link = $"https://jsoneditoronline.org/#left=json.{HttpUtility.UrlEncode(currentJson)}&right=json.{HttpUtility.UrlEncode(expectedJson)}";

            return $"<a href=\"{link}\">Json Editor Online</a>";
        }

        private struct QueuedEvent
        {
            public List<int> Path { get; set; }
            public StateTaskCompletionSource CompletionSource { get; set; }
        }
    }
}
