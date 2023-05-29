namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public readonly struct MatchOnResult<T> : INodeMatchResult where T : GameEvent
    {
        public MatchOnResult(GameStateTracker context, EventMatchCondition<T> condition)
        {
            Tracker = context;
            Node = null;
            Event = null;

            var node = context.Events.CurrentNode;
            if (node.Event is T gameEvent && (condition?.Invoke(gameEvent, node) ?? true))
            {
                Node = node;
                Event = gameEvent;
            }
        }

        public bool IsSuccess => Event != null;

        public GameStateTracker Tracker { get; }
        public GameEventNode Node { get; }
        public T Event { get; }

        public static implicit operator bool(MatchOnResult<T> value)
        {
            return value.IsSuccess;
        }
    }
}
