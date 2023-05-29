using System.Collections.Generic;

namespace CodeName.EventSystem.GameEvents
{
    public class GameEventTracker<TGameState>
    {
        private readonly GameStateSerializer serializer;

        public GameEventTracker(GameStateSerializer serializer)
        {
            this.serializer = serializer;

            var root = new GameEventNode<TGameState>(new TrackerRootEvent<TGameState>(), PathToCurrentNode, serializer);

            Tree = root;
            List = new List<GameEventNode<TGameState>>
            {
                root,
            };
        }

        public GameEventNode<TGameState> Tree { get; }
        public List<GameEventNode<TGameState>> List { get; }
        public List<int> PathToCurrentNode { get; } = new();

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

            var node = new GameEventNode<TGameState>( gameEvent, PathToCurrentNode, serializer);
            current.Children.Add(node);

            List.Add(node);

            return node;
        }

        public void Pop()
        {
            PathToCurrentNode.RemoveAt(PathToCurrentNode.Count - 1);
        }
    }
}
