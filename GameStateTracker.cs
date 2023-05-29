using System.Collections.Generic;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public abstract class GameStateTracker<TGameState>
    {
        protected ISerializer Serializer { get; }
        protected List<IGameEventHandler<TGameState>> GameEventHandlers { get; }

        protected GameStateTracker(TGameState state, ISerializer serializer, List<IGameEventHandler<TGameState>> gameEventHandlers)
        {
            Serializer = serializer;
            GameEventHandlers = gameEventHandlers;

            Events = new GameEventTracker<TGameState>(serializer);

            OriginalState = serializer.Clone(state);
            State = serializer.Clone(state);
        }

        public GameEventTracker<TGameState> Events { get; }

        public TGameState OriginalState { get; protected set; }
        public TGameState State { get; protected set; }

        /// <summary>
        ///     Raise an event to be applied.
        ///     <para/>
        ///     Note: An event can be Prevented. Raising an event does not guarantee it will be applied.
        ///     <para/>
        ///     When an event is raised, 3 events will be called: <br/>
        ///     1. OnEventRaised - Use to prevent events from being applied. <br/>
        ///     2. OnEventConfirmed - Use to react to events before they are applied. <br/>
        ///     3. OnEventApplied - Use to react to events after they are applied.
        /// </summary>
        public abstract StateTask RaiseEvent(GameEvent<TGameState> gameEvent);

        protected virtual async StateTask OnEventRaised(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventRaised(this, node);
            }
        }

        protected virtual async StateTask OnEventConfirmed(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventConfirmed(this, node);
            }
        }

        protected virtual async StateTask OnEventApplied(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventApplied(this, node);
            }
        }
    }
}
