namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public struct CausedByMatchResult<T> : INodeMatchResult where T : GameEvent
    {
        public CausedByMatchResult(INodeMatchResult context, EventMatchCondition<T> condition)
        {
            Tracker = context.Tracker;
            Node = null;
            Event = null;

            if (!context.IsSuccess)
            {
                return;
            }

            var path = context.Node.Path;
            var currentNode = context.Tracker.Events.Tree;

            for (var i = 0; i < path.Count; i++)
            {
                if (currentNode.Event is T gameEvent && (condition?.Invoke(gameEvent, currentNode) ?? true))
                {
                    Node = currentNode;
                    Event = gameEvent;
                }

                currentNode = currentNode.Children[path[i]];
            }
        }

        public bool IsSuccess => Event != null;

        public GameStateTracker Tracker { get; }
        public GameEventNode Node { get; }
        public T Event { get; }

        public static implicit operator bool(CausedByMatchResult<T> value)
        {
            return value.IsSuccess;
        }
    }
}
