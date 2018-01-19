using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using EdgeFitting;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestFixture]
    public class SineTest
    {
        [Test]
        public void TestConstructor()
        {
            Sine sine = new Sine(100, 340, 23, 360);

            Assert.IsTrue(sine.Depth == 100, "Depth should be 100. It is " + sine.Depth);
            Assert.IsTrue(sine.Azimuth == 340, "Azimuth should be 340. It is " + sine.Azimuth);
            Assert.IsTrue(sine.Amplitude == 23, "Amplitude should be 23. It is " + sine.Amplitude);
            Assert.IsTrue(sine.Quality == 1, "Quality should be 1. It is " + sine.Quality);
        }

        [Test]
        public void TestConstructorWithQuality()
        {
            Sine sine = new Sine(100, 340, 23, 360, 3);

            Assert.IsTrue(sine.Depth == 100, "Depth should be 100. It is " + sine.Depth);
            Assert.IsTrue(sine.Azimuth == 340, "Azimuth should be 340. It is " + sine.Azimuth);
            Assert.IsTrue(sine.Amplitude == 23, "Amplitude should be 23. It is " + sine.Amplitude);
            Assert.IsTrue(sine.Quality == 3, "Quality should be 3. It is " + sine.Quality);
        }

        [Test]
        public void TestCalculatePoints()
        {
            int depth = 100;
            int azimuth = 270;
            int amplitude = 20;

            double sourceAzimuthResolution = 360;

            Sine sine = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);

            List<Point> points = sine.Points;

            double frequency = (Math.PI * 2.0) / sourceAzimuthResolution;
            int azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            int xPoint, yPoint, yEq;

            for (int i = 0; i < points.Count; i++)
            {
                xPoint = points[i].X;
                yPoint = points[i].Y;

                Assert.IsTrue(xPoint == i, "xPoint at " + i + " should be " + i + ". It is " + xPoint);

                yEq = (int)(amplitude * (Math.Sin((xPoint + azimuthDisplacement) * (frequency))) + depth);

                Assert.IsTrue(yPoint == yEq, "yPoint at " + i + " should be " + yEq + ". It is " + yPoint);
            }
        }

        [Test]
        public void TestChange()
        {
            int depth = 100;
            int azimuth = 270;
            int amplitude = 20;

            double sourceAzimuthResolution = 360;

            Sine sine = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);

            List<Point> points = sine.Points;

            double frequency = (Math.PI * 2.0) / sourceAzimuthResolution;
            int azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            int xPoint, yPoint, yEq;

            for (int i = 0; i < points.Count; i++)
            {
                xPoint = points[i].X;
                yPoint = points[i].Y;

                Assert.IsTrue(xPoint == i, "xPoint at " + i + " should be " + i + ". It is " + xPoint);

                yEq = (int)(amplitude * (Math.Sin((xPoint + azimuthDisplacement) * (frequency))) + depth);

                Assert.IsTrue(yPoint == yEq, "yPoint at " + i + " should be " + yEq + ". It is " + yPoint);
            }

            //Change
            depth = 110;
            azimuth = 230;
            amplitude = 11;

            sine.change(depth, azimuth, amplitude);
            points = sine.Points;

            azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            for (int i = 0; i < points.Count; i++)
            {
                xPoint = points[i].X;
                yPoint = points[i].Y;

                Assert.IsTrue(xPoint == i, "xPoint at " + i + " should be " + i + ". It is " + xPoint);

                yEq = (int)(amplitude * (Math.Sin((xPoint + azimuthDisplacement) * (frequency))) + depth);

                Assert.IsTrue(yPoint == yEq, "yPoint at " + i + " should be " + yEq + ". It is " + yPoint);
            }
        }

        [Test]
        public void TestGetY()
        {
            int depth = 456;
            int azimuth = 12;
            int amplitude = 23;

            double sourceAzimuthResolution = 360;

            Sine sine = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);

            double frequency = (Math.PI * 2.0) / sourceAzimuthResolution;
            int azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            int yEq;

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                yEq = (int)(amplitude * (Math.Sin((i + azimuthDisplacement) * (frequency))) + depth);

                Assert.IsTrue(sine.GetY(i) == yEq, "y value at " + i + " should be " + yEq + ". It is " + sine.GetY(i));
            }
        }

        [Test]
        public void TestGetSinePoint()
        {
            int depth = 105;
            int azimuth = 127;
            int amplitude = 4;

            double sourceAzimuthResolution = 360;

            Sine sine = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);

            double frequency = (Math.PI * 2.0) / sourceAzimuthResolution;
            int azimuthDisplacement = (int)(((double)sourceAzimuthResolution * (double)0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / (double)360)));

            int yEq;

            for (int i = 0; i < sourceAzimuthResolution; i++)
            {
                yEq = (int)(amplitude * (Math.Sin((i + azimuthDisplacement) * (frequency))) + depth);

                Assert.IsTrue(sine.GetPoint(i).X == i, "sinePoint[" + i + "]'s x value should be " + i + ". It is " + sine.GetPoint(i).X);
                Assert.IsTrue(sine.GetPoint(i).Y == yEq, "sinePoint[" + i + "]'s y value should be " + yEq + ". It is " + sine.GetPoint(i).Y);
            }
        }

        [Test]
        public void TestSourceAzimuthResolution()
        {
            int depth = 105;
            int azimuth = 127;
            int amplitude = 4;

            double sourceAzimuthResolution = 360;

            Sine sine = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);
            Assert.IsTrue(sine.Points.Count == sourceAzimuthResolution, "There should be 360 points. There are " + sine.Points.Count);

            sourceAzimuthResolution = 720;
            Sine sine2 = new Sine(depth, azimuth, amplitude, (int)sourceAzimuthResolution);
            Assert.IsTrue(sine2.Points.Count == sourceAzimuthResolution, "There should be 720 points. There are " + sine2.Points.Count);

        }
    }
}
