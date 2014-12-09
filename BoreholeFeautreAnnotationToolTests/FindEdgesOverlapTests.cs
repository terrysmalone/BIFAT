using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using BoreholeFeautreAnnotationToolTests.Properties;
using EdgeFitting;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class FindEdgesOverlapTests
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        public void TestOverlap()
        {
            bool[] edgeData = getImageData(Resources.TestOverlap);

            int imageWidth = 360;
            int imageHeight = 400;

            FindEdges findEdges = new FindEdges(edgeData, imageWidth, imageHeight);
            findEdges.find();
            List<Edge> edges = findEdges.getEdges();

            List<Sine> sines = new List<Sine>();

            for (int i = 0; i < edges.Count; i++)
            {
                BestFitSine bestFit = new BestFitSine(edges[i], imageWidth, imageHeight);
                bestFit.FindBestFit();
                sines.Add(bestFit.GetSine());
            }

            int maxAmplitude = 70;

            Overlap overlap = new Overlap(sines, edges, imageWidth, imageHeight, maxAmplitude);

            overlap.checkForOverlap();

            List<Edge> edgesAfterOverlap = overlap.getEdges();
            List<Sine> sinesAfterOverlap = overlap.getSines();

            Assert.IsTrue(edgesAfterOverlap.Count == 2, "There should now be 2 edges. There are " + edgesAfterOverlap.Count);
            Assert.IsTrue(sinesAfterOverlap.Count == 2, "There should now be 2 sines. There are " + sinesAfterOverlap.Count);
        }

        [TestMethod]
        public void TestWithOverlappingEdges()
        {
            bool[] edgeData = getImageData(Resources.TestOverlappingEdges);

            int imageWidth = 360;
            int imageHeight = 400;

            FindEdges findEdges = new FindEdges(edgeData, imageWidth, imageHeight);
            findEdges.find();
            List<Edge> edges = findEdges.getEdges();

            List<Sine> sines = new List<Sine>();

            for (int i = 0; i < edges.Count; i++)
            {
                BestFitSine bestFit = new BestFitSine(edges[i], imageWidth, imageHeight);
                bestFit.FindBestFit();
                sines.Add(bestFit.GetSine());
            }

            int maxAmplitude = 70;

            Overlap overlap = new Overlap(sines, edges, imageWidth, imageHeight, maxAmplitude);

            overlap.checkForOverlap();

            List<Edge> edgesAfterOverlap = overlap.getEdges();
            List<Sine> sinesAfterOverlap = overlap.getSines();

            Assert.IsTrue(edgesAfterOverlap.Count == 3, "There should now be 3 edges. There are " + edgesAfterOverlap.Count);
            Assert.IsTrue(sinesAfterOverlap.Count == 3, "There should now be 3 sines. There are " + sinesAfterOverlap.Count);

        }

        [TestMethod]
        public void TestImageDimensions()
        {
            bool[] edgeData = getImageData(Resources.TestOverlap);

            int imageWidth = 360;
            int imageHeight = 400;

            FindEdges findEdges = new FindEdges(edgeData, imageWidth, imageHeight);
            findEdges.find();
            List<Edge> edges = findEdges.getEdges();

            List<Sine> sines = new List<Sine>();

            for (int i = 0; i < edges.Count; i++)
            {
                BestFitSine bestFit = new BestFitSine(edges[i], imageWidth, imageHeight);
                bestFit.FindBestFit();
                sines.Add(bestFit.GetSine());
            }

            int maxAmplitude = 70;

            Overlap overlap = new Overlap(sines, edges, imageWidth, imageHeight, maxAmplitude);

            Assert.IsTrue(overlap.getImageWidth() == imageWidth, "Image width should be " + imageWidth + ". It is " + overlap.getImageWidth());
            Assert.IsTrue(overlap.getImageHeight() == imageHeight, "Image height should be " + imageHeight + ". It is " + overlap.getImageHeight());
        }

        private bool[] getImageData(Bitmap originalImage)
        {
            byte[] tempData;

            //Bitmap originalImage = (Bitmap)Bitmap.FromFile(file);

            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //Get data from image
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            originalImage.Save(ms, ImageFormat.Bmp);
            tempData = ms.GetBuffer();

            ms.Close();

            byte[] imageData = new byte[tempData.Length - 54];

            for (int i = 0; i < imageData.Length; i++)
            {
                imageData[i] = tempData[i + 54];
            }

            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            bool[] boolData = new bool[imageData.Length / 3];

            for (int i = 0; i < boolData.Length; i++)
            {
                if (imageData[i * 3] > 0)
                    boolData[i] = true;
                else
                    boolData[i] = false;
            }

            return boolData;
        }

    }
}
