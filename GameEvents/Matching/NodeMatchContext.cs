namespace CodeName.EventEngine.GameEvents.Matching
{
    public readonly struct NodeMatchContext<TGameEvent, TGameState> : INodeMatchResult<TGameState>
    {
        public NodeMatchContext(ISimulation<TGameState> simulation, GameEventNode<TGameState> node, TGameEvent gameEvent)
        {
            Simulation = simulation;
            Node = node;
            Event = gameEvent;
        }

        public bool IsSuccess => true;

        public ISimulation<TGameState> Simulation { get; }
        public GameEventNode<TGameState> Node { get; }

        public TGameEvent Event { get; }
    }
}
