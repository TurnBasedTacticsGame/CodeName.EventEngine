using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State.GameEvents
{
    public interface IGameEventHandler
    {
        public StateTask OnEventRaised(GameStateTracker tracker, GameEventNode node);

        public StateTask OnEventConfirmed(GameStateTracker tracker, GameEventNode node);

        public StateTask OnEventApplied(GameStateTracker tracker, GameEventNode node);
    }
}
