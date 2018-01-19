using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edges;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestFixture]
    public class RemoveEdgesTests
    {
        [Test]
        public void TestRemoveEdges()
        {
            List<Edge> edges = new List<Edge>();

            Edge edge1 = new Edge(360);     //will be 300 long
            Edge edge2 = new Edge(360);     //will be 250 long
            Edge edge3 = new Edge(360);     //will be 50 long
            Edge edge4 = new Edge(360);     //will be 150 long

            for (int i = 0; i < 300; i++)
            {
                edge1.AddPoint(new Point(i, 20));

                if (i < 250)
                    edge2.AddPoint(new Point(i, 50));

                if (i < 50)
                    edge3.AddPoint(new Point(i, 40));

                if (i < 150)
                    edge4.AddPoint(new Point(i, 90));
            }

            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            edges.Add(edge4);

            RemoveEdges remove = new RemoveEdges(edges, 110);

            remove.removeEdges();

            Assert.IsTrue(remove.getEdges().Count == 3, "There should now be 3 edges. There are + " + remove.getEdges().Count);

            Assert.IsTrue(remove.getEdges()[0].Equals(edge1), "Edge[0] should be the same as edge1.");
            Assert.IsTrue(remove.getEdges()[1].Equals(edge2), "Edge[1] should be the same as edge2.");
            Assert.IsTrue(remove.getEdges()[2].Equals(edge4), "Edge[2] should be the same as edge4.");
        }

        [Test]
        public void TestMinimumLength()
        {
            List<Edge> edges = new List<Edge>();

            Edge edge1 = new Edge(360);     //will be 300 long
            Edge edge2 = new Edge(360);     //will be 250 long
            Edge edge3 = new Edge(360);     //will be 50 long
            Edge edge4 = new Edge(360);     //will be 150 long

            for (int i = 0; i < 300; i++)
            {
                edge1.AddPoint(new Point(i, 20));

                if (i < 250)
                    edge2.AddPoint(new Point(i, 50));

                if (i < 50)
                    edge3.AddPoint(new Point(i, 40));

                if (i < 150)
                    edge4.AddPoint(new Point(i, 90));
            }

            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            edges.Add(edge4);

            RemoveEdges remove = new RemoveEdges(edges, 110);

            remove.setMinimumLength(176);
            remove.removeEdges();

            Assert.IsTrue(remove.getEdges().Count == 2, "There should now be 2 edges. There are + " + remove.getEdges().Count);

            Assert.IsTrue(remove.getEdges()[0].Equals(edge1), "Edge[0] should be the same as edge1.");
            Assert.IsTrue(remove.getEdges()[1].Equals(edge2), "Edge[1] should be the same as edge2.");
        }
    }
}

