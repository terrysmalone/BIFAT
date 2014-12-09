using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoreholeFeautreAnnotationToolTests.Properties;
using EdgeDetector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class CannyDetectorTests
    {       
        [TestMethod]
        public void TestIntCross()
        {
            int imageWidth = 90;
            int imageHeight = 180;

            //Create int array
            int[] imageData = new int[imageWidth * imageHeight];

            //Draw vertical bar
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    if (x >= 36 && x <= 53)
                        imageData[x + (y * imageWidth)] = 0;
                    else
                        imageData[x + (y * imageWidth)] = 255;
                }
            }

            //add horizontal bar
            for (int y = 81; y < 98; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    imageData[x + (y * imageWidth)] = 0;
                }
            }

            CannyDetector edgeDetector = new CannyDetector(imageData, imageWidth, imageHeight);

            edgeDetector.DetectEdges();

            bool[] edgeData = edgeDetector.GetEdgeData();

            Assert.IsTrue(edgeData[35 + (17 * imageWidth)] == true, "(35,17) should be true");
            Assert.IsTrue(edgeData[36 + (17 * imageWidth)] == false, "(36,17) should be false");

            Assert.IsTrue(edgeData[62 + (81 * imageWidth)] == true, "(62,81) should be true");
            Assert.IsTrue(edgeData[64 + (81 * imageWidth)] == true, "(64,81) should be true");
            Assert.IsTrue(edgeData[74 + (105 * imageWidth)] == false, "(74,105) should be false");
        }

        [TestMethod]
        public void TestGetMagnitude()
        {
            int imageWidth = 90;
            int imageHeight = 180;

            //Create int array
            int[] imageData = new int[imageWidth * imageHeight];

            //Draw vertical bar
            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    if (x >= 36 && x <= 53)
                        imageData[x + (y * imageWidth)] = 0;
                    else
                        imageData[x + (y * imageWidth)] = 255;
                }
            }

            //add horizontal bar
            for (int y = 81; y < 98; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    imageData[x + (y * imageWidth)] = 0;
                }
            }

            CannyDetector edgeDetector = new CannyDetector(imageData, imageWidth, imageHeight);

            edgeDetector.DetectEdges();

            //Assert.IsTrue(edgeDetector.getEdgeMagnitude(35, 60) == 0, "(35,60) should be 0. It is " + edgeDetector.getEdgeMagnitude(35, 60));
            //Assert.IsTrue(edgeDetector.getEdgeMagnitude(21, 81) == 0, "(21,81) should be 0. It is " + edgeDetector.getEdgeMagnitude(21, 81));
            //Assert.IsTrue(edgeDetector.getEdgeMagnitude(54, 80) == 0, "(54,80) should be 0. It is " + edgeDetector.getEdgeMagnitude(54, 80));

        }

        [TestMethod]
        public void TestGoToLeftEdge()
        {
            Bitmap originalImage = Resources._1a;

            int[] imageData = BitmapConverter.BitmapToRGBArray(originalImage);

            CannyDetector edgeDetector = new CannyDetector(imageData, originalImage.Width, originalImage.Height);

            edgeDetector.WrapHorizontally = true;

            edgeDetector.GaussianWidth = 20;
            edgeDetector.GaussianSigma = 6.0f;
            edgeDetector.HighThreshold = 0.02f;
            edgeDetector.LowThreshold = 0.01f;

            edgeDetector.HorizontalWeight = 0.0f;
            edgeDetector.VerticalWeight = 1.0f;

            edgeDetector.DetectEdges();

            Bitmap afterEdgeDetectImage = edgeDetector.GetEdgesBitmap();

            Assert.IsTrue(afterEdgeDetectImage.GetPixel(0, 77).R == 255, "Pixel (0,77) should be white. It is " + afterEdgeDetectImage.GetPixel(0, 77).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(0, 174).R == 255, "Pixel (0,174) should be white. It is " + afterEdgeDetectImage.GetPixel(0, 174).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(0, 245).R == 255, "Pixel (0,245) should be white. It is " + afterEdgeDetectImage.GetPixel(0, 245).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(0, 569).R == 255, "Pixel (0,569) should be white. It is " + afterEdgeDetectImage.GetPixel(0, 569).R);
        }

        [TestMethod]
        public void TestGoToRightEdge()
        {
            Bitmap originalImage = Resources._1a;

            int[] imageData = BitmapConverter.BitmapToRGBArray(originalImage);

            CannyDetector edgeDetector = new CannyDetector(imageData, originalImage.Width, originalImage.Height);

            edgeDetector.WrapHorizontally = true;

            edgeDetector.GaussianWidth = 20;
            edgeDetector.GaussianSigma = 6.0f;
            edgeDetector.HighThreshold = 0.02f;
            edgeDetector.LowThreshold = 0.01f;

            edgeDetector.HorizontalWeight = 0.0f;
            edgeDetector.VerticalWeight = 1.0f;
            edgeDetector.DetectEdges();

            Bitmap afterEdgeDetectImage = edgeDetector.GetEdgesBitmap();


            Assert.IsTrue(afterEdgeDetectImage.GetPixel(719, 569).R == 255, "Pixel (719,569) should be white. It is " + afterEdgeDetectImage.GetPixel(719, 569).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(719, 359).R == 255, "Pixel (719,359) should be white. It is " + afterEdgeDetectImage.GetPixel(719, 359).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(719, 174).R == 255, "Pixel (719,174) should be white. It is " + afterEdgeDetectImage.GetPixel(719, 174).R);
            Assert.IsTrue(afterEdgeDetectImage.GetPixel(719, 77).R == 255, "Pixel (719, 77) should be white. It is " + afterEdgeDetectImage.GetPixel(719, 77).R);
        }
    }
}

