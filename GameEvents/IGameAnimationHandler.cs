using CodeName.EventSystem.Tasks;
using Cysharp.Threading.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public interface IGameAnimationHandler<TGameState>
    {
        public StateTask OnEventRaised(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventConfirmed(IGameStateTracker<TGameState> tracker);

        public StateTask OnEventApplied(IGameStateTracker<TGameState> tracker);
    }
}
