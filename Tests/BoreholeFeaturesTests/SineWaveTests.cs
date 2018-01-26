using System;
using System.Collections.Generic;
using System.Drawing;
using BoreholeFeatures;
using NUnit.Framework;

namespace BoreholeFeaturesTests
{
    [TestFixture]
    public class SineWaveTests
    {
        SineWave sineWave;
        int azimuth = 140;
        int amplitude = 34;
        int depth = 303;
        int sourceAzimuthResolution = 720;

        [Test]
        public void TestProperties()
        {
            sineWave = new SineWave(depth, azimuth, amplitude, sourceAzimuthResolution);

            Assert.AreEqual(sineWave.getDepth(), depth, "Depth is not returning correct value");
            Assert.AreEqual(sineWave.getAzimuth(), azimuth, "Azimuth is not returning correct value");
            Assert.AreEqual(sineWave.getAmplitude(), amplitude, "Amplitude is not returning correct value");
        }

        [Test]
        public void TestGetSinePoint()
        {
            sineWave = new SineWave(depth, azimuth, amplitude, sourceAzimuthResolution);

            double azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / 360.0));
            double frequency = ((double)Math.PI * 2.0) / (double)sourceAzimuthResolution;

            int expectedYPoint = (int)((double)depth + ((double)Math.Sin((10.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(10).Y, "Ypoint at x=10 is wrong");

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((35.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(35).Y, "Ypoint at x=35 is wrong");

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((500.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(500).Y, "Ypoint at x=500 is wrong");
        }

        [Test]
        public void TestGetY()
        {
            sineWave = new SineWave(depth, azimuth, amplitude, sourceAzimuthResolution);

            double azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / 360.0));
            double frequency = ((double)Math.PI * 2.0) / (double)sourceAzimuthResolution;

            int expectedYPoint = (int)((double)depth + ((double)Math.Sin((10.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getY(10), "Ypoint at x=10 should be " + expectedYPoint + ". It is " + sineWave.getY(10));

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((35.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getY(35), "Ypoint at x=35 should be " + expectedYPoint + ". It is " + sineWave.getY(35));

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((500.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getY(500), "Ypoint at x=500 should be " + expectedYPoint + ". It is " + sineWave.getY(500));
        }

        [Test]
        public void TestGetSinePoints()
        {
            sineWave = new SineWave(depth, azimuth, amplitude, sourceAzimuthResolution);

            double azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / 360.0));
            double frequency = ((double)Math.PI * 2.0) / (double)sourceAzimuthResolution;

            List<Point> results = sineWave.getSinePoints();

            int expectedYPoint = (int)((double)depth + ((double)Math.Sin((10.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, results[10].Y, "Ypoint at x=10 should be " + expectedYPoint + ". It is " + results[10].Y);

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((35.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, results[35].Y, "Ypoint at x=35 should be " + expectedYPoint + ". It is " + results[35].Y);

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((500.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, results[500].Y, "Ypoint at x=500 should be " + expectedYPoint + ". It is " + results[500].Y);
        }

        [Test]
        public void TestChange()
        {
            sineWave = new SineWave(145, 87, 12, sourceAzimuthResolution);

            double azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)87 * ((double)sourceAzimuthResolution / 360.0));
            double frequency = ((double)Math.PI * 2.0) / (double)sourceAzimuthResolution;

            int expectedYPoint = (int)((double)145 + ((double)Math.Sin((10.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)12));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(10).Y, "Ypoint at x=10 is wrong");

            expectedYPoint = (int)((double)145 + ((double)Math.Sin((35.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)12));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(35).Y, "Ypoint at x=35 is wrong");

            expectedYPoint = (int)((double)145 + ((double)Math.Sin((500.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)12));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(500).Y, "Ypoint at x=500 is wrong");

            sineWave.change(depth, azimuth, amplitude);

            azimuthDisplacement = ((double)sourceAzimuthResolution * 0.25) - ((double)azimuth * ((double)sourceAzimuthResolution / 360.0));
            frequency = ((double)Math.PI * 2.0) / (double)sourceAzimuthResolution;

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((10.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(10).Y, "Ypoint at x=10 is wrong");

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((35.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(35).Y, "Ypoint at x=35 is wrong");

            expectedYPoint = (int)((double)depth + ((double)Math.Sin((500.0 + (double)azimuthDisplacement) * ((double)frequency)) * (double)amplitude));
            Assert.AreEqual(expectedYPoint, sineWave.getSinepoint(500).Y, "Ypoint at x=500 is wrong");
        }
    }
}