using System.Collections.Generic;

namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public struct PrecededByMatchResult<T> : INodeMatchResult where T : GameEvent
    {
        public PrecededByMatchResult(INodeMatchResult context, EventMatchCondition<T> condition)
        {
            Tracker = context.Tracker;
            Node = null;
            Event = null;

            if (!context.IsSuccess)
            {
                return;
            }

            var events = context.Tracker.Events.List;
            var startIndex = events.BinarySearch(context.Node, new CompareById()) - 1;
            for (var i = startIndex; i >= 0; i--)
            {
                var node = events[i];
                if (node.Event is T gameEvent && (condition?.Invoke(gameEvent, node) ?? true))
                {
                    Node = node;
                    Event = gameEvent;
                }
            }
        }

        public bool IsSuccess => Event != null;

        public GameStateTracker Tracker { get; }
        public GameEventNode Node { get; }
        public T Event { get; }

        public static implicit operator bool(PrecededByMatchResult<T> value)
        {
            return value.IsSuccess;
        }

        private struct CompareById : IComparer<GameEventNode>
        {
            public int Compare(GameEventNode x, GameEventNode y)
            {
                return ((int)x!.Id).CompareTo((int)y!.Id);
            }
        }
    }
}
