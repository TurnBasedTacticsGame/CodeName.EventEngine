namespace CodeName.EventSystem.State.GameEvents.Matching
{
    public delegate bool EventMatchCondition<in T>(T gameEvent, GameEventNode node) where T : GameEvent;
}
