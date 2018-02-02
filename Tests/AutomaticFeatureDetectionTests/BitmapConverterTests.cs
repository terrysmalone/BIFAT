using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using AutomaticFeatureDetection;
using AutomaticFeatureDetectionTests.Properties;

namespace AutomaticFeatureDetectionTests
{
    internal sealed class BitmapConverterTests
    {
        [Test]
        public void Bitmap_converter_works()
        {
            //var testImage = Resources.bitmapConverterTestImage;
            var topLeft = Color.Black;
            var topRight = Color.Red;
            var bottomLeft = Color.White;
            var bottomRight = Color.Blue;

            var testImage = CreateSplitBitmap(topLeft,
                                              topRight,
                                              bottomLeft,
                                              bottomRight);

            var bitmapData = BitmapConverter.GetRgbFromBitmap(testImage);

            Assert.That(bitmapData.Length, Is.EqualTo(100));

            //Top left square
            topLeft.to
            var topleftColourValue = topLeft.ToArgb();

            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var pos = i + (j * 10);

                    Assert.That(bitmapData[pos], Is.EqualTo(topleftColourValue));
                }
            }

            //Top right square
            for (var i = 5; i < 10; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var pos = i + (j * 10);

                    Assert.That(bitmapData[pos], Is.EqualTo(16711680));
                }
            }

            //Bottom Left square
            for (var i = 0; i < 5; i++)
            {
                for (var j = 5; j < 10; j++)
                {
                    var pos = i + (j * 10);

                    Assert.That(bitmapData[pos], Is.EqualTo(16777215));
                }
            }

            //Bottom right square
            for (var i = 5; i < 10; i++)
            {
                for (var j = 5; j < 10; j++)
                {
                    var pos = i + (j * 10);

                    Assert.That(bitmapData[pos], Is.EqualTo(38655));
                }
            }
        }

        /// <summary>
        /// Create a bitmap split into four separate colours
        /// </summary>
        /// <returns></returns>
        private static Bitmap CreateSplitBitmap(Color topLeft,
                                                Color topRight,
                                                Color bottomLeft,
                                                Color bottomRight)
        {
            var splitImage = new Bitmap(10,10,PixelFormat.Format24bppRgb);

            FillSection(splitImage, topLeft, new Point(0,0), new Point(4, 0));
            FillSection(splitImage, topRight, new Point(5,0), new Point(9, 4));
            FillSection(splitImage, bottomLeft, new Point(0,5), new Point(4, 9));
            FillSection(splitImage, bottomRight, new Point(5,5), new Point(9, 9));  

            return splitImage;
        }

        private static void FillSection(Bitmap bitmap,
                                        Color fillColour,
                                        Point topLeft, 
                                        Point bottomRight)
        {
            for(var x = topLeft.X; x <= bottomRight.X; x++)
            {
                for(var y = topLeft.Y; y <= bottomRight.Y; y++)
                {
                    bitmap.SetPixel(x, y, fillColour);
                }
            }
        }
    }
}
