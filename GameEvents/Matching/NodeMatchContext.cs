namespace CodeName.EventEngine.GameEvents.Matching
{
    public readonly struct NodeMatchContext<TGameEvent, TGameState> : INodeMatchResult<TGameState>
    {
        public NodeMatchContext(ISimulation<TGameState> tracker, GameEventNode<TGameState> node, TGameEvent gameEvent)
        {
            Tracker = tracker;
            Node = node;
            Event = gameEvent;
        }

        public bool IsSuccess => true;

        public ISimulation<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }

        public TGameEvent Event { get; }
    }
}
