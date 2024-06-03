using CodeName.EventEngine.Tasks;
using CodeName.Serialization.Validation;

namespace CodeName.EventEngine
{
    [ValidateSerializeByValue]
    public abstract class GameEvent<TGameState>
    {
        public virtual StateTask Apply(ISimulation<TGameState> simulation)
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
