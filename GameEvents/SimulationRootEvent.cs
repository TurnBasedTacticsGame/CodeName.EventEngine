namespace CodeName.EventEngine.GameEvents
{
    /// <summary>
    /// Null object used to mark the root of the GameStateTracker event tree.
    /// </summary>
    public class SimulationRootEvent<TGameState> : GameEvent<TGameState> {}
}
