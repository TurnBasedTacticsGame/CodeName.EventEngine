using System.Collections.Generic;
using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public static class GameStateTrackerUtility
    {
        public static async StateTask OnEventRaised<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventRaised(tracker);
            }
        }

        public static async StateTask OnEventConfirmed<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventConfirmed(tracker);
            }
        }

        public static async StateTask OnEventApplied<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventApplied(tracker);
            }
        }

        public static async StateTask OnAnimationEventRaised<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventRaised(tracker);
            }
        }

        public static async StateTask OnAnimationEventConfirmed<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventConfirmed(tracker);
            }
        }

        public static async StateTask OnAnimationEventApplied<TGameState>(ISimulation<TGameState> tracker, IEnumerable<IGameAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventApplied(tracker);
            }
        }
    }
}
