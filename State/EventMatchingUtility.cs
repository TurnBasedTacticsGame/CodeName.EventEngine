using CodeName.EventSystem.State.GameEvents.Matching;

namespace CodeName.EventSystem.State
{
    public static class EventMatchingUtility
    {
        public static MatchOnResult<T> MatchOn<T>(this GameStateTracker context, out MatchOnResult<T> result, EventMatchCondition<T> condition = null) where T : GameEvent
        {
            result = new MatchOnResult<T>(context, condition);

            return result;
        }

        public static CausedByMatchResult<T> CausedBy<T>(this INodeMatchResult context, out CausedByMatchResult<T> result, EventMatchCondition<T> condition = null) where T : GameEvent
        {
            result = new CausedByMatchResult<T>(context, condition);

            return result;
        }

        public static PrecededByMatchResult<T> PrecededBy<T>(this INodeMatchResult context, out PrecededByMatchResult<T> result, EventMatchCondition<T> condition = null) where T : GameEvent
        {
            result = new PrecededByMatchResult<T>(context, condition);

            return result;
        }
    }
}
