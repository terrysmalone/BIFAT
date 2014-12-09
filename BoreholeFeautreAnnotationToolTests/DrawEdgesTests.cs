using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEdges.DrawEdgesFactory;
using Edges;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    /// <summary>
    /// Tests the DrawEdgesImage class with the following data
    /// 0000010000
    /// 0100000011
    /// 0011100110
    /// 0100011000
    /// 0010000000
    /// </summary>
    [TestClass]
    public class DrawEdgesTests
    {
        private int BLACK = -16777216;
        private int WHITE = -1;

        [TestMethod]
        public void TestBoolConstructor()
        {
            bool[] edgeData = createBoolEdgeData();

            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Bool");
            DrawEdgesImage drawEdges = factory.setUpDrawEdges(edgeData, 10, 5);

            drawEdges.drawEdgesImage();
            Bitmap testImage = drawEdges.getDrawnEdges();

            for (int i = 0; i < edgeData.Length; i++)
            {
                int xPos = i % 10;
                int yPos = (i - xPos) / 10;

                if (edgeData[i] == false)
                {
                    Assert.IsTrue(testImage.GetPixel(xPos, yPos).ToArgb() == BLACK, "Pixel at " + xPos + ", " + yPos + " should be black.  It is " + testImage.GetPixel(xPos, yPos).ToArgb());
                }
                else
                {
                    Assert.IsTrue(testImage.GetPixel(xPos, yPos).ToArgb() == WHITE, "Pixel " + i + ", at " + xPos + ", " + yPos + " should be white.  It is " + testImage.GetPixel(xPos, yPos).ToArgb());
                }
            }
        }

        private bool[] createBoolEdgeData()
        {
            bool[] data = new bool[50];

            data[5] = true;
            data[11] = true;
            data[18] = true;
            data[19] = true;
            data[22] = true;
            data[23] = true;
            data[24] = true;
            data[27] = true;
            data[28] = true;
            data[31] = true;
            data[35] = true;
            data[36] = true;
            data[42] = true;

            return data;
        }
    }
}

