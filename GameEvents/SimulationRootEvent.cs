namespace CodeName.EventEngine.GameEvents
{
    /// <summary>
    /// Null object used to mark the root of a <see cref="ISimulation{TState}"/>'s event tree.
    /// </summary>
    public class SimulationRootEvent<TState> : GameEvent<TState> {}
}
