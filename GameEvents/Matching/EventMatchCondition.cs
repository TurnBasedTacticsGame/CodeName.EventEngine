namespace CodeName.EventEngine.GameEvents.Matching
{
    public delegate bool EventMatchCondition<TGameEvent, TState>(NodeMatchContext<TGameEvent, TState> context) where TGameEvent : GameEvent<TState>;
}
