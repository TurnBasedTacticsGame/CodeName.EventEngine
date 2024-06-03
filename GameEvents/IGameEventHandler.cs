using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IGameEventHandler<TGameState>
    {
        public StateTask OnEventRaised(ISimulation<TGameState> tracker);

        public StateTask OnEventConfirmed(ISimulation<TGameState> tracker);

        public StateTask OnEventApplied(ISimulation<TGameState> tracker);
    }
}
