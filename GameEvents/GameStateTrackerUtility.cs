using System.Collections.Generic;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem.GameEvents
{
    public static class GameStateTrackerUtility
    {
        public static async StateTask OnEventRaised<TGameState>(IGameStateTracker<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventRaised(tracker);
            }
        }

        public static async StateTask OnEventConfirmed<TGameState>(IGameStateTracker<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventConfirmed(tracker);
            }
        }

        public static async StateTask OnEventApplied<TGameState>(IGameStateTracker<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventApplied(tracker);
            }
        }
    }
}
