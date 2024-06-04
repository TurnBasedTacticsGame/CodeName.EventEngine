namespace CodeName.EventEngine.GameEvents.Matching
{
    /// <remarks>
    /// Since this is an interface and most MatchResults are implemented as structs, the use of this interface will cause boxing in certain cases.
    /// However, implementing MatchResults as classes will cause more allocations and pooling is difficult.
    /// </remarks>
    public interface INodeMatchResult<TGameState>
    {
        public bool IsSuccess { get; }

        public ISimulation<TGameState> Simulation { get; }
        public GameEventNode<TGameState> Node { get; }
    }
}
