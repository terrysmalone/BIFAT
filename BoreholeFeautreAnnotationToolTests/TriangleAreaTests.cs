using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class TriangleAreaTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        public void TestTriangleArea()
        {
            Point point1 = new Point(5, 5);
            Point point2 = new Point(35, 5);

            int point1Direction = 45;
            int point2Direction = 135;

            TriangleArea triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetArea() == 225, "The area of the triangle should be 225. It is " + triangleArea.GetArea());
        }

        [TestMethod]
        public void TestVerticalLineTriangleArea()
        {
            Point point1 = new Point(5, 5);
            Point point2 = new Point(35, 5);

            int point1Direction = 45;
            int point2Direction = 135;

            TriangleArea triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetArea() == 225, "The area of the triangle should be 225. It is " + triangleArea.GetArea());
        }

        [TestMethod]
        public void TestHorizontalLineTriangleArea()
        {
            Point point1 = new Point(15, 20);
            Point point2 = new Point(45, 10);

            int point1Direction = 0;
            int point2Direction = 135;

            TriangleArea triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetArea() == 200, "The area of the triangle should be 200. It is " + triangleArea.GetArea());
        }

        [TestMethod]
        public void TestTriangleNotPossible()
        {
            Point point1 = new Point(15, 20);
            Point point2 = new Point(45, 10);

            int point1Direction = 45;
            int point2Direction = 225;

            TriangleArea triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetTriangleNotPossible() == true, "Triangle should not be possible.");
        }

        [TestMethod]
        public void TestTriangleNotPossibleWithMatchingSlopes()
        {
            Point point1 = new Point(15, 20);
            Point point2 = new Point(45, 10);

            int point1Direction = 45;
            int point2Direction = 45;

            TriangleArea triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetTriangleNotPossible() == true, "Triangle should not be possible.");
        }       
    }
}

