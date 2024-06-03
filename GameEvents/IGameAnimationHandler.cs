using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IGameAnimationHandler<TGameState>
    {
        public StateTask OnAnimationEventRaised(ISimulation<TGameState> tracker);

        public StateTask OnAnimationEventConfirmed(ISimulation<TGameState> tracker);

        public StateTask OnAnimationEventApplied(ISimulation<TGameState> tracker);
    }
}
