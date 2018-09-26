namespace AmeisenPathCore.Objects
{
    public class NodePosition
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public NodePosition(int x, int y, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}