namespace CodeName.EventSystem.GameEvents.Matching
{
    public delegate bool EventMatchCondition<in TGameEvent, TGameState>(TGameEvent gameEvent, GameEventNode<TGameState> node) where TGameEvent : GameEvent<TGameState>;
}
