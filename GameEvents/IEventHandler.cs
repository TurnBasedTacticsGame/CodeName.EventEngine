using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IEventHandler<TState>
    {
        public StateTask OnEventRaised(ISimulation<TState> simulation);

        public StateTask OnEventConfirmed(ISimulation<TState> simulation);

        public StateTask OnEventApplied(ISimulation<TState> simulation);
    }
}
