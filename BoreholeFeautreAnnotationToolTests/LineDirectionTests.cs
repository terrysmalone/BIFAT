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
    public class LineDirectionTests
    {
        [Test]
        public void TestAngledLine()
        {
            Point startPoint = new Point(10, 20);
            Point endPoint = new Point(15, 18);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 22, "Line direction should be 22. It is " + direction);
        }

        [Test]
        public void TestNorthEastDirection()
        {
            Point startNEPoint = new Point(10, 20);
            Point endNEPoint = new Point(14, 16);

            LineDirection NELineDirection = new LineDirection(startNEPoint, endNEPoint);
            NELineDirection.Calculate();

            int NEDirection = NELineDirection.GetDirection();

            Assert.IsTrue(NEDirection == 45, "NEDirection should be 45. It is " + NEDirection);
        }

         [Test]
        public void TestEastEdgeDirection()
        {
            Point startEastPoint = new Point(43, 111);
            Point endEastPoint = new Point(48, 111);

            LineDirection eastLineDirection = new LineDirection(startEastPoint, endEastPoint);
            eastLineDirection.Calculate();

            int eastDirection = eastLineDirection.GetDirection();

            Assert.IsTrue(eastDirection == 0, "eastDirection should be 0. It is " + eastDirection);
        }

         [Test]
        public void TestNorthEdgeDirection()
        {
            Point startPoint = new Point(43, 111);
            Point endPoint = new Point(43, 107);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 90, "North direction should be 90. It is " + direction);
        }

         [Test]
        public void TestNorthWestEdgeDirection()
        {
            Point startPoint = new Point(15, 25);
            Point endPoint = new Point(10, 20);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 135, "North west should be 135. It is " + direction);
        }

         [Test]
        public void TestWestEdgeDirection()
        {
            Point startPoint = new Point(48, 111);
            Point endPoint = new Point(43, 111);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 180, "West direction should be 180. It is " + direction);
        }

         [Test]
        public void TestSouthWestEdgeDirection()
        {
            Point startPoint = new Point(14, 16);
            Point endPoint = new Point(10, 20);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 225, "SouthWest direction should be 225. It is " + direction);
        }

         [Test]
        public void TestSouthEdgeDirection()
        {
            Point startPoint = new Point(43, 107);
            Point endPoint = new Point(43, 111);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 270, "South direction should be 270. It is " + direction);
        }

         [Test]
        public void TestSouthEastEdgeDirection()
        {
            Point startPoint = new Point(10, 20);
            Point endPoint = new Point(15, 25);

            LineDirection lineDirection = new LineDirection(startPoint, endPoint);
            lineDirection.Calculate();

            int direction = lineDirection.GetDirection();

            Assert.IsTrue(direction == 315, "South east should be 315. It is " + direction);
        }
    }
}
