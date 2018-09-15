using AmeisenPathLib.interfaces;

namespace AmeisenPathLib.objects
{
    public class Node : IHeapItem<Node>
    {
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get { return GCost + HCost; } }
        public NodePosition Position { get; set; }
        public bool IsBlocked { get; set; }

        public Node ParentPathNode { get; set; }
        public int HeapIndex { get; set; }

        public Node(NodePosition position, bool isBlocked)
        {
            Position = position;
            IsBlocked = isBlocked;
        }

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
                compare = HCost.CompareTo(other.HCost);
            return -compare;
        }
    }
}