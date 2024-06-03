namespace CodeName.EventEngine.GameEvents.Matching
{
    public interface INodeMatchResult<TGameState>
    {
        public bool IsSuccess { get; }

        public ISimulation<TGameState> Simulation { get; }
        public GameEventNode<TGameState> Node { get; }
    }
}
