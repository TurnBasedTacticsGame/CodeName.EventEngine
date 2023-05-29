using CodeName.EventSystem.State.GameEvents.Matching;

namespace CodeName.EventSystem.State
{
    public static class EventMatchingUtility
    {
        public static MatchOnResult<TGameEvent, TGameState> MatchOn<TGameEvent, TGameState>(this GameStateTracker<TGameState> context, out MatchOnResult<TGameEvent, TGameState> result, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            result = new MatchOnResult<TGameEvent, TGameState>(context, condition);

            return result;
        }

        public static CausedByMatchResult<TGameEvent, TGameState> CausedBy<TGameEvent, TGameState>(this INodeMatchResult<TGameState> context, out CausedByMatchResult<TGameEvent, TGameState> result, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            result = new CausedByMatchResult<TGameEvent, TGameState>(context, condition);

            return result;
        }

        public static PrecededByMatchResult<TGameEvent, TGameState> PrecededBy<TGameEvent, TGameState>(this INodeMatchResult<TGameState> context, out PrecededByMatchResult<TGameEvent, TGameState> result, EventMatchCondition<TGameEvent, TGameState> condition = null) where TGameEvent : GameEvent<TGameState>
        {
            result = new PrecededByMatchResult<TGameEvent, TGameState>(context, condition);

            return result;
        }
    }
}
