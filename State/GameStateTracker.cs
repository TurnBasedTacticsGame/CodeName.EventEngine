using System.Collections.Generic;
using CodeName.EventSystem.State.GameEvents;
using CodeName.EventSystem.State.Serialization;
using CodeName.EventSystem.State.Tasks;

namespace CodeName.EventSystem.State
{
    public abstract class GameStateTracker
    {
        protected GameStateSerializer Serializer { get; }
        protected List<IGameEventHandler> GameEventHandlers { get; }

        protected GameStateTracker(GameState state, GameStateSerializer serializer, List<IGameEventHandler> gameEventHandlers)
        {
            Serializer = serializer;
            GameEventHandlers = gameEventHandlers;

            Events = new GameEventTracker(serializer);

            OriginalState = serializer.Clone(state);
            State = serializer.Clone(state);
        }

        public GameEventTracker Events { get; }

        public GameState OriginalState { get; protected set; }
        public GameState State { get; protected set; }

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
        public abstract StateTask RaiseEvent(GameEvent gameEvent);

        protected virtual async StateTask OnEventRaised(GameEventNode node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventRaised(this, node);
            }
        }

        protected virtual async StateTask OnEventConfirmed(GameEventNode node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventConfirmed(this, node);
            }
        }

        protected virtual async StateTask OnEventApplied(GameEventNode node)
        {
            foreach (var gameEventHandler in GameEventHandlers)
            {
                await gameEventHandler.OnEventApplied(this, node);
            }
        }
    }
}
