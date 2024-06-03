using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IAnimationHandler<TState>
    {
        public StateTask OnAnimationEventRaised(ISimulation<TState> simulation);

        public StateTask OnAnimationEventConfirmed(ISimulation<TState> simulation);

        public StateTask OnAnimationEventApplied(ISimulation<TState> simulation);
    }
}
