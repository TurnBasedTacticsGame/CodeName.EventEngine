using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public abstract class GameEvent<TGameState>
    {
        public virtual StateTask Apply(GameStateTracker<TGameState> tracker)
        {
            return StateTask.CompletedTask;
        }

        public override string ToString()
        {
            var name = GetType().Name;

            // Remove generic argument
            var genericArgIndex = name.IndexOf('`');
            if (genericArgIndex > 0)
            {
                name = name.Substring(0, genericArgIndex);
            }

            return name;
        }
    }
}
