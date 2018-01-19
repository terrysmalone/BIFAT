using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using EdgesTests.Properties;
using Edges;
using NUnit.Framework;

namespace EdgesTests
{
    [TestFixture]
    public class FindEdgesTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [Test]
        public void TestConstructor()
        {
            bool[] test = new bool[100];

            FindEdges edges = new FindEdges(test, 10, 10);

            edges.find();

            Assert.IsTrue(edges.getEdges().Count == 0, "No edges should have been found");
        }

        [Test]
        public void TestGetEdges()
        {
            bool[] data = getImageData(Resources.Edge1Test);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            List<Edge> foundEdges = edges.getEdges();

            Assert.IsTrue(foundEdges.Count == 5, "There should be 5 edges.  There are " + foundEdges.Count);

            //Edge 0
            Assert.IsTrue(foundEdges[0].Points.Contains(new Point(73, 23)), "Edge 0 should contain (73, 23)");
            Assert.IsTrue(foundEdges[0].Points.Contains(new Point(9, 36)), "Edge 0 should contain (9, 36)");
            Assert.IsTrue(foundEdges[0].Points.Contains(new Point(112, 28)), "Edge 0 should contain (112, 28)");
            Assert.IsTrue(foundEdges[0].Points.Contains(new Point(96, 27)), "Edge 0 should contain (96, 27)");

            Assert.IsFalse(foundEdges[0].Points.Contains(new Point(139, 32)), "Edge 0 should not contain (139, 32)");
            Assert.IsFalse(foundEdges[0].Points.Contains(new Point(113, 28)), "Edge 0 should not contain (113, 28)");
            Assert.IsFalse(foundEdges[0].Points.Contains(new Point(82, 50)), "Edge 0 should not contain (82, 50)");

            //Edge 1
            Assert.IsTrue(foundEdges[1].Points.Contains(new Point(128, 45)), "Edge 1 should contain (128, 45)");
            Assert.IsTrue(foundEdges[1].Points.Contains(new Point(187, 53)), "Edge 1 should contain (187, 53)");

            Assert.IsFalse(foundEdges[1].Points.Contains(new Point(72, 50)), "Edge 1 should not contain (72, 50)");
            Assert.IsFalse(foundEdges[1].Points.Contains(new Point(129, 57)), "Edge 1 should not contain (129, 57)");

            //Edge 2
            Assert.IsTrue(foundEdges[2].Points.Contains(new Point(44, 57)), "Edge 2 should contain (44, 57)");
            Assert.IsTrue(foundEdges[2].Points.Contains(new Point(89, 69)), "Edge 2 should contain (89, 69)");

            Assert.IsFalse(foundEdges[2].Points.Contains(new Point(41, 67)), "Edge 2 should not contain (41, 67)");
            Assert.IsFalse(foundEdges[2].Points.Contains(new Point(164, 29)), "Edge 2 should not contain (164, 29)");

            //Edge 3
            Assert.IsTrue(foundEdges[3].Points.Contains(new Point(43, 67)), "Edge 3 should contain (43, 67)");
            Assert.IsTrue(foundEdges[3].Points.Contains(new Point(30, 66)), "Edge 3 should contain (30, 66)");

            Assert.IsFalse(foundEdges[3].Points.Contains(new Point(0, 0)), "Edge 3 should not contain (0, 0)");
            Assert.IsFalse(foundEdges[3].Points.Contains(new Point(86, 69)), "Edge 3 should not contain (86, 69)");

            //Edge 4
            Assert.IsTrue(foundEdges[4].Points.Contains(new Point(167, 92)), "Edge 4 should contain (167, 92)");

            Assert.IsFalse(foundEdges[4].Points.Contains(new Point(104, 69)), "Edge 4 should not contain (104, 69)");
            Assert.IsFalse(foundEdges[4].Points.Contains(new Point(100, 50)), "Edge 4 should not contain (100, 50)");

        }

        [Test]
        public void TestGetNumberOfEdges()
        {
            bool[] data = getImageData(Resources.Edge1Test);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            Assert.IsTrue(edges.getNumberOfEdges() == 5, "There should be 5 edges.  There are " + edges.getNumberOfEdges());

        }

