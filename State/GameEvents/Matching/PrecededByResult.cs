namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public struct PrecededByMatchResult<TGameEvent, TGameState> : INodeMatchResult<TGameState> where TGameEvent : GameEvent<TGameState>
    {
        public PrecededByMatchResult(INodeMatchResult<TGameState> context, EventMatchCondition<TGameEvent, TGameState> condition)
        {
            Tracker = context.Tracker;
            Node = null;
            Event = null;

            if (!context.IsSuccess)
            {
                return;
            }

            var events = context.Tracker.Events.List;
            var startIndex = events.FindIndex(node => node.Id == context.Node.Id) - 1;
            for (var i = startIndex; i >= 0; i--)
            {
                var node = events[i];
                if (node.Event is TGameEvent gameEvent && (condition?.Invoke(gameEvent, node) ?? true))
                {
                    Node = node;
                    Event = gameEvent;
                }
            }
        }

        public bool IsSuccess => Event != null;

        public GameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }
        public TGameEvent Event { get; }

        public static implicit operator bool(PrecededByMatchResult<TGameEvent, TGameState> value)
        {
            return value.IsSuccess;
        }
    }
}
