using System.Collections.Generic;
using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public static class SimulationUtility
    {
        public static async StateTask OnEventRaised<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventRaised(simulation);
            }
        }

        public static async StateTask OnEventConfirmed<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventConfirmed(simulation);
            }
        }

        public static async StateTask OnEventApplied<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IEventHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventApplied(simulation);
            }
        }

        public static async StateTask OnAnimationEventRaised<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventRaised(simulation);
            }
        }

        public static async StateTask OnAnimationEventConfirmed<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventConfirmed(simulation);
            }
        }

        public static async StateTask OnAnimationEventApplied<TGameState>(ISimulation<TGameState> simulation, IEnumerable<IAnimationHandler<TGameState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventApplied(simulation);
            }
        }
    }
}
