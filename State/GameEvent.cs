using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State
{
    public abstract class GameEvent
    {
        public virtual StateTask Apply(GameStateTracker tracker)
        {
            return StateTask.CompletedTask;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
