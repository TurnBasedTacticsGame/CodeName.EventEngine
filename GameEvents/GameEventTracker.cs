using System.Collections.Generic;
using CodeName.Serialization;

namespace CodeName.EventSystem.GameEvents
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

            var root = new GameEventNode<TGameState>(new TrackerRootEvent<TGameState>(), PathToCurrentNode, serializer);

            Tree = root;
            List = new List<GameEventNode<TGameState>>
            {
                root,
            };
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

            Tree = root;

            List = new List<GameEventNode<TGameState>>();
            FlattenEventTree(root, List);

            PathToCurrentNode = new List<int>(List[List.Count - 1].Path);
        }

        public GameEventNode<TGameState> Tree { get; }
        public List<GameEventNode<TGameState>> List { get; }
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

        public GameEventNode<TGameState> Push(TGameState gameState, GameEvent<TGameState> gameEvent)
        {
            var current = CurrentNode;
            var index = current.Children.Count;

            // Update path before passing to GameEventNode's constructor
            PathToCurrentNode.Add(index);

            var node = new GameEventNode<TGameState>(gameEvent, PathToCurrentNode, serializer);
            current.Children.Add(node);

            List.Add(node);

            return node;
        }

        public void Pop()
        {
            PathToCurrentNode.RemoveAt(PathToCurrentNode.Count - 1);
        }

        private void FlattenEventTree(GameEventNode<TGameState> root, List<GameEventNode<TGameState>> results)
        {
            results.Add(root);
            foreach (var childEventNode in root.Children)
            {
                FlattenEventTree(childEventNode, results);
            }
        }
    }
}
