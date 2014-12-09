using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using BoreholeFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaveLoadBoreholeData;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class SaveLoadDataTests
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        private SaveLoadData saveLoadData;

        [TestMethod]
        public void TestCreateDirectories()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            Assert.IsFalse(Directory.Exists(projectLocation), "projectLocation should not exist");
            Assert.IsFalse(Directory.Exists(projectLocation + "\\source"), "projectLocation'\'source should not exist");
            Assert.IsFalse(Directory.Exists(projectLocation + "\\features"), "projectLocation\features should not exist");
            Assert.IsFalse(Directory.Exists(projectLocation + "\\data"), "projectLocation'\'data should not exist");

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            Assert.IsTrue(Directory.Exists(projectLocation), "projectLocation should exist");
            Assert.IsTrue(Directory.Exists(projectLocation + "\\source"), "projectLocation'\'source should not exist");
            Assert.IsTrue(Directory.Exists(projectLocation + "\\features"), "projectLocation\features should not exist");
            Assert.IsTrue(Directory.Exists(projectLocation + "\\data"), "projectLocation'\'data should not exist");

            deleteDirectory();
        }

        [TestMethod]
        public void TestCopySourceFile()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            File.Create(testRootFolder + "\\testSourceFile");

            Assert.IsTrue(File.Exists(testRootFolder + "\\testSourceFile"), "testSourceFile should exist");
            Assert.IsFalse(File.Exists(testRootFolder + "\\projectLocation\\source\\testSourceFile"), "testSourceFile should not be in the source folder");

            saveLoadData.copySourceFile(testRootFolder + "\\testSourceFile");

            Assert.IsTrue(File.Exists(testRootFolder + "\\testSourceFile"), "testSourceFile should exist");
            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\source\\testSourceFile"), "testSourceFile should be in the source folder");

            deleteDirectory();
            File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\testSourceFile");
        }

        [TestMethod]
        public void TestSaveBoreholeDetails()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            string boreholeDetails = createTestBoreholeDetails();

            Assert.IsTrue(boreholeDetails != null, "BoreholeDetails should not be null");

            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\testProject.features"), "Features File should exist");

            System.IO.StreamReader file = new System.IO.StreamReader(projectLocation + "\\testProject.features");
            string line;

            line = file.ReadLine();
            Assert.IsTrue(line != null, "line should not be null");
            Assert.IsTrue(line.Equals("Borehole test name"), "First line should be 'Borehole test name'");
            line = file.ReadLine();
            Assert.IsTrue(line.Equals("720"), "Second line should be '720'");
            line = file.ReadLine();
            Assert.IsTrue(line.Equals("340"), "Third line should be '340'");
            line = file.ReadLine();
            Assert.IsTrue(line.Equals("1000"), "Third line should be '1000'");
            line = file.ReadLine();
            Assert.IsTrue(line.Equals("1340"), "Third line should be '1340'");
            line = file.ReadLine();
            Assert.IsTrue(line.Equals("2"), "Third line should be '2'");

            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();
            deleteDirectory();
        }

        [TestMethod]
        public void TestSaveLayers()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            List<Layer> layers = createTestLayers();

            Assert.IsTrue(layers != null, "layers should not be null");

            saveLoadData.saveLayers(layers);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\layers"), "Layer file should exist");

            Assert.IsTrue(layers.Count == 3, "There should be 3 layers");

            System.IO.StreamReader file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\layers");
            string line;

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(layers[0].GetDetails()), "First line should be same as first layer. First Layer: " + layers[0].GetDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(layers[1].GetDetails()), "second line should be same as second layer. Second Layer: " + layers[1].GetDetails() + " - Second line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(layers[2].GetDetails()), "Third line should be same as third layer. Third Layer: " + layers[2].GetDetails() + " - Third line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            layers.RemoveAt(2);

            saveLoadData.saveLayers(layers);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\layers"), "Layer file should exist");

            Assert.IsTrue(layers.Count == 2, "There should be 2 layers");

            file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\layers");

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(layers[0].GetDetails()), "First line should be same as first layer. First Layer: " + layers[0].GetDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(layers[1].GetDetails()), "second line should be same as second layer. Second Layer: " + layers[1].GetDetails() + " - Second line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            deleteDirectory();
        }

        private List<Layer> createTestLayers()
        {
            List<Layer> testLayers = new List<Layer>();

            LayerTypeSelector selector = new LayerTypeSelector("Borehole");
            Layer layer1 = selector.setUpLayer(100, 23, 320, 110, 23, 110, 720, 1);
            testLayers.Add(layer1);

            Layer layer2 = selector.setUpLayer(300, 10, 200, 323, 2, 210, 720, 1);
            layer2.Description = "Layer 2";
            layer2.SetQuality(3);
            layer2.CoarseDebris = true;
            testLayers.Add(layer2);

            Layer layer3 = selector.setUpLayer(1, 2, 3, 4, 5, 6, 7, 8);
            testLayers.Add(layer3);

            return testLayers;
        }

        [TestMethod]
        public void TestSaveClusters()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            List<Cluster> clusters = createTestClusters();

            saveLoadData.saveClusters(clusters);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\clusters"), "Cluster file should exist");

            Assert.IsTrue(clusters.Count == 3, "There should be 3 clusters");

            System.IO.StreamReader file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\clusters");
            string line;

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(clusters[0].GetDetails()), "First line should be same as first cluster. First cluster: " + clusters[0].GetDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(clusters[1].GetDetails()), "second line should be same as second cluster. Second cluster: " + clusters[1].GetDetails() + " - Second line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(clusters[2].GetDetails()), "Third line should be same as third cluster. Third cluster: " + clusters[2].GetDetails() + " - Third line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            clusters.RemoveAt(2);

            saveLoadData.saveClusters(clusters);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\clusters"), "cluster file should exist");

            Assert.IsTrue(clusters.Count == 2, "There should be 2 clusters");

            file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\clusters");

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(clusters[0].GetDetails()), "First line should be same as first cluster. First cluster: " + clusters[0].GetDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(clusters[1].GetDetails()), "second line should be same as second cluster. Second cluster: " + clusters[1].GetDetails() + " - Second line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            deleteDirectory();
        }

        [TestMethod]
        public void TestSaveInclusions()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");

            saveLoadData.createDirectories();

            List<Inclusion> inclusions = createTestInclusions();

            saveLoadData.saveInclusions(inclusions);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\inclusions"), "inclusion file should exist");

            Assert.IsTrue(inclusions.Count == 3, "There should be 3 inclusions");

            System.IO.StreamReader file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\inclusions");
            string line;

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(inclusions[0].getDetails()), "First line should be same as first inclusion. First inclusion: " + inclusions[0].getDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(inclusions[1].getDetails()), "second line should be same as second inclusion. Second inclusion: " + inclusions[1].getDetails() + " - Second line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(inclusions[2].getDetails()), "Third line should be same as third inclusion. Third inclusion: " + inclusions[2].getDetails() + " - Third line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            inclusions.RemoveAt(2);

            saveLoadData.saveInclusions(inclusions);

            Assert.IsTrue(File.Exists(testRootFolder + "\\projectLocation\\features\\inclusions"), "inclusion file should exist");

            Assert.IsTrue(inclusions.Count == 2, "There should be 2 inclusions");

            file = new System.IO.StreamReader(testRootFolder + "\\projectLocation\\features\\inclusions");

            line = file.ReadLine();
            Assert.IsTrue(line.Equals(inclusions[0].getDetails()), "First line should be same as first inclusion. First inclusion: " + inclusions[0].getDetails() + " - First line: " + line);
            line = file.ReadLine();
            Assert.IsTrue(line.Equals(inclusions[1].getDetails()), "second line should be same as second inclusion. Second inclusion: " + inclusions[1].getDetails() + " - Second line: " + line);
            Assert.IsTrue(file.ReadLine() == null, "There should be no more lines");

            file.Close();

            deleteDirectory();
        }

        private List<Inclusion> createTestInclusions()
        {
            List<Inclusion> testInclusions = new List<Inclusion>();

            Inclusion inclusion1 = new Inclusion(720, 2);
            inclusion1.AddPoint(new Point(10, 4));
            testInclusions.Add(inclusion1);

            Inclusion inclusion2 = new Inclusion(720, 1);
            inclusion2.Description = "Inclusion 2";
            inclusion2.AddPoint(new Point(2, 5));
            inclusion2.AddPoint(new Point(10, 4));
            inclusion2.AddPoint(new Point(10, 4));
            testInclusions.Add(inclusion2);

            Inclusion inclusion3 = new Inclusion(360, 2);
            inclusion3.AddPoint(new Point(100, 9));
            inclusion3.AddPoint(new Point(10, 13));
            testInclusions.Add(inclusion3);

            return testInclusions;
        }

        [TestMethod]
        public void TestGetImageDataFilePath()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();

            Assert.IsTrue(saveLoadData.getImageDataFilePath().Equals(testRootFolder + "\\projectLocation\\data\\imageData"));

            deleteDirectory();
        }

        [TestMethod]
        public void TestBoreholeName()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetBoreholeName().Equals("Borehole test name"), "Borehole name should be 'Borehole test name'. It is " + saveLoadData.GetBoreholeName());

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetBoreholeWidth()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetBoreholeWidth().Equals(720), "Borehole width should be '720'. It is " + saveLoadData.GetBoreholeWidth());

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetBoreholeHeight()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetBoreholeHeight().Equals(340), "Borehole height should be '340'. It is " + saveLoadData.GetBoreholeHeight());

            deleteDirectory();
        }

        [TestMethod]
        public void TestBoreholeStartDepth()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetBoreholeStartDepth().Equals(1000), "Borehole start depth should be '1000'. It is " + saveLoadData.GetBoreholeStartDepth());

            deleteDirectory();
        }

        [TestMethod]
        public void TestBoreholeEndDepth()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetBoreholeEndDepth() == 1340, "Borehole height should be '1340'. It is " + saveLoadData.GetBoreholeEndDepth());

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetResolution()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();
            string boreholeDetails = createTestBoreholeDetails();
            saveLoadData.saveBoreholeDetails(boreholeDetails);

            Assert.IsTrue(saveLoadData.GetDepthResolution() == 2, "Borehole height should be '2'. It is " + saveLoadData.GetDepthResolution());

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetFluidLevel()
        {
            string projectLocation = testRootFolder + "\\projectLocation";
            int fluidLevel = 120;

            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();

            saveLoadData.saveFluidLevel(fluidLevel);

            Assert.IsTrue(saveLoadData.getIsFluidLevelSet(), "isFluidLevelSet should be true");
            Assert.IsTrue(saveLoadData.getFluidLevel() == 120, "Fluid level should be 120. It is " + saveLoadData.getFluidLevel());

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetLayers()
        {
            string projectLocation = testRootFolder + "\\projectLocation";
            List<Layer> layers = new List<Layer>();
            List<Layer> returnedLayers = new List<Layer>();
            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();

            saveLoadData.saveBoreholeDetails(createTestBoreholeDetails());

            layers = createTestLayers();

            saveLoadData.saveLayers(layers);

            returnedLayers = saveLoadData.getLayers("Borehole");

            Assert.IsTrue(returnedLayers.Count == 3, "There should be 3 returned layers. There are " + returnedLayers.Count);

            Assert.IsTrue(returnedLayers[0].BottomSineAzimuth == 110, "Bottom sine azimuth should be 110. It is " + returnedLayers[0].BottomSineAzimuth);
            Assert.IsTrue(returnedLayers[0].TopEdgeDepth == 100, "Top depth should be 100. It is " + returnedLayers[0].TopEdgeDepth);

            Assert.IsTrue(returnedLayers[1].TopEdgeDepth == 300, "Top depth should be 300. It is " + returnedLayers[1].TopEdgeDepth);
            Assert.IsTrue(returnedLayers[1].Description.Equals("Layer 2"), "Layer 2 description should be 'Layer 2'. It is " + returnedLayers[1].Description);
            Assert.IsTrue(returnedLayers[1].CoarseDebris == true, "Coarse debris should be set to true");

            Assert.IsTrue(returnedLayers[2].TopEdgeDepth == 1, "Top depth should be 1. It is " + returnedLayers[2].TopEdgeDepth);

            layers.RemoveAt(1);
            saveLoadData.saveLayers(layers);
            returnedLayers = saveLoadData.getLayers("Borehole");

            Assert.IsTrue(returnedLayers.Count == 2, "There should be 2 returned layers. There are " + returnedLayers.Count);

            Assert.IsTrue(returnedLayers[0].BottomSineAzimuth == 110, "Bottom sine azimuth should be 110. It is " + returnedLayers[0].BottomSineAzimuth);
            Assert.IsTrue(returnedLayers[0].TopEdgeDepth == 100, "Top depth should be 100. It is " + returnedLayers[0].TopEdgeDepth);

            Assert.IsTrue(returnedLayers[1].TopEdgeDepth == 1, "Top depth should be 1. It is " + returnedLayers[1].TopEdgeDepth);

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetClusters()
        {
            string projectLocation = testRootFolder + "\\projectLocation";
            List<Cluster> clusters = new List<Cluster>();
            List<Cluster> returnedClusters = new List<Cluster>();
            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();

            saveLoadData.saveBoreholeDetails(createTestBoreholeDetails());

            clusters = createTestClusters();

            saveLoadData.saveClusters(clusters);

            returnedClusters = saveLoadData.GetClusters();

            Assert.IsTrue(returnedClusters.Count == 3, "There should be 3 returned clusters. There are " + returnedClusters.Count);

            Assert.IsTrue(returnedClusters[0].Description == "", "The first clusters description should be ''. It is " + returnedClusters[0].Description);
            Assert.IsTrue(returnedClusters[1].Description == "Cluster 2", "The second clusters description should be 'Cluster 2'. It is " + returnedClusters[1].Description);
            Assert.IsTrue(returnedClusters[2].Description == "", "The third clusters description should be ''. It is " + returnedClusters[2].Description);

            clusters.RemoveAt(0);
            saveLoadData.saveClusters(clusters);
            returnedClusters = saveLoadData.GetClusters();

            Assert.IsTrue(returnedClusters[0].Description == "Cluster 2", "The first clusters description should be 'Cluster 2'. It is " + returnedClusters[0].Description);
            Assert.IsTrue(returnedClusters[1].Description == "", "The second clusters description should be ''. It is " + returnedClusters[1].Description);

            deleteDirectory();
        }

        [TestMethod]
        public void TestGetInclusions()
        {
            string projectLocation = testRootFolder + "\\projectLocation";
            List<Inclusion> inclusions = new List<Inclusion>();
            List<Inclusion> returnedInclusions = new List<Inclusion>();
            clearPreviousTestFiles();

            saveLoadData = new SaveLoadData(projectLocation, "testProject");
            saveLoadData.createDirectories();

            saveLoadData.saveBoreholeDetails(createTestBoreholeDetails());

            inclusions = createTestInclusions();

            saveLoadData.saveInclusions(inclusions);

            returnedInclusions = saveLoadData.GetInclusions();

            Assert.IsTrue(returnedInclusions.Count == 3, "There should be 3 returned inclusions. There are " + returnedInclusions.Count);

            Assert.IsTrue(returnedInclusions[0].Description == "", "The first inclusion description should be ''. It is " + returnedInclusions[0].Description);
            Assert.IsTrue(returnedInclusions[1].Description == "Inclusion 2", "The second inclusion description should be 'Inclusion 2'. It is " + returnedInclusions[1].Description);
            Assert.IsTrue(returnedInclusions[2].Description == "", "The third inclusion description should be ''. It is " + returnedInclusions[2].Description);

            inclusions.RemoveAt(0);
            saveLoadData.saveInclusions(inclusions);
            returnedInclusions = saveLoadData.GetInclusions();

            Assert.IsTrue(returnedInclusions[0].Description == "Inclusion 2", "The first inclusion description should be 'Inclusion 2'. It is " + returnedInclusions[0].Description);
            Assert.IsTrue(returnedInclusions[1].Description == "", "The second inclusion description should be ''. It is " + returnedInclusions[1].Description);

            deleteDirectory();
        }

        private string createTestBoreholeDetails()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Borehole test name");
            stringBuilder.AppendLine();
            stringBuilder.Append("720");
            stringBuilder.AppendLine();
            stringBuilder.Append("340");
            stringBuilder.AppendLine();
            stringBuilder.Append("1000");
            stringBuilder.AppendLine();
            stringBuilder.Append("1340");
            stringBuilder.AppendLine();
            stringBuilder.Append("2");
            stringBuilder.AppendLine();

            string boreholeDetails = stringBuilder.ToString();

            return boreholeDetails;
        }

        private void clearPreviousTestFiles()
        {
            GC.Collect();

            string projectLocation = testRootFolder + "\\projectLocation";

            if (Directory.Exists(projectLocation))
            {
                deleteDirectory();
            }

            GC.Collect();
        }

        private void deleteDirectory()
        {
            string projectLocation = testRootFolder + "\\projectLocation";

            try
            {
                if (Directory.Exists(projectLocation))
                    Directory.Delete(projectLocation, true);
            }
            catch (Exception e)
            { }
        }

        private List<Cluster> createTestClusters()
        {
            List<Cluster> testClusters = new List<Cluster>();

            Cluster cluster1 = new Cluster(720, 2);
            cluster1.AddPoint(new Point(1, 1));
            cluster1.AddPoint(new Point(2, 1));
            cluster1.AddPoint(new Point(2, 2));
            cluster1.IsComplete = true;
            testClusters.Add(cluster1);

            Cluster cluster2 = new Cluster(720, 1);
            cluster2.Description = "Cluster 2";
            cluster2.AddPoint(new Point(2, 5));
            cluster2.AddPoint(new Point(10, 4));
            cluster2.AddPoint(new Point(11, 4));
            cluster2.CoarseDebris = true;
            cluster2.CoarseDebris = true;
            cluster2.IsComplete = true;
            testClusters.Add(cluster2);

            Cluster cluster3 = new Cluster(360, 2);
            cluster3.AddPoint(new Point(100, 9));
            cluster3.AddPoint(new Point(10, 13));
            cluster3.AddPoint(new Point(10, 10));
            cluster3.Diamicton = true;
            cluster3.FineDebris = true;
            cluster2.IsComplete = true;
            testClusters.Add(cluster3);

            return testClusters;
        }

    }
}

