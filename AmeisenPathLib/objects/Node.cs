namespace AmeisenPathLib.objects
{
    public class Node
    {
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }
        public NodePosition Position { get; private set; }
        public bool IsBlocked { get; private set; }

        public Node ParentPathNode { get; set; }

        public Node(NodePosition position, bool isBlocked)
        {
            Position = position;
            IsBlocked = isBlocked;
        }
    }
}