        [Test]
        public void TestEdgeEnds()
        {
            bool[] data = getImageData(Resources.Edge1Test);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            List<Edge> foundEdges = edges.getEdges();

            //Edge 0
            Assert.IsTrue(foundEdges[0].EdgeEnd1.Equals(new Point(9, 36)), "Edge 0, end 1 should be (9, 36).  It is " + foundEdges[0].EdgeEnd1);
            Assert.IsTrue(foundEdges[0].EdgeEnd2.Equals(new Point(112, 28)), "Edge 0, end 2 should be (112, 28).  It is " + foundEdges[0].EdgeEnd2);

            //Edge 1
            Assert.IsTrue(foundEdges[1].EdgeEnd1.Equals(new Point(74, 50)), "Edge 1, end 1 should be (74, 50).  It is " + foundEdges[1].EdgeEnd1);
            Assert.IsTrue(foundEdges[1].EdgeEnd2.Equals(new Point(187, 53)), "Edge 1, end 2 should be (187, 53).  It is " + foundEdges[1].EdgeEnd2);

            //Edge 2
            Assert.IsTrue(foundEdges[2].EdgeEnd1.Equals(new Point(44, 57)), "Edge 2, end 1 should be (44, 57).  It is " + foundEdges[2].EdgeEnd1);
            Assert.IsTrue(foundEdges[2].EdgeEnd2.Equals(new Point(104, 69)), "Edge 2, end 2 should be (104, 69).  It is " + foundEdges[2].EdgeEnd2);

            //Edge 3
            Assert.IsTrue(foundEdges[3].EdgeEnd1.Equals(new Point(27, 66)), "Edge 3, end 1 should be (27, 66).  It is " + foundEdges[3].EdgeEnd1);
            Assert.IsTrue(foundEdges[3].EdgeEnd2.Equals(new Point(43, 67)), "Edge 3, end 2 should be (43, 67).  It is " + foundEdges[3].EdgeEnd2);

            //Edge 4
            Assert.IsTrue(foundEdges[4].EdgeEnd1.Equals(new Point(167, 92)), "Edge 3, end 1 should be (167, 92).  It is " + foundEdges[4].EdgeEnd1);
            Assert.IsTrue(foundEdges[4].EdgeEnd2.Equals(new Point(167, 92)), "Edge 3, end 2 should be (167, 92).  It is " + foundEdges[4].EdgeEnd2);


        }

        [Test]
        public void TestEndToEndEdge()
        {
            bool[] data = getImageData(Resources.EndToEndTestImage);

            FindEdges edges = new FindEdges(data, 720, 261);
            edges.find();

            Assert.IsTrue(edges.getNumberOfEdges() == 2, "There should be 2 edges. There are " + edges.getNumberOfEdges());

            Assert.IsTrue(edges.getEdges()[0].EdgeLength == 720, "Edge 0 should be 720 pixels long.  It is " + edges.getEdges()[0].EdgeLength);

            Assert.IsTrue(edges.getEdges()[0].EdgeEnd1.Equals(new Point(0, 99)), "Edge 0 end 1 should be (0, 99).  It is " + edges.getEdges()[0].EdgeEnd1);
            Assert.IsTrue(edges.getEdges()[0].EdgeEnd2.Equals(new Point(719, 99)), "Edge 0 end 2 should be (719, 99).  It is " + edges.getEdges()[0].EdgeEnd2);

            Assert.IsTrue(edges.getEdges()[1].EdgeLength == 720, "Edge 1 should be 720 pixels long.  It is " + edges.getEdges()[1].EdgeLength);

            Assert.IsTrue(edges.getEdges()[1].EdgeEnd1.Equals(new Point(0, 143)), "Edge 1 end 1 should be (0, 143).  It is " + edges.getEdges()[1].EdgeEnd1);
            Assert.IsTrue(edges.getEdges()[1].EdgeEnd2.Equals(new Point(719, 143)), "Edge 1 end 2 should be (719, 143).  It is " + edges.getEdges()[1].EdgeEnd2);


        }

        [Test]
        public void TestWrappedEdge()
        {
            bool[] data = getImageData(Resources.Edge2Test);

            FindEdges edges = new FindEdges(data, 200, 100);
            edges.find();

            Assert.IsTrue(edges.getNumberOfEdges() == 2, "There should be 2 edges. There are " + edges.getNumberOfEdges());

            Assert.IsTrue(edges.getEdges()[0].EdgeLength == 98, "Edge 0 should be 98 pixels long.  It is " + edges.getEdges()[0].EdgeLength);

            Assert.IsTrue(edges.getEdges()[0].EdgeEnd1.Equals(new Point(156, 9)), "Edge 0 end 1 should be (156, 9).  It is " + edges.getEdges()[0].EdgeEnd1);
            Assert.IsTrue(edges.getEdges()[0].EdgeEnd2.Equals(new Point(49, 27)), "Edge 0 end 2 should be (49, 27).  It is " + edges.getEdges()[0].EdgeEnd2);

            Assert.IsTrue(edges.getEdges()[1].EdgeEnd1.Equals(new Point(75, 51)), "Edge 1 end 1 should be (75, 51).  It is " + edges.getEdges()[1].EdgeEnd1);
            Assert.IsTrue(edges.getEdges()[1].EdgeEnd2.Equals(new Point(68, 81)), "Edge 1 end 2 should be (68, 81).  It is " + edges.getEdges()[1].EdgeEnd2);
        }

        private bool[] getImageData(Bitmap originalImage)
        {
            byte[] tempData;

            originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //imageHeight = originalImage.Height;
            //imageWidth = originalImage.Width;

            //imageSize = imageWidth * imageHeight;

            //Get data from image
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            originalImage.Save(ms, ImageFormat.Bmp);
            tempData = ms.GetBuffer();

            //originalImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
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
                //Assert.IsTrue(false, "length: " + imageData.Length);

                if (imageData[i * 3] > 0)
                    boolData[i] = true;
                else
                    boolData[i] = false;
            }

            return boolData;
        }
    }
}
