using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using EdgeFitting;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class SineFitTests
    {
        /// <summary>
        /// Tests sinefitting when all points are present and accurate
        /// </summary>
        [TestMethod]
        public void TestFullSineFitting()
        {
            int DEPTH = 86;
            int AMPLITUDE = 24;
            int AZIMUTH = 215;

            List<Edge> edges = new List<Edge>();
            List<Sine> sines = new List<Sine>();

            Edge edge1 = createNewFullEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);
            edges.Add(edge1);

            EdgeFit sineFit = new EdgeFit(edges, 720, 300);
            sineFit.MaxAmplitude = 30;

            sineFit.FitEdges();
            sines = sineFit.FitSines;
            Assert.IsTrue(sines.Count == 1, "Count should be 1. It is " + sines.Count);

            Assert.IsTrue(sines[0].Depth > DEPTH - 2 && sines[0].Depth < DEPTH + 2, "Depth should be " + DEPTH + ". It is " + sines[0].Depth);
            Assert.IsTrue(sines[0].Azimuth > AZIMUTH - 5 && sines[0].Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sines[0].Azimuth);
            Assert.IsTrue(sines[0].Amplitude > AMPLITUDE - 2 && sines[0].Amplitude < AMPLITUDE + 2, "Amplitude should be " + AMPLITUDE + ". It is " + sines[0].Amplitude);
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
        /// Tests sinefitting when some points are present and accurate
        /// </summary>
        [TestMethod]
        public void TestPartialSineFitting()
        {
            int DEPTH = 106;
            int AMPLITUDE = 15;
            int AZIMUTH = 320;

            List<Edge> edges = new List<Edge>();
            List<Sine> sines = new List<Sine>();

            Edge edge1 = createNewPartialEdge(DEPTH, AMPLITUDE, AZIMUTH, 720);
            edges.Add(edge1);

            EdgeFit sineFit = new EdgeFit(edges, 720, 300);
            sineFit.MaxAmplitude = 30;

            sineFit.FitEdges();

            sines = sineFit.FitSines;

            Assert.IsTrue(sines[0].Depth > DEPTH - 5 && sines[0].Depth < DEPTH + 5, "Depth should be " + DEPTH + ". It is " + sines[0].Depth);
            Assert.IsTrue(sines[0].Amplitude > AMPLITUDE - 5 && sines[0].Amplitude < AMPLITUDE + 5, "Amplitude should be " + AMPLITUDE + ". It is " + sines[0].Amplitude);
            Assert.IsTrue(sines[0].Azimuth > AZIMUTH - 5 && sines[0].Azimuth < AZIMUTH + 5, "Azimuth should be " + AZIMUTH + ". It is " + sines[0].Azimuth);
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
