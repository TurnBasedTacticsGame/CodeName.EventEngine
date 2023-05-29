using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State
{
    public abstract class GameEvent<TGameState>
    {
        public virtual StateTask Apply(GameStateTracker<TGameState> tracker)
        {
            return StateTask.CompletedTask;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
