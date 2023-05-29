namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public interface INodeMatchResult : IMatchResult
    {
        public GameStateTracker Tracker { get; }
        public GameEventNode Node { get; }
    }
}
