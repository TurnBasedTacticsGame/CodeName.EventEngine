namespace CodeName.EventEngine.GameEvents.Matching
{
    public readonly struct NodeMatchContext<TGameEvent, TState> : INodeMatchResult<TState>
    {
        public NodeMatchContext(ISimulation<TState> simulation, GameEventNode<TState> node, TGameEvent gameEvent)
        {
            Simulation = simulation;
            Node = node;
            Event = gameEvent;
        }

        public bool IsSuccess => true;

        public ISimulation<TState> Simulation { get; }
        public GameEventNode<TState> Node { get; }

        public TGameEvent Event { get; }
    }
}
