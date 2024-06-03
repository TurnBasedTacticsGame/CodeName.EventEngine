namespace CodeName.EventEngine.GameEvents.Matching
{
    public interface INodeMatchResult<TState>
    {
        public bool IsSuccess { get; }

        public ISimulation<TState> Simulation { get; }
        public GameEventNode<TState> Node { get; }
    }
}
