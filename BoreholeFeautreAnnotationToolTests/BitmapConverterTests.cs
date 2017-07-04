using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomaticFeatureDetection;
using BoreholeFeautreAnnotationToolTests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class BitmapConverterTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        [TestMethod]
        public void TestBitmapConverter()
        {
            Bitmap testImage = Resources.bitmapConverterTestImage;
            
            int[] bitmapData = BitmapConverter.GetRgbFromBitmap(testImage);

            Assert.IsTrue(bitmapData.Length == 100, "bitmapData should contain 100 values. It contains " + bitmapData.Length);

            //Top left square
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int pos = i + (j * 10);

                    Assert.IsTrue(bitmapData[pos].Equals(0), "bitmapData at (" + i + ", " + j + ") should be 0. It is " + bitmapData[pos]);
                }
            }

            //Top right square
            for (int i = 5; i < 10; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int pos = i + (j * 10);

                    Assert.IsTrue(bitmapData[pos].Equals(16711680), "bitmapData at (" + i + ", " + j + ") should be 16711680. It is " + bitmapData[pos]);
                }
            }

            //Bottom Left square
            for (int i = 0; i < 5; i++)
            {
                for (int j = 5; j < 10; j++)
                {
                    int pos = i + (j * 10);

                    Assert.IsTrue(bitmapData[pos].Equals(16777215), "bitmapData at (" + i + ", " + j + ") should be 16777215. It is " + bitmapData[pos]);
                }
            }

            //Bottom right square
            for (int i = 5; i < 10; i++)
            {
                for (int j = 5; j < 10; j++)
                {
                    int pos = i + (j * 10);

                    Assert.IsTrue(bitmapData[pos].Equals(38655), "bitmapData at (" + i + ", " + j + ") should be 38655. It is " + bitmapData[pos]);
                }
            }
        }
    }
}
