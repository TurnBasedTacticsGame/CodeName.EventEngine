namespace CodeName.EventEngine.GameEvents.Matching
{
    public struct CausedByMatchResult<TGameEvent, TState> : INodeMatchResult<TState> where TGameEvent : GameEvent<TState>
    {
        public CausedByMatchResult(INodeMatchResult<TState> context, EventMatchCondition<TGameEvent, TState> condition)
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
                if (currentNode.Event is TGameEvent gameEvent && (condition?.Invoke(new NodeMatchContext<TGameEvent, TState>(Simulation, currentNode, gameEvent)) ?? true))
                {
                    Node = currentNode;
                    Event = gameEvent;
                }

                currentNode = currentNode.Children[path[i]];
            }
        }

        public bool IsSuccess => Event != null;

        public ISimulation<TState> Simulation { get; }
        public GameEventNode<TState> Node { get; }
        public TGameEvent Event { get; }

        public static implicit operator bool(CausedByMatchResult<TGameEvent, TState> value)
        {
            return value.IsSuccess;
        }
    }
}
