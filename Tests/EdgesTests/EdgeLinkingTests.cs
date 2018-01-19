using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Edges;
using NUnit.Framework;
using EdgesTests.Properties;

namespace EdgesTests
{
    [TestFixture]
    public class EdgeLinkingTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [Test]
        public void TestEdgeLinking()
        {            
            bool[] data = getImageData(Resources.EdgeLinkingTestImage);
            FindEdges edges = new FindEdges(data, 316, 150);
            edges.find();
            
            Assert.IsTrue(edges.getEdges().Count == 21, "21 edges should have been found. " + edges.getEdges().Count + " were found.");
            
            EdgeLinking edgeLinking = new EdgeLinking(edges.getEdges(), 316, 150, 20);

            edgeLinking.MaximumNeighbourDistance = 15;
            edgeLinking.Link();

            List<Edge> linkedEdges = edgeLinking.LinkedEdges;
            
            Assert.IsTrue(linkedEdges.Count == 2, "2 linked edge should have been found. " + linkedEdges.Count + " were found.");
        }

        [Test]
        public void TestMaxNeighbourDistance()
        {
            bool[] data = getImageData(Resources.EdgeLinkingTestImage);

            FindEdges edges = new FindEdges(data, 316, 150);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 21, "21 edges should have been found. " + edges.getEdges().Count + " were found.");

            EdgeLinking edgeLinking = new EdgeLinking(edges.getEdges(), 316, 150, 20);

            edgeLinking.MaximumNeighbourDistance = 15;
            edgeLinking.Link();
            
            Assert.IsTrue(edgeLinking.LinkedEdges.Count == 2, "2 linked edges should have been found. " + edgeLinking.LinkedEdges.Count + " were found.");
        }

        [Test]
        public void TestMinimumLength()
        {
            bool[] data = getImageData(Resources.EdgeLinkingTestImage);

            FindEdges edges = new FindEdges(data, 316, 150);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 21, "21 edges should have been found. " + edges.getEdges().Count + " were found.");

            EdgeLinking edgeLinking = new EdgeLinking(edges.getEdges(), 316, 150, 20);
            edgeLinking.MinimumLength = 5;
            edgeLinking.Link();

            Assert.IsTrue(edgeLinking.LinkedEdges.Count == 5, "5 linked edges should have been found. " + edgeLinking.LinkedEdges.Count + " were found. 0:");

            FindEdges edges2 = new FindEdges(data, 316, 150);
            edges2.find(); 
            
            EdgeLinking edgeLinking2 = new EdgeLinking(edges2.getEdges(), 316, 150, 20);
            edgeLinking2.MinimumLength = 10;
            edgeLinking2.Link();

            Assert.IsTrue(edgeLinking2.LinkedEdges.Count == 3, "3 linked edges should have been found. " + edgeLinking2.LinkedEdges.Count + " were found. 0:");

            FindEdges edges3 = new FindEdges(data, 316, 150);
            edges3.find(); 

            EdgeLinking edgeLinking3 = new EdgeLinking(edges3.getEdges(), 316, 150, 20);
            edgeLinking3.MinimumLength = 89;
            edgeLinking3.Link();

            Assert.IsTrue(edgeLinking3.LinkedEdges.Count == 1, "1 linked edge should have been found. " + edgeLinking3.LinkedEdges.Count + " were found.");

            FindEdges edges4 = new FindEdges(data, 316, 150);
            edges3.find();

            EdgeLinking edgeLinking4 = new EdgeLinking(edges4.getEdges(), 316, 150, 20);
            edgeLinking4.MinimumLength = 100;
            edgeLinking4.Link();

            Assert.IsTrue(edgeLinking4.LinkedEdges.Count == 0, "0 linked edge should have been found. " + edgeLinking4.LinkedEdges.Count + " were found.");
        }

        [Test]
        public void TestLinkToCorrectEdge()
        {
            bool[] data = getImageData(Resources.LinkToCorrectEdgeImage);

            FindEdges edges = new FindEdges(data, 320, 100);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 3, "3 edges should have been found. " + edges.getEdges().Count + " were found.");

            Assert.IsTrue(edges.getEdges()[0].EdgeLength == 32, "edge 0's length should be 32. It is " + edges.getEdges()[0].EdgeLength);
            Assert.IsTrue(edges.getEdges()[1].EdgeLength == 173, "edge 1's length should be 173. It is " + edges.getEdges()[1].EdgeLength);
            Assert.IsTrue(edges.getEdges()[2].EdgeLength == 116, "edge 2's length should be 116. It is " + edges.getEdges()[2].EdgeLength);

            EdgeLinking edgeLinking = new EdgeLinking(edges.getEdges(), 320, 100, 100);
            edgeLinking.Link();

            //Edges 1 and 2 should have joined as they are closer and edge 1 is bigger
            Assert.IsTrue(edgeLinking.LinkedEdges.Count == 1, "1 linked edge should have been found. " + edgeLinking.LinkedEdges.Count + " were found. ");

            Assert.IsTrue(edgeLinking.LinkedEdges[0].EdgeLength == 289, "Edge 0 should be 289 pixels long. It is " + edgeLinking.LinkedEdges[0].EdgeLength);
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
