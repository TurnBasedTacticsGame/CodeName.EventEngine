namespace CodeName.EventEngine.GameEvents.Matching
{
    public struct CausedByMatchResult<TGameEvent, TGameState> : INodeMatchResult<TGameState> where TGameEvent : GameEvent<TGameState>
    {
        public CausedByMatchResult(INodeMatchResult<TGameState> context, EventMatchCondition<TGameEvent, TGameState> condition)
        {
            Simulation = context.Simulation;
            Node = null;
            Event = null;

            if (!context.IsSuccess)
            {
                return;
            }

            var path = context.Node.Path;
            var currentNode = context.Simulation.Events.Tree;

            for (var i = 0; i < path.Count; i++)
            {
                if (currentNode.Event is TGameEvent gameEvent && (condition?.Invoke(new NodeMatchContext<TGameEvent, TGameState>(Simulation, currentNode, gameEvent)) ?? true))
                {
                    Node = currentNode;
                    Event = gameEvent;
                }

                currentNode = currentNode.Children[path[i]];
            }
        }

        public bool IsSuccess => Event != null;

        public ISimulation<TGameState> Simulation { get; }
        public GameEventNode<TGameState> Node { get; }
        public TGameEvent Event { get; }

        public static implicit operator bool(CausedByMatchResult<TGameEvent, TGameState> value)
        {
            return value.IsSuccess;
        }
    }
}
