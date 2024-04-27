using Cysharp.Threading.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public interface IGameAnimationHandler<TGameState>
    {
        public UniTask OnEventRaised(IGameStateTracker<TGameState> tracker);

        public UniTask OnEventConfirmed(IGameStateTracker<TGameState> tracker);

        public UniTask OnEventApplied(IGameStateTracker<TGameState> tracker);
    }
}
