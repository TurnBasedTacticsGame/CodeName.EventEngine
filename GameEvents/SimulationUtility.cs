using System.Collections.Generic;
using CodeName.EventEngine.Tasks;

namespace CodeName.EventEngine.GameEvents
{
    public static class SimulationUtility
    {
        public static async StateTask OnEventRaised<TState>(ISimulation<TState> simulation, IEnumerable<IEventHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventRaised(simulation);
            }
        }

        public static async StateTask OnEventConfirmed<TState>(ISimulation<TState> simulation, IEnumerable<IEventHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventConfirmed(simulation);
            }
        }

        public static async StateTask OnEventApplied<TState>(ISimulation<TState> simulation, IEnumerable<IEventHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnEventApplied(simulation);
            }
        }

        public static async StateTask OnAnimationEventRaised<TState>(ISimulation<TState> simulation, IEnumerable<IAnimationHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventRaised(simulation);
            }
        }

        public static async StateTask OnAnimationEventConfirmed<TState>(ISimulation<TState> simulation, IEnumerable<IAnimationHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventConfirmed(simulation);
            }
        }

        public static async StateTask OnAnimationEventApplied<TState>(ISimulation<TState> simulation, IEnumerable<IAnimationHandler<TState>> eventHandlers)
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.OnAnimationEventApplied(simulation);
            }
        }
    }
}
