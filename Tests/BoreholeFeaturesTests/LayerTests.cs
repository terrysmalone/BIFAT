using System;
using System.Globalization;
using BoreholeFeatures;
using NUnit.Framework;

namespace BoreholeFeaturesTests
{
    [TestFixture]
    public class LayerTest
    {
        [Test]
        public void Long_constructor_sets_properties()
        {
            const int firstDepth = 100;
            const int firstAmplitude = 10;
            const int firstAzimuth = 235;
            const int secondDepth = 213;
            const int secondAmplitude = 13;
            const int secondAzimuth = 320;

            var layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, 
                                                         firstAmplitude, 
                                                         firstAzimuth, 
                                                         secondDepth, 
                                                         secondAmplitude, 
                                                         secondAzimuth, 
                                                         azimuthResolution: 360, 
                                                         depthResolution: 1);

            Assert.That(testLayer.TopEdgeDepth, Is.EqualTo(firstDepth));
            Assert.That(testLayer.BottomEdgeDepth, Is.EqualTo(secondDepth));

            Assert.That(testLayer.TopSineAmplitude, Is.EqualTo(firstAmplitude));
            Assert.That(testLayer.BottomSineAmplitude, Is.EqualTo(secondAmplitude));
            
            Assert.That(testLayer.TopSineAzimuth, Is.EqualTo(firstAzimuth));
            Assert.That(testLayer.BottomSineAzimuth, Is.EqualTo(secondAzimuth));

            Assert.That(testLayer.TopEdgeDepthMm, Is.EqualTo(firstDepth));
            Assert.That(testLayer.BottomEdgeDepthMm, Is.EqualTo(secondDepth));

            Assert.That(testLayer.GetTopEdgePoints(), Is.Not.Null);
            Assert.That(testLayer.GetBottomEdgePoints(), Is.Not.Null);

            Assert.That(testLayer.StartY, Is.EqualTo(firstDepth - firstAmplitude));
            Assert.That(testLayer.EndY, Is.EqualTo(secondDepth + secondAmplitude));

            Assert.That(testLayer.Quality, Is.EqualTo(4));

            Assert.That(testLayer.SourceStartDepth, Is.EqualTo(0));
            Assert.That(testLayer.SourceEndDepth, Is.EqualTo(0));

            Assert.That(testLayer.SourceAzimuthResolution, Is.EqualTo(360));
            Assert.That(testLayer.SourceDepthResolution, Is.EqualTo(1));
        }

        [Test]
        public void TestSetStartDepth()
        {
            const int firstDepth = 210;
            const int firstAmplitude = 15;
            const int firstAzimuth = 300;
            const int secondDepth = 510;
            const int secondAmplitude = 13;
            const int secondAzimuth = 210;

            var layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, 
                                                         firstAmplitude, 
                                                         firstAzimuth, 
                                                         secondDepth, 
                                                         secondAmplitude, 
                                                         secondAzimuth, 
                                                         360, 
                                                         1);
            
            testLayer.SetBoreholeStartDepth(300);

            Assert.IsTrue(testLayer.TopEdgeDepth == firstDepth, "First depth should be  " + firstDepth + ". It is " + testLayer.TopEdgeDepth);
            Assert.IsTrue(testLayer.BottomEdgeDepth == secondDepth, "Second depth should be " + secondDepth + ". It is " + testLayer.BottomEdgeDepth);

            Assert.IsTrue(testLayer.TopSineAmplitude == firstAmplitude, "First amplitude should be " + firstAmplitude + ". It is " + testLayer.TopSineAmplitude);
            Assert.IsTrue(testLayer.BottomSineAmplitude == secondAmplitude, "Second amplitude should be " + secondAmplitude + ". It is " + testLayer.BottomSineAmplitude);

            Assert.IsTrue(testLayer.TopSineAzimuth == firstAzimuth, "First azimuth should be " + firstAzimuth + ". It is " + testLayer.TopSineAzimuth);
            Assert.IsTrue(testLayer.BottomSineAzimuth == secondAzimuth, "Second azimuth should be " + secondAzimuth + ". It is " + testLayer.BottomSineAzimuth);

            Assert.IsTrue(testLayer.TopEdgeDepthMm == firstDepth + 300, "First depth should be " + (firstDepth + 300) + "mm. It is " + testLayer.TopEdgeDepthMm);
            Assert.IsTrue(testLayer.BottomEdgeDepthMm == secondDepth + 300, "Second depth should be  " + (secondDepth + 300) + "mm. It is " + testLayer.BottomEdgeDepthMm);

