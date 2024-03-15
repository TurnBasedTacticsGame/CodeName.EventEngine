using System;
using System.Collections.Generic;
using CodeName.EventSystem.GameEvents;
using CodeName.EventSystem.Tasks;

namespace CodeName.EventSystem
{
    public abstract class GameStateTracker<TGameState> : IGameStateTracker<TGameState>
    {
        protected GameStateTracker(TGameState state, GameStateTrackerConfig<TGameState> config, bool initialize = true)
        {
            if (initialize)
            {
                Initialize(state, config);
            }
        }

        public GameStateTrackerConfig<TGameState> Config { get; protected set; }
        public GameEventTracker<TGameState> Events { get; protected set; }

        public virtual GameEventNode<TGameState> Tree => Events.Tree;
        [Obsolete]
        public virtual List<GameEventNode<TGameState>> List => Events.List;
        public virtual IReadOnlyList<int> PathToCurrentNode => Events.PathToCurrentNode;

        public GameEventNode<TGameState> CurrentNode
        {
            get
            {
                var current = Tree;

                foreach (var index in PathToCurrentNode)
                {
                    current = current.Children[index];
                }

                return current;
            }
        }

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

        protected void Initialize(TGameState state, GameStateTrackerConfig<TGameState> config)
        {
            Config = config;
            Events = new GameEventTracker<TGameState>(config.Serializer);

            if (config.CloneState)
            {
                OriginalState = config.Serializer.Clone(state);
                State = config.Serializer.Clone(state);
            }
            else
            {
                OriginalState = state;
                State = state;
            }
        }

        protected virtual async StateTask OnEventRaised(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in Config.GameEventHandlers)
            {
                await gameEventHandler.OnEventRaised(this, node);
            }
        }

        protected virtual async StateTask OnEventConfirmed(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in Config.GameEventHandlers)
            {
                await gameEventHandler.OnEventConfirmed(this, node);
            }
        }

        protected virtual async StateTask OnEventApplied(GameEventNode<TGameState> node)
        {
            foreach (var gameEventHandler in Config.GameEventHandlers)
            {
                await gameEventHandler.OnEventApplied(this, node);
            }
        }
    }
}
