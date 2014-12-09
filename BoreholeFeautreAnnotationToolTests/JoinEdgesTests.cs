using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoreholeFeautreAnnotationToolTests.Properties;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class JoinEdgesTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        public void TestConstructor()
        {
            bool[] data = getImageData(Resources.EdgeJoiningTestImage);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 4, "4 edges should have been found. " + edges.getEdges().Count + " were found.");

            JoinEdges joinEdges = new JoinEdges(edges.getEdges(), 10, 200, 100);
            joinEdges.join();

            Assert.IsTrue(joinEdges.getJoinedEdges().Count == 3, "3 edges should have been found. " + joinEdges.getJoinedEdges().Count + " were found.");
        }

        [TestMethod]
        public void TestSetDistanceToBridge()
        {
            bool[] data = getImageData(Resources.EdgeJoiningTestImage);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 4, "4 edges should have been found. " + edges.getEdges().Count + " were found.");

            JoinEdges joinEdges = new JoinEdges(edges.getEdges(), 10, 200, 100);
            joinEdges.setDistanceToBridge(20);
            joinEdges.join();

            Assert.IsTrue(joinEdges.getJoinedEdges().Count == 2, "2 edges should have been found. " + joinEdges.getJoinedEdges().Count + " were found.");
        }

        [TestMethod]
        public void TestLinkToCorrectEdge()
        {
            bool[] data = getImageData(Resources.LinkToCorrectEdgeImage);

            FindEdges edges = new FindEdges(data, 320, 100);
            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 3, "3 edges should have been found. " + edges.getEdges().Count + " were found.");

            Assert.IsTrue(edges.getEdges()[0].EdgeLength == 32, "edge 0's length should be 32. It is " + edges.getEdges()[0].EdgeLength);
            Assert.IsTrue(edges.getEdges()[1].EdgeLength == 173, "edge 1's length should be 173. It is " + edges.getEdges()[1].EdgeLength);
            Assert.IsTrue(edges.getEdges()[2].EdgeLength == 116, "edge 2's length should be 116. It is " + edges.getEdges()[2].EdgeLength);

            JoinEdges joinEdges = new JoinEdges(edges.getEdges(), 10, 320, 100);
            joinEdges.join();

            //Edges 1 and 2 should have joined as they are closer and edge 1 is bigger
            Assert.IsTrue(joinEdges.getJoinedEdges().Count == 2, "2 joined edge should have been found. " + joinEdges.getJoinedEdges().Count + " were found. ");

            Assert.IsTrue(joinEdges.getJoinedEdges()[0].EdgeLength == 289, "Edge 0 should be 289 pixels long. It is " + joinEdges.getJoinedEdges()[0].EdgeLength);
            Assert.IsTrue(joinEdges.getJoinedEdges()[1].EdgeLength == 32, "Edge 1 should be 32 pixels long. It is " + joinEdges.getJoinedEdges()[1].EdgeLength);

        }

        private bool[] getImageData(Bitmap originalImage)
        {
            byte[] tempData;

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