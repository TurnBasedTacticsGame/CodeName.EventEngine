namespace CodeName.EventSystem.GameEvents.Matching
{
    public readonly struct NodeMatchContext<TGameEvent, TGameState> : INodeMatchResult<TGameState>
    {
        public NodeMatchContext(GameStateTracker<TGameState> tracker, GameEventNode<TGameState> node, TGameEvent gameEvent)
        {
            Tracker = tracker;
            Node = node;
            Event = gameEvent;
        }

        public bool IsSuccess => true;

        public GameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }

        public TGameEvent Event { get; }
    }
}
