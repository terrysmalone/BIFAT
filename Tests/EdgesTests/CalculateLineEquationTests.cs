using System;
using System.Drawing;
using Edges;
using NUnit.Framework;

namespace EdgesTests
{
    [TestFixture]
    public class CalculateLineEquationTest
    {
        [Test]
        public void TestCalculateSlope0()
        {
            Point point = new Point(1, 4);
            int direction = 0;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(calc.Slope);

            Assert.IsTrue(slope.CompareTo((decimal)0.0) == 0, "slope of 0 should be 0. It is " + slope);
        }

        [Test]
        public void TestCalculateSlope180()
        {
            Point point = new Point(1, 4);
            int direction = 180;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(string.Format("{0:0.00}", calc.Slope));

            Assert.IsTrue(slope.CompareTo((decimal)0.0) == 0, "slope of 180 should be 0. It is " + slope);
        }

        [Test]
        public void TestCalculateSlope45()
        {
            Point point = new Point(1, 4);
            int direction = 45;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(calc.Slope);

            Assert.IsTrue(slope.CompareTo((decimal)-1.0) == 0, "slope of 45 should be -1. It is " + slope);
        }

        [Test]
        public void TestCalculateSlope225()
        {
            Point point = new Point(1, 4);
            int direction = 225;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(calc.Slope);

            Assert.IsTrue(slope.CompareTo((decimal)-1.0) == 0, "slope of 225 should be -1. It is " + slope);
        }

        [Test]
        public void TestCalculateSlope135()
        {
            Point point = new Point(1, 4);
            int direction = 135;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(calc.Slope);

            Assert.IsTrue(slope.CompareTo((decimal)1.0) == 0, "slope of 135 should be 1. It is " + slope);
        }

        [Test]
        public void TestCalculateSlope344()
        {
            Point point = new Point(1, 4);
            int direction = 344;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal slope = Convert.ToDecimal(string.Format("{0:0.00}", calc.Slope));

            Assert.IsTrue(slope.CompareTo((decimal)0.29) == 0, "slope of 135 should be 0.29. It is " + slope);
        }

        [Test]
        public void TestCalculateHorizontalIntercept()
        {
            Point point = new Point(1, 6);
            int direction = 180;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            double intercept = calc.Intercept;

            Assert.IsTrue(intercept == 6, "Intercept should be 6. It is " + intercept);
        }

        [Test]
        public void TestCalculateHorizontalIntercept2()
        {
            Point point = new Point(1, 4);
            int direction = 0;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            double intercept = calc.Intercept;

            Assert.IsTrue(intercept == 4, "Intercept should be 4. It is " + intercept);
        }

        [Test]
        public void TestCalculateSlopingIntercept()
        {
            Point point = new Point(1, 4);
            int direction = 135;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            double intercept = calc.Intercept;

            Assert.IsTrue(intercept == 3, "Intercept should be 3. It is " + intercept);
        }

        [Test]
        public void TestCalculateSlopingIntercept2()
        {
            Point point = new Point(2, 4);
            int direction = 225;

            CalculateLineEquation calc = new CalculateLineEquation(point, direction);

            decimal intercept = Convert.ToDecimal(string.Format("{0:0.00}", calc.Intercept));

            Assert.IsTrue(intercept.CompareTo((decimal)6.0) == 0, "Intercept should be 6. It is " + intercept);
        }
    }
}