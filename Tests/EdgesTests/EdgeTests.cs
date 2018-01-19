using System.Collections.Generic;
using System.Drawing;
using Edges;
using NUnit.Framework;

namespace EdgesTests
{
    [TestFixture]
    public class EdgeTests
    {
        [Test]
        public void TestConstructor()
        {
            Edge edge = new Edge(720);

            Assert.IsTrue(edge.LowestYPoint == 2147483647, "lowestPoint should be 2147483647");
            Assert.IsTrue(edge.HighestYPoint == 0, "highestPoint should be 0");
            Assert.IsTrue(edge.EdgeLength == 0, "Edge size should be 0");
        }

        [Test]
        public void TestAddPoint()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);

            Assert.IsTrue(testEdge.EdgeLength == 2, "Edge Length should be 2");
            Assert.IsTrue(testEdge.Points[0].X == 43, "First x point should be 43");
            Assert.IsTrue(testEdge.Points[0].Y == 20, "First y point should be 20");
            Assert.IsTrue(testEdge.Points[1].X == 45, "Second x point should be 45");
            Assert.IsTrue(testEdge.Points[1].Y == 54, "Second y point should be 54");
        }

        [Test]
        public void TestAddPoint2()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(new Point(43, 20));
            testEdge.AddPoint(new Point(45, 54));

            Assert.IsTrue(testEdge.EdgeLength == 2, "Edge Length should be 2");
            Assert.IsTrue(testEdge.Points[0].X == 43, "First x point should be 43");
            Assert.IsTrue(testEdge.Points[0].Y == 20, "First y point should be 20");
            Assert.IsTrue(testEdge.Points[1].X == 45, "Second x point should be 45");
            Assert.IsTrue(testEdge.Points[1].Y == 54, "Second y point should be 54");
        }

        [Test]
        public void TestRemovePoint()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(new Point(43, 20));
            testEdge.AddPoint(new Point(45, 54));
            testEdge.AddPoint(new Point(47, 54));
            testEdge.AddPoint(new Point(43, 10));

            Assert.IsTrue(testEdge.EdgeLength == 4, "Edge Length should be 4. It is " + testEdge.EdgeLength);

            testEdge.RemovePoint(new Point(43, 10));

            Assert.IsTrue(testEdge.EdgeLength == 3, "Edge Length should be 3. It is " + testEdge.EdgeLength);

            testEdge.RemovePoint(new Point(40, 10));

            Assert.IsTrue(testEdge.EdgeLength == 3, "Edge Length should be 3. It is " + testEdge.EdgeLength);

            testEdge.AddPoint(new Point(40, 10));

            Assert.IsTrue(testEdge.EdgeLength == 4, "Edge Length should be 4. It is " + testEdge.EdgeLength);

            testEdge.RemovePoint(new Point(43, 20));

            Assert.IsTrue(testEdge.EdgeLength == 3, "Edge Length should be 3. It is " + testEdge.EdgeLength);

            testEdge.RemovePoint(new Point(40, 10));

            Assert.IsTrue(testEdge.EdgeLength == 2, "Edge Length should be 2. It is " + testEdge.EdgeLength);
        }

        [Test]
        public void testAddEdge()
        {
            Edge testEdge = new Edge(100);
            Edge edgeToAdd = new Edge(100);

            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);

            Assert.IsTrue(testEdge.EdgeLength == 2, "Edge Length should be 2");
            Assert.IsTrue(testEdge.Points[0].X == 43, "First x point should be 43");
            Assert.IsTrue(testEdge.Points[0].Y == 20, "First y point should be 20");
            Assert.IsTrue(testEdge.Points[1].X == 45, "Second x point should be 45");
            Assert.IsTrue(testEdge.Points[1].Y == 54, "Second y point should be 54");

            edgeToAdd.AddPoint(20, 10);
            edgeToAdd.AddPoint(21, 12);

            Assert.IsTrue(edgeToAdd.EdgeLength == 2, "Edge Length should be 2");
            Assert.IsTrue(edgeToAdd.Points[0].X == 20, "First x point should be 20");
            Assert.IsTrue(edgeToAdd.Points[0].Y == 10, "First y point should be 10");
            Assert.IsTrue(edgeToAdd.Points[1].X == 21, "Second x point should be 21");
            Assert.IsTrue(edgeToAdd.Points[1].Y == 12, "Second y point should be 12");

            testEdge.AddEdge(edgeToAdd);

            Assert.IsTrue(testEdge.EdgeLength == 4, "Edge length should be 4");
            Assert.IsTrue(testEdge.Points[0].X == 43, "First x point should be 43");
            Assert.IsTrue(testEdge.Points[2].X == 20, "Third x point should be 20");
        }

        [Test]
        public void testPoints()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(20, 10);
            testEdge.AddPoint(21, 15);
            testEdge.AddPoint(22, 16);

            List<Point> testArrayList = testEdge.Points;

            Assert.IsTrue(testArrayList[0].Equals(new Point(20, 10)), "testArrayList 0");
            Assert.IsTrue(testArrayList[1].Equals(new Point(21, 15)), "testArrayList 1");
            Assert.IsTrue(testArrayList[2].Equals(new Point(22, 16)), "testArrayList 2");
        }

        [Test]
        public void testEdgeEnd()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(20, 10);
            testEdge.AddPoint(21, 15);
            testEdge.AddPoint(22, 16);
            testEdge.AddPoint(23, 16);
            testEdge.AddPoint(24, 16);
            testEdge.AddPoint(25, 17);

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(0, 0), "EdgeEnd1 should be (0, 0) but it is " + testEdge.EdgeEnd1);
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(0, 0), "EdgeEnd2 should be (0, 0) but it is " + testEdge.EdgeEnd2);

            testEdge.EdgeEnd1 = new Point(20, 10);
            testEdge.EdgeEnd2 = new Point(24, 16);

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(20, 10), "EdgeEnd1 should be (20, 10). It is " + testEdge.EdgeEnd1);
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(24, 16), "EdgeEnd2 should be (24, 16). It is " + testEdge.EdgeEnd2);

            testEdge.EdgeEnd1 = new Point(12, 12);
            testEdge.EdgeEnd2 = new Point(240, 160);

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(20, 10), "EdgeEnd1 should be (20, 10). It is " + testEdge.EdgeEnd1);
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(24, 16), "EdgeEnd2 should be (24, 16). It is " + testEdge.EdgeEnd2);

            testEdge.EdgeEnd1 = new Point(25, 17);
            testEdge.EdgeEnd2 = new Point(20, 10);

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(25, 17), "EdgeEnd1 should be (25, 17). It is " + testEdge.EdgeEnd1);
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(20, 10), "EdgeEnd2 should be (20, 10). It is " + testEdge.EdgeEnd2);
        }

        [Test]
        public void testEdgeEndInt()
        {
            Edge testEdge = new Edge(100);
            testEdge.AddPoint(20, 10);
            testEdge.AddPoint(21, 15);
            testEdge.AddPoint(22, 16);
            testEdge.AddPoint(23, 16);
            testEdge.AddPoint(24, 16);
            testEdge.AddPoint(25, 17);

            testEdge.EdgeEnd1 = new Point(20, 10);
            testEdge.EdgeEnd2 = new Point(25, 17);

            Assert.IsTrue(testEdge.GetEdgeEnd(1) == new Point(20, 10), "EdgeEnd1 should be (20, 10). It is " + testEdge.GetEdgeEnd(1));
            Assert.IsTrue(testEdge.GetEdgeEnd(2) == new Point(25, 17), "EdgeEnd2 should be (25, 17). It is " + testEdge.GetEdgeEnd(2));

            testEdge.EdgeEnd1 = new Point(21, 15);
            testEdge.EdgeEnd2 = new Point(24, 16);

            Assert.IsTrue(testEdge.GetEdgeEnd(1) == new Point(21, 15), "EdgeEnd1 should be (21, 15). It is " + testEdge.GetEdgeEnd(1));
            Assert.IsTrue(testEdge.GetEdgeEnd(2) == new Point(24, 16), "EdgeEnd2 should be (24, 16). It is " + testEdge.GetEdgeEnd(2));
        }

        [Test]
        public void TestLowestXPoint()
        {
            Edge testEdge = new Edge(100);
            Edge edgeToAdd = new Edge(100);

            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);
            testEdge.AddPoint(54, 59);


            edgeToAdd.AddPoint(20, 10);
            edgeToAdd.AddPoint(21, 25);
            edgeToAdd.AddPoint(25, 5);

            Assert.IsTrue(testEdge.LowestXPoint == 43, "testEdge lowestXPoint should be 43");
            Assert.IsTrue(edgeToAdd.LowestXPoint == 20, "edgeToAdd lowestXPoint should be 20");
        }

        [Test]
        public void testHighestXPoint()
        {
            Edge testEdge = new Edge(100);
            Edge edgeToAdd = new Edge(100);

            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);
            testEdge.AddPoint(54, 59);


            edgeToAdd.AddPoint(20, 10);
            edgeToAdd.AddPoint(21, 25);
            edgeToAdd.AddPoint(25, 5);

            Assert.IsTrue(testEdge.HighestXPoint == 54, "testEdge highestXPoint should be 54");
            Assert.IsTrue(edgeToAdd.HighestXPoint == 25, "edgeToAdd highestXPoint should be 25");
        }

        [Test]
        public void testLowestYPoint()
        {
            Edge testEdge = new Edge(100);
            Edge edgeToAdd = new Edge(100);

            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);
            testEdge.AddPoint(54, 59);


            edgeToAdd.AddPoint(20, 10);

            Assert.IsTrue(edgeToAdd.LowestYPoint == 10, "edgeToAdd lowestPoint should be 10. It is " + edgeToAdd.LowestYPoint);

            edgeToAdd.AddPoint(21, 25);
            edgeToAdd.AddPoint(25, 5);

            Assert.IsTrue(testEdge.LowestYPoint == 20, "testEdge lowestPoint should be 20");
            Assert.IsTrue(edgeToAdd.LowestYPoint == 5, "edgeToAdd lowestPoint should be 5");
        }

        [Test]
        public void testHighestYPoint()
        {
            Edge testEdge = new Edge(100);
            Edge edgeToAdd = new Edge(100);

            testEdge.AddPoint(43, 20);
            testEdge.AddPoint(45, 54);

            Assert.IsTrue(testEdge.HighestYPoint == 54, "testEdge highestPoint should be 54. It is " + testEdge.HighestYPoint);

            testEdge.AddPoint(54, 59);


            edgeToAdd.AddPoint(20, 10);
            edgeToAdd.AddPoint(21, 25);
            edgeToAdd.AddPoint(25, 5);

            Assert.IsTrue(testEdge.HighestYPoint == 59, "testEdge highestPoint should be 59");
            Assert.IsTrue(edgeToAdd.HighestYPoint == 25, "edgeToAdd highestPoint should be 25");
        }

        [Test]
        public void testSortEdge()
        {
            Edge testEdge = new Edge(100);


            testEdge.AddPoint(4, 10);   //3
            testEdge.AddPoint(5, 10);   //4
            testEdge.AddPoint(2, 10);   //1
            testEdge.AddPoint(1, 10);   //0
            testEdge.AddPoint(5, 9);    //5
            testEdge.AddPoint(3, 10);   //2
            testEdge.AddPoint(6, 9);    //6
            testEdge.AddPoint(8, 9);    //9
            testEdge.AddPoint(9, 10);    //11
            testEdge.AddPoint(8, 10);   //10
            testEdge.AddPoint(7, 8);    //7
            testEdge.AddPoint(8, 8);    //8


            testEdge.SortEdge();

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(1, 10), "Edge end 1 should be (1,10).  It is (" + testEdge.EdgeEnd1.X + "," + testEdge.EdgeEnd1.Y + ")");
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(9, 10), "Edge end 2 should be (9,10).  It is (" + testEdge.EdgeEnd2.X + "," + testEdge.EdgeEnd2.Y + ")");

            List<Point> points = testEdge.Points;

            Assert.IsTrue(points[0] == new Point(1, 10), "points[0] should be (1,10).  It is " + points[0].X + "," + points[0].Y + ")");
            Assert.IsTrue(points[1] == new Point(2, 10), "points[1] should be (2,10).  It is " + points[1].X + "," + points[1].Y + ")");
            Assert.IsTrue(points[2] == new Point(3, 10), "points[2] should be (3,10).  It is " + points[2].X + "," + points[2].Y + ")");
            Assert.IsTrue(points[3] == new Point(4, 10), "points[3] should be (4,10).  It is " + points[3].X + "," + points[3].Y + ")");
            Assert.IsTrue(points[4] == new Point(5, 10), "points[4] should be (5,10).  It is " + points[4].X + "," + points[4].Y + ")");
            Assert.IsTrue(points[5] == new Point(5, 9), "points[5] should be (5,9).  It is " + points[5].X + "," + points[5].Y + ")");
            Assert.IsTrue(points[6] == new Point(6, 9), "points[6] should be (6,9).  It is " + points[6].X + "," + points[6].Y + ")");
            Assert.IsTrue(points[7] == new Point(7, 8), "points[7] should be (7,8).  It is " + points[7].X + "," + points[7].Y + ")");
            Assert.IsTrue(points[8] == new Point(8, 8), "points[8] should be (8,8).  It is " + points[8].X + "," + points[8].Y + ")");
            Assert.IsTrue(points[9] == new Point(8, 9), "points[9] should be (8,9).  It is " + points[9].X + "," + points[9].Y + ")");
            Assert.IsTrue(points[10] == new Point(8, 10), "points[10] should be (8,10).  It is " + points[10].X + "," + points[10].Y + ")");

            Assert.IsTrue(points[11] == new Point(9, 10), "points[11] should be (9,10).  It is " + points[11].X + "," + points[11].Y + ")");
        }

        [Test]
        public void testSortEdge2()
        {
            Edge testEdge = new Edge(100);


            testEdge.AddPoint(9, 7);
            testEdge.AddPoint(10, 4);
            testEdge.AddPoint(10, 5);
            testEdge.AddPoint(10, 6);
            testEdge.AddPoint(11, 3);

            testEdge.SortEdge();

            Assert.IsTrue(testEdge.EdgeEnd1 == new Point(9, 7), "Edge end 1 should be (9,7).  It is (" + testEdge.EdgeEnd1.X + "," + testEdge.EdgeEnd1.Y + ")");
            Assert.IsTrue(testEdge.EdgeEnd2 == new Point(11, 3), "Edge end 2 should be (11,3).  It is (" + testEdge.EdgeEnd2.X + "," + testEdge.EdgeEnd2.Y + ")");

            List<Point> points = testEdge.Points;

            Assert.IsTrue(points[0] == new Point(9, 7), "points[0] should be (9,7).  It is " + points[0].X + "," + points[0].Y + ")");
            Assert.IsTrue(points[1] == new Point(10, 6), "points[1] should be (10,6).  It is " + points[1].X + "," + points[1].Y + ")");
            Assert.IsTrue(points[2] == new Point(10, 5), "points[2] should be (10,5).  It is " + points[2].X + "," + points[2].Y + ")");
            Assert.IsTrue(points[3] == new Point(10, 4), "points[3] should be (10,4).  It is " + points[3].X + "," + points[3].Y + ")");
            Assert.IsTrue(points[4] == new Point(11, 3), "points[4] should be (11,3).  It is " + points[4].X + "," + points[4].Y + ")");

        }
    }
}

