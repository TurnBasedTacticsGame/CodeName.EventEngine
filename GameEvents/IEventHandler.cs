using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IEventHandler<TGameState>
    {
        public StateTask OnEventRaised(ISimulation<TGameState> simulation);

        public StateTask OnEventConfirmed(ISimulation<TGameState> simulation);

        public StateTask OnEventApplied(ISimulation<TGameState> simulation);
    }
}
