using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class CalculateIntersectingPointsTest
    {
        [TestMethod]
        public void TestCalculateIntersectingPoint1()
        {
            CalculateIntersectingPoint calcIntersection = new CalculateIntersectingPoint(3, 2, 2, -1);

            Assert.IsTrue(calcIntersection.GetIntersectingPoint() == new Point(-3, -7), "The intersecting point should be (-3,-7). It is " + calcIntersection.GetIntersectingPoint());
        }

        [TestMethod]
        public void TestCalculateIntersectingPoint2()
        {
            CalculateIntersectingPoint calcIntersection = new CalculateIntersectingPoint(3, -7, -2, 3);

            Assert.IsTrue(calcIntersection.GetIntersectingPoint() == new Point(2, -1), "The intersecting point should be (2, -1). It is " + calcIntersection.GetIntersectingPoint());
        }

        [TestMethod]
        public void TestNoIntersectingPoint()
        {
            CalculateIntersectingPoint calcIntersection = new CalculateIntersectingPoint(3, 5, 3, 4);

            Assert.IsTrue(calcIntersection.GetNoIntersectingPoint() == true, "There should be no intersecting point");
        }

        [TestMethod]
        public void TestMatchingInterceptPoint()
        {
            CalculateIntersectingPoint calcIntersection = new CalculateIntersectingPoint(4, 5, 3, 5);

            Assert.IsTrue(calcIntersection.GetIntersectingPoint() == new Point(0, 5), "The intersecting point should be (0, 5). It is " + calcIntersection.GetIntersectingPoint());
        }

    }
}