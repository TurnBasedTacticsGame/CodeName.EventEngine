using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IAnimationHandler<TGameState>
    {
        public StateTask OnAnimationEventRaised(ISimulation<TGameState> simulation);

        public StateTask OnAnimationEventConfirmed(ISimulation<TGameState> simulation);

        public StateTask OnAnimationEventApplied(ISimulation<TGameState> simulation);
    }
}
