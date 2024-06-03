namespace CodeName.EventEngine.GameEvents.Matching
{
    public delegate bool EventMatchCondition<TGameEvent, TGameState>(NodeMatchContext<TGameEvent, TGameState> context) where TGameEvent : GameEvent<TGameState>;
}
