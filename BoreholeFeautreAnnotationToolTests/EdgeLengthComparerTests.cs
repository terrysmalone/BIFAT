using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edges;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestFixture]
    public class EdgesLengthComparerTest
    {
        [Test]
        public void TestEdgesLengthComparer()
        {
            Edge biggestEdge = new Edge(360);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);
            biggestEdge.AddPoint(1, 1);

            Edge secondBiggestEdge = new Edge(360);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);
            secondBiggestEdge.AddPoint(2, 2);

            Edge thirdBiggestEdge = new Edge(360);
            thirdBiggestEdge.AddPoint(3, 3);
            thirdBiggestEdge.AddPoint(3, 3);
            thirdBiggestEdge.AddPoint(3, 3);
            thirdBiggestEdge.AddPoint(3, 3);
            thirdBiggestEdge.AddPoint(3, 3);

            Edge fourthBiggestEdge = new Edge(360);
            fourthBiggestEdge.AddPoint(4, 4);
            fourthBiggestEdge.AddPoint(4, 4);
            fourthBiggestEdge.AddPoint(4, 4);

            List<Edge> edges = new List<Edge>();
            edges.Add(thirdBiggestEdge);
            edges.Add(biggestEdge);
            edges.Add(fourthBiggestEdge);
            edges.Add(secondBiggestEdge);

            Assert.IsTrue(edges[0].Equals(thirdBiggestEdge), "edges[0] should be the thirdBiggestEdge");
            Assert.IsTrue(edges[1].Equals(biggestEdge), "edges[1] should be the biggestEdge");
            Assert.IsTrue(edges[2].Equals(fourthBiggestEdge), "edges[2] should be the fourthBiggestEdge");
            Assert.IsTrue(edges[3].Equals(secondBiggestEdge), "edges[3] should be the secondBiggestEdge");

            edges.Sort(new EdgesLengthComparer());

            Assert.IsTrue(edges[0].Equals(biggestEdge), "edges[0] should be the biggestEdge");
            Assert.IsTrue(edges[1].Equals(secondBiggestEdge), "edges[1] should be the secondBiggestEdge");
            Assert.IsTrue(edges[2].Equals(thirdBiggestEdge), "edges[2] should be the thirdBiggestEdge.");
            Assert.IsTrue(edges[3].Equals(fourthBiggestEdge), "edges[3] should be the fourthBiggestEdge");

        }
    }
}