using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using BrightnessAnalysisTests.Properties;
using BrightnessAnalysis;
using BoreholeFeatures;

namespace BrightnessAnalysisTests
{
    [TestFixture]
    public class AverageBrightnessTest
    {
        [Test]
        public void TestWithoutExcludePoints()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            AverageBrightness averageBrightness = new AverageBrightness();

            averageBrightness.ProcessSection(image, 0);
            
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(100) == 193, "Line 100 should have a brightness of 193. It is " + averageBrightness.GetBrightnessOfLine(100));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(300) == 116, "Line 300 should have a brightness of 116. It is " + averageBrightness.GetBrightnessOfLine(300));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(600) == 189, "Line 600 should have a brightness of 189. It is " + averageBrightness.GetBrightnessOfLine(600));
        }

        [Test]
        public void TestWithLayersExcluded()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            Layer layer1 = new BoreholeLayer(190, 28, 122, 429, 28, 122, 720, 1);
            Layer layer2 = new BoreholeLayer(524, 4, 100, 559, 4, 100, 720, 1);

            List<Layer> layers = new List<Layer>();
            layers.Add(layer1);
            layers.Add(layer2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllLayers(layers);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(100) == 193, "Line 100 should have a brightness of 193. It is " + averageBrightness.GetBrightnessOfLine(100));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(300) == -1, "Line 300 should have a brightness of -1. It is " + averageBrightness.GetBrightnessOfLine(300));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(600) == 189, "Line 600 should have a brightness of 189. It is " + averageBrightness.GetBrightnessOfLine(600));
        }

        [Test]
        public void TestWithLayersIncluded()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            Layer layer1 = new BoreholeLayer(190, 28, 122, 429, 28, 122, 720, 1);
            Layer layer2 = new BoreholeLayer(524, 4, 100, 559, 4, 100, 720, 1);

            List<Layer> layers = new List<Layer>();
            layers.Add(layer1);
            layers.Add(layer2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllLayers(layers);

            averageBrightness.AddLayersToIncludeList(layers);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(100) == 193, "Line 100 should have a brightness of 193. It is " + averageBrightness.GetBrightnessOfLine(100));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(300) == 116, "Line 300 should have a brightness of 116. It is " + averageBrightness.GetBrightnessOfLine(300));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(530) == 102, "Line 530 should have a brightness of 102. It is " + averageBrightness.GetBrightnessOfLine(530));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(600) == 189, "Line 600 should have a brightness of 189. It is " + averageBrightness.GetBrightnessOfLine(600));

        }

        [Test]
        public void TestWithOneLayerIncluded()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            Layer layer1 = new BoreholeLayer(190, 28, 122, 429, 28, 122, 720, 1);
            Layer layer2 = new BoreholeLayer(524, 4, 100, 559, 4, 100, 720, 1);

            List<Layer> layers = new List<Layer>();
            layers.Add(layer1);
            layers.Add(layer2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllLayers(layers);

            averageBrightness.AddLayerToIncludeList(layer1);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(100) == 193, "Line 100 should have a brightness of 193. It is " + averageBrightness.GetBrightnessOfLine(100));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(300) == 116, "Line 300 should have a brightness of 116. It is " + averageBrightness.GetBrightnessOfLine(300));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(530) == -1, "Line 530 should have a brightness of -1. It is " + averageBrightness.GetBrightnessOfLine(530));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(600) == 189, "Line 600 should have a brightness of 189. It is " + averageBrightness.GetBrightnessOfLine(600));
        }

        [Test]
        public void TestWithClustersExcluded()
        {
            Bitmap image = Resources.ExcludeInclusionsTestImage;
            
            Cluster cluster1 = new Cluster(720, 1);
            cluster1.AddPoint(new Point(130, 455));
            cluster1.AddPoint(new Point(128, 441));
            cluster1.AddPoint(new Point(131, 429));
            cluster1.AddPoint(new Point(131, 417));
            cluster1.AddPoint(new Point(125, 410));
            cluster1.AddPoint(new Point(112, 407));
            cluster1.AddPoint(new Point(101, 409));
            cluster1.AddPoint(new Point(93, 413));
            cluster1.AddPoint(new Point(87, 420));
            cluster1.AddPoint(new Point(78, 422));
            cluster1.AddPoint(new Point(73, 430));
            cluster1.AddPoint(new Point(76, 440));
            cluster1.AddPoint(new Point(80, 447));
            cluster1.AddPoint(new Point(85, 456));
            cluster1.AddPoint(new Point(93, 457));
            cluster1.AddPoint(new Point(102, 458));
            cluster1.AddPoint(new Point(114, 461));

            Cluster cluster2 = new Cluster(720, 1);
            cluster2.AddPoint(new Point(365, 458));
            cluster2.AddPoint(new Point(372, 448));
            cluster2.AddPoint(new Point(388, 443));
            cluster2.AddPoint(new Point(398, 427));
            cluster2.AddPoint(new Point(409, 419));
            cluster2.AddPoint(new Point(431, 374));
            cluster2.AddPoint(new Point(443, 372));
            cluster2.AddPoint(new Point(452, 362));
            cluster2.AddPoint(new Point(473, 363));
            cluster2.AddPoint(new Point(482, 357));
            cluster2.AddPoint(new Point(552, 359));
            cluster2.AddPoint(new Point(569, 375));
            cluster2.AddPoint(new Point(589, 396));
            cluster2.AddPoint(new Point(612, 437));
            cluster2.AddPoint(new Point(617, 459));
            cluster2.AddPoint(new Point(616, 495));
            cluster2.AddPoint(new Point(599, 514));
            cluster2.AddPoint(new Point(578, 526));
            cluster2.AddPoint(new Point(548, 523));
            cluster2.AddPoint(new Point(541, 530));
            cluster2.AddPoint(new Point(528, 530));
            cluster2.AddPoint(new Point(515, 541));
            cluster2.AddPoint(new Point(413, 541));
            cluster2.AddPoint(new Point(379, 494));

            List<Cluster> clusters = new List<Cluster>();
            clusters.Add(cluster1);
            clusters.Add(cluster2);

            AverageBrightness averageBrightness = new AverageBrightness();

            averageBrightness.SetAllClusters(clusters);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(150) == 212, "Line 150 should have a brightness of 212. It is " + averageBrightness.GetBrightnessOfLine(150));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(430) == 207, "Line 430 should have a brightness of 207. It is " + averageBrightness.GetBrightnessOfLine(430));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(651) == 204, "Line 651 should have a brightness of 204. It is " + averageBrightness.GetBrightnessOfLine(651));
        }

        [Test]
        public void TestWithClustersIncluded()
        {
            //throw new NotImplementedException();

            Bitmap image = Resources.ExcludeInclusionsTestImage;
            
            Cluster cluster1 = new Cluster(720, 1);
            cluster1.AddPoint(new Point(130, 455));
            cluster1.AddPoint(new Point(128, 441));
            cluster1.AddPoint(new Point(131, 429));
            cluster1.AddPoint(new Point(131, 417));
            cluster1.AddPoint(new Point(125, 410));
            cluster1.AddPoint(new Point(112, 407));
            cluster1.AddPoint(new Point(101, 409));
            cluster1.AddPoint(new Point(93, 413));
            cluster1.AddPoint(new Point(87, 420));
            cluster1.AddPoint(new Point(78, 422));
            cluster1.AddPoint(new Point(73, 430));
            cluster1.AddPoint(new Point(76, 440));
            cluster1.AddPoint(new Point(80, 447));
            cluster1.AddPoint(new Point(85, 456));
            cluster1.AddPoint(new Point(93, 457));
            cluster1.AddPoint(new Point(102, 458));
            cluster1.AddPoint(new Point(114, 461));

            Cluster cluster2 = new Cluster(720, 1);
            cluster2.AddPoint(new Point(365, 458));
            cluster2.AddPoint(new Point(372, 448));
            cluster2.AddPoint(new Point(388, 443));
            cluster2.AddPoint(new Point(398, 427));
            cluster2.AddPoint(new Point(409, 419));
            cluster2.AddPoint(new Point(431, 374));
            cluster2.AddPoint(new Point(443, 372));
            cluster2.AddPoint(new Point(452, 362));
            cluster2.AddPoint(new Point(473, 363));
            cluster2.AddPoint(new Point(482, 357));
            cluster2.AddPoint(new Point(552, 359));
            cluster2.AddPoint(new Point(569, 375));
            cluster2.AddPoint(new Point(589, 396));
            cluster2.AddPoint(new Point(612, 437));
            cluster2.AddPoint(new Point(617, 459));
            cluster2.AddPoint(new Point(616, 495));
            cluster2.AddPoint(new Point(599, 514));
            cluster2.AddPoint(new Point(578, 526));
            cluster2.AddPoint(new Point(548, 523));
            cluster2.AddPoint(new Point(541, 530));
            cluster2.AddPoint(new Point(528, 530));
            cluster2.AddPoint(new Point(515, 541));
            cluster2.AddPoint(new Point(413, 541));
            cluster2.AddPoint(new Point(379, 494));

            List<Cluster> clusters = new List<Cluster>();
            clusters.Add(cluster1);
            clusters.Add(cluster2);

            AverageBrightness averageBrightness = new AverageBrightness();

            averageBrightness.SetAllClusters(clusters);

            averageBrightness.AddClustersToIncludeList(clusters);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(150) == 212, "Line 150 should have a brightness of 212. It is " + averageBrightness.GetBrightnessOfLine(150));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(430) == 176, "Line 430 should have a brightness of 176. It is " + averageBrightness.GetBrightnessOfLine(430));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(651) == 204, "Line 651 should have a brightness of 204. It is " + averageBrightness.GetBrightnessOfLine(651));
        }

        [Test]
        public void TestWithInclusionsExcluded()
        {
            Bitmap image = Resources.ExcludeInclusionsTestImage;
            
            Inclusion inclusion1 = new Inclusion(720, 1);
            inclusion1.AddPoint(new Point(130, 455));
            inclusion1.AddPoint(new Point(128, 441));
            inclusion1.AddPoint(new Point(131, 429));
            inclusion1.AddPoint(new Point(131, 417));
            inclusion1.AddPoint(new Point(125, 410));
            inclusion1.AddPoint(new Point(112, 407));
            inclusion1.AddPoint(new Point(101, 409));
            inclusion1.AddPoint(new Point(93, 413));
            inclusion1.AddPoint(new Point(87, 420));
            inclusion1.AddPoint(new Point(78, 422));
            inclusion1.AddPoint(new Point(73, 430));
            inclusion1.AddPoint(new Point(76, 440));
            inclusion1.AddPoint(new Point(80, 447));
            inclusion1.AddPoint(new Point(85, 456));
            inclusion1.AddPoint(new Point(93, 457));
            inclusion1.AddPoint(new Point(102, 458));
            inclusion1.AddPoint(new Point(114, 461));

            Inclusion inclusion2 = new Inclusion(720, 1);
            inclusion2.AddPoint(new Point(365, 458));
            inclusion2.AddPoint(new Point(372, 448));
            inclusion2.AddPoint(new Point(388, 443));
            inclusion2.AddPoint(new Point(398, 427));
            inclusion2.AddPoint(new Point(409, 419));
            inclusion2.AddPoint(new Point(431, 374));
            inclusion2.AddPoint(new Point(443, 372));
            inclusion2.AddPoint(new Point(452, 362));
            inclusion2.AddPoint(new Point(473, 363));
            inclusion2.AddPoint(new Point(482, 357));
            inclusion2.AddPoint(new Point(552, 359));
            inclusion2.AddPoint(new Point(569, 375));
            inclusion2.AddPoint(new Point(589, 396));
            inclusion2.AddPoint(new Point(612, 437));
            inclusion2.AddPoint(new Point(617, 459));
            inclusion2.AddPoint(new Point(616, 495));
            inclusion2.AddPoint(new Point(599, 514));
            inclusion2.AddPoint(new Point(578, 526));
            inclusion2.AddPoint(new Point(548, 523));
            inclusion2.AddPoint(new Point(541, 530));
            inclusion2.AddPoint(new Point(528, 530));
            inclusion2.AddPoint(new Point(515, 541));
            inclusion2.AddPoint(new Point(413, 541));
            inclusion2.AddPoint(new Point(379, 494));

            List<Inclusion> inclusions = new List<Inclusion>();
            inclusions.Add(inclusion1);
            inclusions.Add(inclusion2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllInclusions(inclusions);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(150) == 212, "Line 150 should have a brightness of 212. It is " + averageBrightness.GetBrightnessOfLine(150));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(430) == 207, "Line 430 should have a brightness of 207. It is " + averageBrightness.GetBrightnessOfLine(430));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(651) == 204, "Line 651 should have a brightness of 204. It is " + averageBrightness.GetBrightnessOfLine(651));
        }

        [Test]
        public void TestWithInclusionsIncluded()
        {
            Bitmap image = Resources.ExcludeInclusionsTestImage;
            
            Inclusion inclusion1 = new Inclusion(720, 1);
            inclusion1.AddPoint(new Point(130, 455));
            inclusion1.AddPoint(new Point(128, 441));
            inclusion1.AddPoint(new Point(131, 429));
            inclusion1.AddPoint(new Point(131, 417));
            inclusion1.AddPoint(new Point(125, 410));
            inclusion1.AddPoint(new Point(112, 407));
            inclusion1.AddPoint(new Point(101, 409));
            inclusion1.AddPoint(new Point(93, 413));
            inclusion1.AddPoint(new Point(87, 420));
            inclusion1.AddPoint(new Point(78, 422));
            inclusion1.AddPoint(new Point(73, 430));
            inclusion1.AddPoint(new Point(76, 440));
            inclusion1.AddPoint(new Point(80, 447));
            inclusion1.AddPoint(new Point(85, 456));
            inclusion1.AddPoint(new Point(93, 457));
            inclusion1.AddPoint(new Point(102, 458));
            inclusion1.AddPoint(new Point(114, 461));

            Inclusion inclusion2 = new Inclusion(720, 1);
            inclusion2.AddPoint(new Point(365, 458));
            inclusion2.AddPoint(new Point(372, 448));
            inclusion2.AddPoint(new Point(388, 443));
            inclusion2.AddPoint(new Point(398, 427));
            inclusion2.AddPoint(new Point(409, 419));
            inclusion2.AddPoint(new Point(431, 374));
            inclusion2.AddPoint(new Point(443, 372));
            inclusion2.AddPoint(new Point(452, 362));
            inclusion2.AddPoint(new Point(473, 363));
            inclusion2.AddPoint(new Point(482, 357));
            inclusion2.AddPoint(new Point(552, 359));
            inclusion2.AddPoint(new Point(569, 375));
            inclusion2.AddPoint(new Point(589, 396));
            inclusion2.AddPoint(new Point(612, 437));
            inclusion2.AddPoint(new Point(617, 459));
            inclusion2.AddPoint(new Point(616, 495));
            inclusion2.AddPoint(new Point(599, 514));
            inclusion2.AddPoint(new Point(578, 526));
            inclusion2.AddPoint(new Point(548, 523));
            inclusion2.AddPoint(new Point(541, 530));
            inclusion2.AddPoint(new Point(528, 530));
            inclusion2.AddPoint(new Point(515, 541));
            inclusion2.AddPoint(new Point(413, 541));
            inclusion2.AddPoint(new Point(379, 494));

            List<Inclusion> inclusions = new List<Inclusion>();
            inclusions.Add(inclusion1);
            inclusions.Add(inclusion2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllInclusions(inclusions);
            averageBrightness.AddInclusionsToIncludeList(inclusions);

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(150) == 212, "Line 150 should have a brightness of 212. It is " + averageBrightness.GetBrightnessOfLine(150));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(430) == 176, "Line 430 should have a brightness of 176. It is " + averageBrightness.GetBrightnessOfLine(430));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(651) == 204, "Line 651 should have a brightness of 204. It is " + averageBrightness.GetBrightnessOfLine(651));
        }

        [Test]
        public void TestHeight()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            AverageBrightness averageBrightness = new AverageBrightness();

            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBoreholeHeight() == image.Height, "Borehole height should be " + image.Height + ". It is " + averageBrightness.GetBoreholeHeight());
        }

        [Test]
        public void TestWidth()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.ProcessSection(image, 0);

            Assert.IsTrue(averageBrightness.GetBoreholeWidth() == image.Width, "Borehole width should be " + image.Width + ". It is " + averageBrightness.GetBoreholeWidth());
        }

        [Test]
        public void TestSamplingRate()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetSamplingRate(20);
            averageBrightness.ProcessSection(image, 0);

            int rowsProcessed = averageBrightness.GetAllAverageBrightnesses().Count;

            Assert.IsTrue(rowsProcessed == image.Height / 20, "Rows processed should be " + image.Height / 20 + ". It is " + rowsProcessed);

            //5
            averageBrightness = new AverageBrightness();
            averageBrightness.SetSamplingRate(5);
            averageBrightness.ProcessSection(image, 0);

            rowsProcessed = averageBrightness.GetAllAverageBrightnesses().Count;

            Assert.IsTrue(rowsProcessed == image.Height / 5, "Rows processed should be " + image.Height / 5 + ". It is " + rowsProcessed);

        }

        [Test]
        public void TestDepthResolution()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetSamplingRate(20);
            averageBrightness.SetDepthResolution(2);
            averageBrightness.ProcessSection(image, 0);

            int rowsProcessed = averageBrightness.GetAllAverageBrightnesses().Count;

            Assert.IsTrue(rowsProcessed == image.Height / (20 / 2), "Rows processed should be " + image.Height / (20 / 2) + ". It is " + rowsProcessed);
        }

        [Test]
        public void TestSectionStart()
        {
            Bitmap image = Resources.ExcludeInclusionsTestImage;
            
            Inclusion inclusion1 = new Inclusion(720, 1);
            inclusion1.AddPoint(new Point(130, 555));
            inclusion1.AddPoint(new Point(128, 541));
            inclusion1.AddPoint(new Point(131, 529));
            inclusion1.AddPoint(new Point(131, 517));
            inclusion1.AddPoint(new Point(125, 510));
            inclusion1.AddPoint(new Point(112, 507));
            inclusion1.AddPoint(new Point(101, 509));
            inclusion1.AddPoint(new Point(93, 513));
            inclusion1.AddPoint(new Point(87, 520));
            inclusion1.AddPoint(new Point(78, 522));
            inclusion1.AddPoint(new Point(73, 530));
            inclusion1.AddPoint(new Point(76, 540));
            inclusion1.AddPoint(new Point(80, 547));
            inclusion1.AddPoint(new Point(85, 556));
            inclusion1.AddPoint(new Point(93, 557));
            inclusion1.AddPoint(new Point(102, 558));
            inclusion1.AddPoint(new Point(114, 561));

            Inclusion inclusion2 = new Inclusion(720, 1);
            inclusion2.AddPoint(new Point(365, 558));
            inclusion2.AddPoint(new Point(372, 548));
            inclusion2.AddPoint(new Point(388, 543));
            inclusion2.AddPoint(new Point(398, 527));
            inclusion2.AddPoint(new Point(409, 519));
            inclusion2.AddPoint(new Point(431, 474));
            inclusion2.AddPoint(new Point(443, 472));
            inclusion2.AddPoint(new Point(452, 462));
            inclusion2.AddPoint(new Point(473, 463));
            inclusion2.AddPoint(new Point(482, 457));
            inclusion2.AddPoint(new Point(552, 459));
            inclusion2.AddPoint(new Point(569, 475));
            inclusion2.AddPoint(new Point(589, 496));
            inclusion2.AddPoint(new Point(612, 537));
            inclusion2.AddPoint(new Point(617, 559));
            inclusion2.AddPoint(new Point(616, 595));
            inclusion2.AddPoint(new Point(599, 614));
            inclusion2.AddPoint(new Point(578, 626));
            inclusion2.AddPoint(new Point(548, 623));
            inclusion2.AddPoint(new Point(541, 630));
            inclusion2.AddPoint(new Point(528, 630));
            inclusion2.AddPoint(new Point(515, 641));
            inclusion2.AddPoint(new Point(413, 641));
            inclusion2.AddPoint(new Point(379, 694));

            List<Inclusion> inclusions = new List<Inclusion>();
            inclusions.Add(inclusion1);
            inclusions.Add(inclusion2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllInclusions(inclusions);

            averageBrightness.ProcessSection(image, 100);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(150) == 212, "Line 150 should have a brightness of 212. It is " + averageBrightness.GetBrightnessOfLine(150));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(430) == 207, "Line 430 should have a brightness of 207. It is " + averageBrightness.GetBrightnessOfLine(430));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLine(651) == 204, "Line 651 should have a brightness of 204. It is " + averageBrightness.GetBrightnessOfLine(651));
        }

        [Test]
        public void TestGetBrightnessOfLayer()
        {
            Bitmap image = Resources.ExcludeLayersTestImage;
            
            LayerTypeSelector layerTypeSelector = new LayerTypeSelector("Borehole");

            Layer layer1 = new BoreholeLayer(190, 28, 122, 429, 28, 122, 720, 1);
            Layer layer2 = new BoreholeLayer(524, 4, 100, 559, 4, 100, 720, 1);

            List<Layer> layers = new List<Layer>();
            layers.Add(layer1);
            layers.Add(layer2);

            AverageBrightness averageBrightness = new AverageBrightness();
            averageBrightness.SetAllLayers(layers);
            averageBrightness.SetCurrentSectionImage(image);

            Assert.IsTrue(averageBrightness.GetBrightnessOfLayer(layer1) == 116, "Layer 1's brightness should be 116. It is " + averageBrightness.GetBrightnessOfLayer(layer1));
            Assert.IsTrue(averageBrightness.GetBrightnessOfLayer(layer2) == 108, "Layer 2's brightness should be 108. It is " + averageBrightness.GetBrightnessOfLayer(layer2));
        }
    }
}

