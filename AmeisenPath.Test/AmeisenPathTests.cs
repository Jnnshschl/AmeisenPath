using Microsoft.VisualStudio.TestTools.UnitTesting;
using AmeisenPathCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmeisenPathCore.Objects;

namespace AmeisenPathCore.Tests
{
    [TestClass()]
    public class AmeisenPathTests
    {
        [TestMethod()]
        public void FindPathAStarTest()
        {
            Assert.IsTrue(DoAStar2DTest());
            Assert.IsTrue(DoAStar2DTestWithHeight());
        }

        private bool DoAStar2DTest()
        {
            // Sample Map looking like this
            // _ # # #
            // _ _ # #
            // # _ _ #
            // # # _ _
            Node[,] sampleMap = {
                {
                    new Node(new NodePosition(0,0),false),
                    new Node(new NodePosition(0,1),false),
                    new Node(new NodePosition(0,2),true),
                    new Node(new NodePosition(0,3),true)
                },
                {
                    new Node(new NodePosition(1,0),true),
                    new Node(new NodePosition(1,1),false),
                    new Node(new NodePosition(1,2),false),
                    new Node(new NodePosition(1,3),true)
                },
                {
                    new Node(new NodePosition(2,0),true),
                    new Node(new NodePosition(2,1),true),
                    new Node(new NodePosition(2,2),false),
                    new Node(new NodePosition(2,3),false)
                },
                {
                    new Node(new NodePosition(3,0),true),
                    new Node(new NodePosition(3,1),true),
                    new Node(new NodePosition(3,2),false),
                    new Node(new NodePosition(3,3),false)
                }
            };

            List<Node> expectedPath = new List<Node>()
            {
                new Node(new NodePosition(1,1),false),
                new Node(new NodePosition(2,2),false),
                new Node(new NodePosition(3,3),false)
            };

            List<Node> calculatedPath =
                AmeisenPath.FindPathAStar(
                    sampleMap,
                    new NodePosition(0, 0),
                    new NodePosition(3, 3));

            return ComparePath(expectedPath, calculatedPath);
        }

        private bool DoAStar2DTestWithHeight()
        {
            // Sample Map looking like this
            // _ # # #
            // _ _ # #
            // # _ _ #
            // # # _ _
            // Each node is 1 higher than the diagonal
            // neighbour, so that the path must fold around
            Node[,] sampleMap = {
                {
                    new Node(new NodePosition(0,0,0),false),
                    new Node(new NodePosition(0,1,1),false),
                    new Node(new NodePosition(0,2,0),true),
                    new Node(new NodePosition(0,3,0),true)
                },
                {
                    new Node(new NodePosition(1,0,0),true),
                    new Node(new NodePosition(1,1,2),false),
                    new Node(new NodePosition(1,2,3),false),
                    new Node(new NodePosition(1,3,0),true)
                },
                {
                    new Node(new NodePosition(2,0,0),true),
                    new Node(new NodePosition(2,1,0),true),
                    new Node(new NodePosition(2,2,4),false),
                    new Node(new NodePosition(2,3,5),false)
                },
                {
                    new Node(new NodePosition(3,0,0),true),
                    new Node(new NodePosition(3,1,0),true),
                    new Node(new NodePosition(3,2,6),false),
                    new Node(new NodePosition(3,3,7),false)
                }
            };

            List<Node> expectedPath = new List<Node>()
            {
                new Node(new NodePosition(0,1,1),false),
                new Node(new NodePosition(1,1,2),false),
                new Node(new NodePosition(1,2,3),false),
                new Node(new NodePosition(2,2,4),false),
                new Node(new NodePosition(2,3,5),false),
                new Node(new NodePosition(3,2,6),false),
                new Node(new NodePosition(3,3,7),false)
            };

            List<Node> calculatedPath =
                AmeisenPath.FindPathAStar(
                    sampleMap,
                    new NodePosition(0, 0, 0),
                    new NodePosition(3, 3, 7),
                    true,
                    1.5);

            return ComparePath(expectedPath, calculatedPath, true);
        }

        /// <summary>
        /// Compares the paths by the coordinates
        /// </summary>
        /// <param name="expectedPath"></param>
        /// <param name="calculatedPath"></param>
        /// <returns></returns>
        private bool ComparePath(List<Node> expectedPath, List<Node> calculatedPath, bool hasHeight = false)
        {
            for (int i = 0; i < calculatedPath.Count; i++)
            {
                if (calculatedPath[i].Position.X
                    != expectedPath[i].Position.X)
                    return false;
                if (calculatedPath[i].Position.Y
                    != expectedPath[i].Position.Y)
                    return false;
                if (hasHeight)
                    if (calculatedPath[i].Position.Z
                        != expectedPath[i].Position.Z)
                        return false;
            }

            return calculatedPath.Count > 0;
        }

        [TestMethod()]
        public void CalculateCostTest()
        {
            Assert.AreEqual(10,
                AmeisenPath.CalculateCost(
                    new NodePosition(0, 0),
                    new NodePosition(0, 1)));
            Assert.AreEqual(14,
                AmeisenPath.CalculateCost(
                    new NodePosition(0, 0),
                    new NodePosition(1, 1)));
        }

        [TestMethod()]
        public void GetNeighboursTest()
        {
            Node[,] sampleMap = {
                {
                    new Node(new NodePosition(0,0),false),
                    new Node(new NodePosition(0,1),false),
                    new Node(new NodePosition(0,2),false)
                },
                {
                    new Node(new NodePosition(1,0),false),
                    new Node(new NodePosition(1,1),false),
                    new Node(new NodePosition(1,2),false)
                },
                {
                    new Node(new NodePosition(2,0),false),
                    new Node(new NodePosition(2,1),false),
                    new Node(new NodePosition(2,2),false)
                }
            };

            Assert.AreEqual(9, AmeisenPath.GetNeighbours(sampleMap, new NodePosition(1, 1)).Count);
        }
    }
}