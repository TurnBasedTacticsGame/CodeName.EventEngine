namespace CodeName.EventEngine.GameEvents.Matching
{
    public readonly struct MatchOnResult<TGameEvent, TState> : INodeMatchResult<TState> where TGameEvent : GameEvent<TState>
    {
        public MatchOnResult(ISimulation<TState> context, EventMatchCondition<TGameEvent, TState> condition)
        {
            Simulation = context;
            Node = null;
            Event = null;

            var node = context.Events.CurrentNode;
            if (node.Event is TGameEvent gameEvent && (condition?.Invoke(new NodeMatchContext<TGameEvent, TState>(Simulation, node, gameEvent)) ?? true))
            {
                Node = node;
                Event = gameEvent;
            }
        }

        public bool IsSuccess => Event != null;

        public ISimulation<TState> Simulation { get; }
        public GameEventNode<TState> Node { get; }
        public TGameEvent Event { get; }

        public static implicit operator bool(MatchOnResult<TGameEvent, TState> value)
        {
            return value.IsSuccess;
        }
    }
}
