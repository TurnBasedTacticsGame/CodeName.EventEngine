using System;
using System.Collections.Generic;
using CodeName.Serialization;

namespace CodeName.EventEngine.GameEvents
{
    public class GameEventTracker<TGameState>
    {
        private readonly ISerializer serializer;

        /// <summary>
        /// Creates a new <see cref="GameEventTracker{TGameState}"/>.
        /// </summary>
        public GameEventTracker(ISerializer serializer)
        {
            this.serializer = serializer;

            PathToCurrentNode = new List<int>();
            Tree = new GameEventNode<TGameState>(new TrackerRootEvent<TGameState>(), PathToCurrentNode, serializer);
        }

        /// <summary>
        /// Creates a new <see cref="GameEventTracker{TGameState}"/> using an existing event tree.
        /// <para/>
        /// Note: This does not clone the existing event tree. The passed in event tree is used directly.
        /// If this is not desirable, clone the tree before passing it in.
        /// </summary>
        public GameEventTracker(ISerializer serializer, GameEventNode<TGameState> root)
        {
            this.serializer = serializer;

            PathToCurrentNode = new List<int>();
            Tree = root;

            var current = root;
            while (true)
            {
                if (current.Children.Count == 0)
                {
                    break;
                }

                var lastChildIndex = current.Children.Count - 1;

                PathToCurrentNode.Add(lastChildIndex);
                current = current.Children[lastChildIndex];
            }
        }

        public GameEventNode<TGameState> Tree { get; }
        public List<int> PathToCurrentNode { get; }

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

        public GameEventNode<TGameState> Push(TGameState gameState, GameEvent<TGameState> gameEvent, EventId eventId = default)
        {
            var current = CurrentNode;
            var index = current.Children.Count;

            // Update path before passing to GameEventNode's constructor
            PathToCurrentNode.Add(index);

            var node = new GameEventNode<TGameState>(gameEvent, PathToCurrentNode, serializer, eventId);
            current.Children.Add(node);

            return node;
        }

        public void Pop()
        {
            PathToCurrentNode.RemoveAt(PathToCurrentNode.Count - 1);
        }
    }
}
