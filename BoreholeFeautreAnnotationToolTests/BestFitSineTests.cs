using System;
using System.Collections.Generic;
using System.Drawing;
using EdgeFitting;
using Edges;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestFixture]
    public class BestFitTest
    {
        [Test]
        public void TestFullBestFit()
        {
            int DEPTH = 86;
            int AMPLITUDE = 24;
            int AZIMUTH = 215;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestFullBestFit1()
        {
            int DEPTH = 20;
            int AMPLITUDE = 2;
            int AZIMUTH = 300;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestFullBestFit2()
        {
            int DEPTH = 86;
            int AMPLITUDE = 6;
            int AZIMUTH = 109;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestFullBestFit3()
        {
            int DEPTH = 36;
            int AMPLITUDE = 10;
            int AZIMUTH = 109;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestFullBestFit4()
        {
            int DEPTH = 400;
            int AMPLITUDE = 12;
            int AZIMUTH = 350;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestFullBestFit5()
        {
            int DEPTH = 154;
            int AMPLITUDE = 32;
            int AZIMUTH = 100;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 2 && sine.Depth< DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 2 && sine.Amplitude< AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestPartialBestFit()
        {
            int DEPTH = 100;
            int AMPLITUDE = 13;
            int AZIMUTH = 340;

            Edge edge1 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 300);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 5 && sine.Depth< DEPTH + 5, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 5 && sine.Amplitude< AMPLITUDE + 5, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestPartialBestFit2()
        {
            int DEPTH = 54;
            int AMPLITUDE = 5;
            int AZIMUTH = 210;

            Edge edge1 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 300);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 5 && sine.Depth< DEPTH + 5, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 5 && sine.Amplitude< AMPLITUDE + 5, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestPartialBestFit3()
        {
            int DEPTH = 143;
            int AMPLITUDE = 65;
            int AZIMUTH = 260;

            Edge edge1 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 300);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 5 && sine.Depth< DEPTH + 5, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 5 && sine.Amplitude< AMPLITUDE + 5, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestPartialBestFit4()
        {
            int DEPTH = 500;
            int AMPLITUDE = 19;
            int AZIMUTH = 43;

            Edge edge1 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 1000);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Depth> DEPTH - 5 && sine.Depth< DEPTH + 5, "Depth should be " + DEPTH + ". It is " + sine.Depth);
            Assert.IsTrue(sine.Amplitude> AMPLITUDE - 5 && sine.Amplitude< AMPLITUDE + 5, "Amplitude should be " + AMPLITUDE + ". It is " + sine.Amplitude);
            Assert.IsTrue(sine.Azimuth > AZIMUTH - 5 && sine.Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sine.Azimuth);
        }

        [Test]
        public void TestQuality()
        {
            int DEPTH = 86;
            int AMPLITUDE = 24;
            int AZIMUTH = 215;

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);

            BestFitSine bestFit = new BestFitSine(edge1, 720, 500);
            bestFit.FindBestFit();

            Sine sine = bestFit.GetSine();

            Assert.IsTrue(sine.Quality == 4, "Quality should be " + 4 + ". It is " + sine.Quality);

            Edge edge2 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);
            BestFitSine bestFit2 = new BestFitSine(edge2, 720, 500);
            bestFit2.FindBestFit();

            Sine sine2 = bestFit2.GetSine();

            Assert.IsTrue(sine2.Quality == 2, "Quality should be " + 2 + ". It is " + sine2.Quality);
        }

        /// <summary>
        /// Creates an Edge with all points from the given sinusoid values
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="amplitude"></param>
        /// <param name="azimuth"></param>
        /// <param name="sourceImageWidth"></param>
        /// <returns></returns>
        private Edge createNewFullEdge(int depth, int amplitude, int azimuth, int sourceImageWidth)
        {
            Sine sine = new Sine(depth, azimuth, amplitude, sourceImageWidth);

            List<Point> points = sine.Points;

            Edge edge = new Edge(sourceImageWidth);

            for (int i = 0; i < points.Count; i++)
            {
                edge.AddPoint(points[i].X, points[i].Y);
            }

            return edge;
        }

        /// <summary>
        /// Creates an Edge with 400 points from the given sinusoid values
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="amplitude"></param>
        /// <param name="azimuth"></param>
        /// <param name="sourceImageWidth"></param>
        /// <returns></returns>
        private Edge createNewPartialEdge(int depth, int amplitude, int azimuth, int sourceImageWidth)
        {
            Sine sine = new Sine(depth, azimuth, amplitude, sourceImageWidth);

            List<Point> points = sine.Points;

            Edge edge = new Edge(sourceImageWidth);

            Random random = new Random();
            int position;

            for (int i = 0; i < 400; i++)
            {
                position = random.Next(points.Count);
                edge.AddPoint(points[position].X, points[position].Y);
            }

            return edge;
        }
    }
}

