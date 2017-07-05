using System.Drawing;
using Edges;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    internal sealed class TriangleAreaTest
    {
        public double Tolerance = 0.00000000000000001;

        [TestCase(15, 20, 0, 45, 10, 135, 200)]     //Vertical
        [TestCase(5, 5, 45, 35, 5, 135, 225)]       //Horizontal
        public void TestTriangleArea(int x1, int y1, int direction1, 
                                     int x2, int y2, int direction2,
                                     int expectedArea)
        {
            var point1 = new Point(x1, y1);
            var point2 = new Point(x2, y2);

            var point1Direction = direction1;
            var point2Direction = direction2;

            var triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.That(triangleArea.GetArea(), Is.EqualTo(expectedArea));
        }

        [TestCase(15, 20, 45, 45, 10, 225)]
        [TestCase(15, 20, 45, 45, 10, 45)]  //Matching slopes
        public void TestTriangleNotPossible(int x1, int y1, int direction1, 
                                            int x2, int y2, int direction2)
        {
            var point1 = new Point(x1, y1);
            var point2 = new Point(x2, y2);

            var point1Direction = direction1;
            var point2Direction = direction2;

            var triangleArea = new TriangleArea(point1, point1Direction, point2, point2Direction);
            triangleArea.CalculateArea();

            Assert.IsTrue(triangleArea.GetTriangleNotPossible());
        }   
    }
}

