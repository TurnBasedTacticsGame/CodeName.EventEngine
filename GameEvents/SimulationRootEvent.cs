namespace CodeName.EventEngine.GameEvents
{
    /// <summary>
    /// Null object used to mark the root of a <see cref="ISimulation{TGameState}"/>'s event tree.
    /// </summary>
    public class SimulationRootEvent<TGameState> : GameEvent<TGameState> {}
}
