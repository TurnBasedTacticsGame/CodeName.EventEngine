namespace CodeName.EventSystem.GameEvents.Matching
{
    public interface INodeMatchResult<TGameState>
    {
        public bool IsSuccess { get; }

        public GameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }
    }
}
