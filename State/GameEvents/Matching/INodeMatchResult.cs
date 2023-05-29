namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public interface INodeMatchResult<TGameState> : IMatchResult
    {
        public GameStateTracker<TGameState> Tracker { get; }
        public GameEventNode<TGameState> Node { get; }
    }
}
