using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public interface IGameAnimationHandler<TGameState>
    {
        public StateTask OnAnimationEventRaised(IGameStateTracker<TGameState> tracker);

        public StateTask OnAnimationEventConfirmed(IGameStateTracker<TGameState> tracker);

        public StateTask OnAnimationEventApplied(IGameStateTracker<TGameState> tracker);
    }
}
