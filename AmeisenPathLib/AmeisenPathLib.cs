using AmeisenPathLib.objects;
using System;
using System.Collections.Generic;

namespace AmeisenPathLib
{
    public class AmeisenPath
    {
        /// <summary>
        /// Calculate a path using the A* algorithm
        /// </summary>
        /// <param name="map">the map that conatin our start and end points</param>
        /// <param name="startPosition">starting position</param>
        /// <param name="endPosition">end position</param>
        /// <returns>a list of nodes to walk to reach the end position</returns>
        public static List<Node> FindPathAStar(Node[,] map, NodePosition startPosition, NodePosition endPosition)
        {
            Heap<Node> openNodes = new Heap<Node>(map.GetLength(0) * map.GetLength(1));
            HashSet<Node> closedNodes = new HashSet<Node>();

            openNodes.Add(map[startPosition.X, startPosition.Y]);

            // Aslong as there are nodes to check, go for it
            while (openNodes.Count > 0)
            {
                Node activeNode = openNodes.RemoveFirst();
                closedNodes.Add(activeNode);

                // Are we at the endNode
                if (activeNode.Position.X == endPosition.X && activeNode.Position.Y == endPosition.Y)
                    return GeneratePath(map, startPosition, endPosition);

                // Check the neighbour nodes
                foreach (Node neighbourNode in GetNeighbours(map, activeNode.Position))
                {
                    // if its blocked or closed, skip it
                    if (neighbourNode.IsBlocked || closedNodes.Contains(neighbourNode))
                        continue;

                    // calculate new cost to go to the neighbour node and add it to the openNodes
                    // list if its not already in there
                    int newCostToNeighbour = activeNode.GCost + CalculateCost(activeNode.Position, neighbourNode.Position);
                    if (newCostToNeighbour < neighbourNode.GCost || !openNodes.Contains(neighbourNode))
                    {
                        neighbourNode.GCost = newCostToNeighbour;
                        neighbourNode.HCost = CalculateCost(neighbourNode.Position, endPosition);
                        neighbourNode.ParentPathNode = activeNode;

                        if (!openNodes.Contains(neighbourNode))
                            openNodes.Add(neighbourNode);
                        else
                            openNodes.UpdateItem(neighbourNode);
                    }
                }
            }
            // No path found
            return null;
        }

        public static List<Node> GetNeighbours(Node[,] map, NodePosition currentPosition)
        {
            List<Node> neighbours = new List<Node>();

            // Calculate the boundaries where the hood is in :^)
            int xStart = currentPosition.X > 0 ? currentPosition.X - 1 : currentPosition.X;
            int xEnd = currentPosition.X < map.GetLength(0) - 1 ? currentPosition.X + 1 : currentPosition.X;

            int yStart = currentPosition.Y > 0 ? currentPosition.Y - 1 : currentPosition.Y;
            int yEnd = currentPosition.Y < map.GetLength(1) - 1 ? currentPosition.Y + 1 : currentPosition.Y;

            for (int x = xStart; x <= xEnd; x++)
                for (int y = yStart; y <= yEnd; y++)
                    neighbours.Add(map[x, y]);

            return neighbours;
        }

        private static List<Node> GeneratePath(Node[,] map, NodePosition startPosition, NodePosition endPosition)
        {
            List<Node> pathTogGo = new List<Node>();

            Node currentNode = map[endPosition.X, endPosition.Y];
            Node targetNode = map[startPosition.X, startPosition.Y];

            while (currentNode != targetNode)
            {
                pathTogGo.Add(currentNode);
                currentNode = currentNode.ParentPathNode;
            }

            pathTogGo.Reverse();

            return pathTogGo;
        }

        private static int CalculateCost(NodePosition nodeA, NodePosition nodeB)
        {
            int distanceX = Math.Abs(nodeA.X - nodeB.X);
            int distanceY = Math.Abs(nodeA.Y - nodeB.Y);

            // 10 for a move 14 for a diagonal move
            if (distanceX > distanceY)
                return 14 * distanceY + 10 * (distanceX - distanceY);
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}