            Assert.IsTrue(testLayer.StartY == firstDepth - firstAmplitude, "Start y should be " + (firstDepth - firstAmplitude).ToString() + ". It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == secondDepth + secondAmplitude, "End y should be " + (secondDepth + secondAmplitude).ToString() + ". It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.SourceStartDepth== 300, "Source start depth should be 300. It is " + testLayer.SourceStartDepth);
            Assert.IsTrue(testLayer.SourceEndDepth == 0, "Source end depth should be 0. It is " + testLayer.SourceEndDepth);
        }

        [Test]
        public void TestSetEndDepth()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 360, 1);

            Assert.IsTrue(testLayer.SourceEndDepth == 0, "Source end depth should be 0. It is " + testLayer.SourceEndDepth);

            testLayer.SetBoreholeEndDepth(500);

            Assert.IsTrue(testLayer.SourceEndDepth == 500, "Source end depth should be 500. It is " + testLayer.SourceEndDepth);
        }

        [Test]
        public void TestSetLayerType()
        {
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            Assert.IsTrue(testLayer.LayerType.Equals(""), "Layer type should be null.  It is " + testLayer.LayerType);

            testLayer.LayerVoid = true;

            Assert.IsTrue(testLayer.LayerType.Equals("void "), "Layer type should be 'void'.  It is " + testLayer.LayerType);

        }

        [Test]
        public void TestDescription()
        {
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            Assert.IsTrue(testLayer.Description.Equals(""), "Description should be blank.  It is " + testLayer.Description);

            testLayer.Description = "A faint debris layer";

            Assert.IsTrue(testLayer.Description.Equals("A faint debris layer"), "Description should be 'A faint debris layer'.  It is " + testLayer.Description);
        }

        [Test]
        public void TestSetQuality()
        {
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            Assert.IsTrue(testLayer.Quality == 4, "Quality should be 4.  It is " + testLayer.Quality);

            testLayer.Quality = 3;
            Assert.IsTrue(testLayer.Quality == 3, "Quality should be 3.  It is " + testLayer.Quality);

            testLayer.Quality = 2;
            Assert.IsTrue(testLayer.Quality == 2, "Quality should be 2.  It is " + testLayer.Quality);
        }

        [Test]
        public void TestSetAdded()
        {
            DateTime timeStamp = new DateTime(2011, 06, 12, 2, 54, 32);

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);
            Assert.IsFalse(testLayer.TimeAdded.Equals(timeStamp), "Time stamps should not match. Time added: " + testLayer.TimeAdded + " - timeStamp: " + timeStamp);

            testLayer.TimeAdded = timeStamp;
            Assert.IsTrue(testLayer.TimeAdded.Equals(timeStamp), "Time stamps should match. Time added: " + testLayer.TimeAdded + " - timeStamp: " + timeStamp);
        }

        [Test]
        public void TestSetTimeLastModified()
        {
            DateTime timeStamp = new DateTime(2011, 06, 12, 2, 54, 32);

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(100, 10, 360, 200, 10, 360, 720, 1);

            testLayer.ChangeBottomAmplitudeBy(10);
            DateTime timeLastModifiedTimeStamp = DateTime.Now;

            Assert.IsTrue(testLayer.TimeLastModified.Equals(timeLastModifiedTimeStamp), "Time stamps should match. Time added: " + testLayer.TimeLastModified + " - timeStamp: " + timeLastModifiedTimeStamp);

            testLayer.TimeLastModified = timeStamp;

            Assert.IsTrue(testLayer.TimeLastModified.Equals(timeStamp), "Time stamps should match. Time added: " + testLayer.TimeLastModified + " - timeStamp: " + timeStamp);
        }

        [Test]
        public void TestCalculateStartY()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 360, 1);

            Assert.IsTrue(testLayer.StartY == 195, "Start Y should be 195. It is " + testLayer.StartY);
        }

        [Test]
        public void TestCalculateEndY()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 360, 1);

            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);
        }

        [Test]
        public void TestChangeAmplitude()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 360, 1);

            Assert.IsTrue(testLayer.TopSineAmplitude == 15, "First amplitude should now be 15.  It is " + testLayer.TopSineAmplitude);
            Assert.IsTrue(testLayer.BottomSineAmplitude == 13, "First amplitude should now be 13.  It is " + testLayer.BottomSineAmplitude);

            Assert.IsTrue(testLayer.StartY == 195, "Start Y should be 195. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);

            testLayer.ChangeTopAmplitudeBy(-4);
            testLayer.ChangeBottomAmplitudeBy(19);

            Assert.IsTrue(testLayer.TopSineAmplitude == 11, "First amplitude should now be 11.  It is " + testLayer.TopSineAmplitude);
            Assert.IsTrue(testLayer.BottomSineAmplitude == 32, "First amplitude should now be 32.  It is " + testLayer.BottomSineAmplitude);

            Assert.IsTrue(testLayer.StartY == 199, "Start Y should be 199. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 542, "End Y should be 542. It is " + testLayer.EndY);
        }

        [Test]
        public void TestMoveSine()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 360, 1);

            Assert.IsTrue(testLayer.TopEdgeDepth == 210, "First depth should be 210");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 510, "First depth should be 510");

            Assert.IsTrue(testLayer.StartY == 195, "Start Y should be 195. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 300, "First azimuth should be 300");
            Assert.IsTrue(testLayer.BottomSineAzimuth == 210, "Second azimuth should be 210");

            testLayer.MoveEdge(1, -70, 10);

            Assert.IsTrue(testLayer.TopEdgeDepth == 220, "First depth should be 220");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 510, "Second depth should be 510");

            Assert.IsTrue(testLayer.StartY == 205, "Start Y should be 205. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 230, "First azimuth should be 230");
            Assert.IsTrue(testLayer.BottomSineAzimuth == 210, "Second azimuth should be 210");

            testLayer.MoveEdge(2, 20, -10);

            Assert.IsTrue(testLayer.TopEdgeDepth == 220, "First depth should be 220");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 500, "Second depth should be 500");

            Assert.IsTrue(testLayer.StartY == 205, "Start Y should be 205. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 513, "End Y should be 513. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 230, "First azimuth should be 230");
            Assert.IsTrue(testLayer.BottomSineAzimuth == 230, "Second azimuth should be 230");
        }

        [Test]
        public void Test2MMDepthResolution()
        {
            int firstDepth = 210;
            int firstAmplitude = 15;
            int firstAzimuth = 300;
            int secondDepth = 510;
            int secondAmplitude = 13;
            int secondAzimuth = 210;

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(firstDepth, firstAmplitude, firstAzimuth, secondDepth, secondAmplitude, secondAzimuth, 720, 2);

            Assert.IsTrue(testLayer.TopEdgeDepth == 210, "First depth should be 210");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 510, "First depth should be 510");

            Assert.IsTrue(testLayer.StartY == 195, "Start Y should be 195. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 300, "First azimuth should be 300");
            Assert.IsTrue(testLayer.BottomSineAzimuth == 210, "Second azimuth should be 210");

            testLayer.MoveEdge(1, -70, 10);

            Assert.IsTrue(testLayer.TopEdgeDepth == 220, "First depth should be 220");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 510, "Second depth should be 510");

            Assert.IsTrue(testLayer.StartY == 205, "Start Y should be 205. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 523, "End Y should be 523. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 265, "First azimuth should be 230.  It is " + testLayer.TopSineAzimuth);
            Assert.IsTrue(testLayer.BottomSineAzimuth == 210, "Second azimuth should be 210");

            testLayer.MoveEdge(2, 20, -10);

            Assert.IsTrue(testLayer.TopEdgeDepth == 220, "First depth should be 220");
            Assert.IsTrue(testLayer.BottomEdgeDepth == 500, "Second depth should be 500");

            Assert.IsTrue(testLayer.StartY == 205, "Start Y should be 205. It is " + testLayer.StartY);
            Assert.IsTrue(testLayer.EndY == 513, "End Y should be 513. It is " + testLayer.EndY);

            Assert.IsTrue(testLayer.TopSineAzimuth == 265, "First azimuth should be 265");
            Assert.IsTrue(testLayer.BottomSineAzimuth == 220, "Second azimuth should be 220. It is " + testLayer.BottomSineAzimuth);
        }

        [Test]
        public void TestTimeAdded()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            DateTime timeAdded = Convert.ToDateTime(testLayer.TimeAdded, dtfi);

            timeAdded = timeAdded.AddDays(4.0);

            Layer addedLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            Assert.IsFalse(addedLayer.TimeAdded.Equals(timeAdded), "Layers times should not match. They are " + addedLayer.TimeAdded + " and " + timeAdded.ToString());

            addedLayer.TimeAdded = timeAdded;

            Assert.IsTrue(addedLayer.TimeAdded.Equals(timeAdded), "Layers times should match. They are " + addedLayer.TimeAdded + " and " + timeAdded.ToString());
        }

        [Test]
        public void TestTimeLastModified()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            var testLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            DateTime timeModified = Convert.ToDateTime(testLayer.TimeAdded, dtfi);

            timeModified = timeModified.AddDays(4.0);

            Layer addedLayer = layerTypeSelector.setUpLayer(0, 0, 0, 0, 0, 0, 720, 1);

            Assert.IsFalse(addedLayer.TimeLastModified.Equals(timeModified), "Layers times should not match. They are " + addedLayer.TimeLastModified + " and " + timeModified.ToString());

            addedLayer.TimeLastModified = timeModified;

            Assert.IsTrue(addedLayer.TimeLastModified.Equals(timeModified), "Layers times should match. They are " + addedLayer.TimeLastModified + " and " + timeModified.ToString());

            addedLayer.MoveEdge(1, 10, 10);

            Assert.IsFalse(addedLayer.TimeLastModified.Equals(timeModified), "Layers times should not match. They are " + addedLayer.TimeLastModified + " and " + timeModified.ToString());
        }
    }
}

