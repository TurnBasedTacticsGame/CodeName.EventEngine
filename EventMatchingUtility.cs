using CodeName.EventEngine.GameEvents.Matching;

namespace CodeName.EventEngine
{
    public static class EventMatchingUtility
    {
        public static MatchOnResult<TGameEvent, TGameState> MatchOn<TGameEvent, TGameState>(this ISimulation<TGameState> context, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            return new MatchOnResult<TGameEvent, TGameState>(context, condition);
        }

        public static MatchOnResult<TGameEvent, TGameState> MatchOn<TGameEvent, TGameState>(this ISimulation<TGameState> context, out MatchOnResult<TGameEvent, TGameState> result, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            result = MatchOn(context, condition);
            return result;
        }

        public static CausedByMatchResult<TGameEvent, TGameState> CausedBy<TGameEvent, TGameState>(this INodeMatchResult<TGameState> context, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            return new CausedByMatchResult<TGameEvent, TGameState>(context, condition);
        }

        public static CausedByMatchResult<TGameEvent, TGameState> CausedBy<TGameEvent, TGameState>(this INodeMatchResult<TGameState> context, out CausedByMatchResult<TGameEvent, TGameState> result, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            result = CausedBy(context, condition);
            return result;
        }
    }
}
