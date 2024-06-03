using CodeName.EventEngine.GameEvents.Matching;

namespace CodeName.EventEngine
{
    public static class EventMatchingUtility
    {
        public static MatchOnResult<TGameEvent, TState> MatchOn<TGameEvent, TState>(this ISimulation<TState> context, EventMatchCondition<TGameEvent, TState> condition = null) where TGameEvent : GameEvent<TState>
        {
            return new MatchOnResult<TGameEvent, TState>(context, condition);
        }

        public static MatchOnResult<TGameEvent, TState> MatchOn<TGameEvent, TState>(this ISimulation<TState> context, out MatchOnResult<TGameEvent, TState> result, EventMatchCondition<TGameEvent, TState> condition = null) where TGameEvent : GameEvent<TState>
        {
            result = MatchOn(context, condition);
            return result;
        }

        public static CausedByMatchResult<TGameEvent, TState> CausedBy<TGameEvent, TState>(this INodeMatchResult<TState> context, EventMatchCondition<TGameEvent, TState> condition = null) where TGameEvent : GameEvent<TState>
        {
            return new CausedByMatchResult<TGameEvent, TState>(context, condition);
        }

        public static CausedByMatchResult<TGameEvent, TState> CausedBy<TGameEvent, TState>(this INodeMatchResult<TState> context, out CausedByMatchResult<TGameEvent, TState> result, EventMatchCondition<TGameEvent, TState> condition = null) where TGameEvent : GameEvent<TState>
        {
            result = CausedBy(context, condition);
            return result;
        }
    }
}
