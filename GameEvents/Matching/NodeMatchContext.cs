namespace CodeName.EventEngine.GameEvents.Matching
{
    public readonly struct NodeMatchContext<TGameEvent, TGameState> : INodeMatchResult<TGameState>
    {
        public NodeMatchContext(IGameStateTracker<TGameState> tracker, GameEventNode<TGameState> node, TGameEvent gameEvent)
        {
            Tracker = tracker;
            Node = node;
            Event = gameEvent;
        }

        public bool IsSuccess => true;

        public IGameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }

        public TGameEvent Event { get; }
    }
}
