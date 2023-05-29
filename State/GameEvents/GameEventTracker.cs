using System.Collections.Generic;
using CodeName.EventSystem.State.Serialization;

namespace CodeName.EventSystem.State.GameEvents
{
    public class GameEventTracker
    {
        private readonly GameStateSerializer serializer;

        public GameEventTracker(GameStateSerializer serializer)
        {
            this.serializer = serializer;

            var root = new GameEventNode(EntityId.InvalidId, new TrackerRootEvent(), PathToCurrentNode, serializer);

            Tree = root;
            List = new List<GameEventNode>
            {
                root,
            };
        }

        public GameEventNode Tree { get; }
        public List<GameEventNode> List { get; }
        public List<int> PathToCurrentNode { get; } = new();

        public GameEventNode CurrentNode
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

        public GameEventNode Push(GameState gameState, GameEvent gameEvent)
        {
            var current = CurrentNode;
            var index = current.Children.Count;

            // Update path before passing to GameEventNode's constructor
            PathToCurrentNode.Add(index);

            var node = new GameEventNode(gameState.GenerateId(), gameEvent, PathToCurrentNode, serializer);
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
