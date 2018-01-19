using NUnit.Framework;
using AutomaticFeatureDetection;
using BoreholeFeautreAnnotationToolTests.Properties;

namespace BoreholeFeautreAnnotationToolTests.AutomaticFeatureDetection
{
    
    internal sealed class BitmapConverterTests
    {
        [Test]
        public void Bitmap_converter_works()
        {
            var testImage = Resources.bitmapConverterTestImage;

            var bitmapData = BitmapConverter.GetRgbFromBitmap(testImage);

            Assert.IsTrue(bitmapData.Length == 100, 
                          "bitmapData should contain 100 values. It contains " + bitmapData.Length);

            //Top left square
            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var pos = i + (j * 10);

                    Assert.That(bitmapData[pos], Is.EqualTo(0));
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
    }
}
