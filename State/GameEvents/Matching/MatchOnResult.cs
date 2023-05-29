namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public readonly struct MatchOnResult<TGameEvent, TGameState> : INodeMatchResult<TGameState> where TGameEvent : GameEvent<TGameState>
    {
        public MatchOnResult(GameStateTracker<TGameState> context, EventMatchCondition<TGameEvent, TGameState> condition)
        {
            Tracker = context;
            Node = null;
            Event = null;

            var node = context.Events.CurrentNode;
            if (node.Event is TGameEvent gameEvent && (condition?.Invoke(gameEvent, node) ?? true))
            {
                Node = node;
                Event = gameEvent;
            }
        }

        public bool IsSuccess => Event != null;

        public GameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }
        public TGameEvent Event { get; }

        public static implicit operator bool(MatchOnResult<TGameEvent, TGameState> value)
        {
            return value.IsSuccess;
        }
    }
}
