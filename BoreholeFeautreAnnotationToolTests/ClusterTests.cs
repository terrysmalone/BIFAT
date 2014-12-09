using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoreholeFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class ClusterTests
    {
        Cluster testCluster;

        [TestMethod]
        public void TestConstructor()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.BottomYBoundary == 0, "Bottom Y boundary should be 0. It is " + testCluster.BottomYBoundary);
            Assert.IsTrue(testCluster.TopYBoundary == 0, "Top Y boundary should be 0. It is " + testCluster.TopYBoundary);
            Assert.IsTrue(testCluster.LeftXBoundary == 0, "Left X boundary should be 0. It is " + testCluster.LeftXBoundary);
            Assert.IsTrue(testCluster.RightXBoundary == 0, "Right X boundary should be 0. It is " + testCluster.RightXBoundary);

            Assert.IsTrue(testCluster.IsComplete == false, "isComplete should be false. It is " + testCluster.IsComplete);
            Assert.IsTrue(testCluster.Points != null, "points list should not be null.  It should be an empty list. It is " + testCluster.Points);
            Assert.IsTrue(testCluster.Points.Count == 0, "points list count should be 0. It is " + testCluster.Points.Count);

            Assert.IsTrue(testCluster.PointsString == "", "points string should be ''. It is " + testCluster.PointsString);

            Assert.IsTrue(testCluster.StartDepth == 0, "Start Depth should be 0. It is " + testCluster.StartDepth);
            Assert.IsTrue(testCluster.EndDepth == 0, "End Depth should be 0. It is " + testCluster.EndDepth);

            Assert.IsTrue(testCluster.ClusterType == "", "Type should be ''. It is " + testCluster.ClusterType);

        }

        [TestMethod]
        public void TestLeftXBoundary()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.LeftXBoundary == 0, "Left X boundary should be 0. It is " + testCluster.LeftXBoundary);

            testCluster.AddPoint(new Point(13, 60));
            Assert.IsTrue(testCluster.LeftXBoundary == 13, "Left X boundary should be 13. It is " + testCluster.LeftXBoundary);

            testCluster.AddPoint(new Point(45, 3));
            Assert.IsTrue(testCluster.LeftXBoundary == 13, "Left X boundary should be 13. It is " + testCluster.LeftXBoundary);

            testCluster.AddPoint(new Point(12, 1));
            Assert.IsTrue(testCluster.LeftXBoundary == 12, "Left X boundary should be 12. It is " + testCluster.LeftXBoundary);

        }

        [TestMethod]
        public void TestRightXBoundary()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.RightXBoundary == 0, "Right X boundary should be 0. It is " + testCluster.RightXBoundary);

            testCluster.AddPoint(new Point(13, 60));
            Assert.IsTrue(testCluster.RightXBoundary == 13, "Right X boundary should be 13. It is " + testCluster.RightXBoundary);

            testCluster.AddPoint(new Point(45, 3));
            Assert.IsTrue(testCluster.RightXBoundary == 45, "Right X boundary should be 45. It is " + testCluster.RightXBoundary);

            testCluster.AddPoint(new Point(12, 1));
            Assert.IsTrue(testCluster.RightXBoundary == 45, "Right X boundary should be 45. It is " + testCluster.RightXBoundary);
        }

        [TestMethod]
        public void TestTopYBoundary()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.TopYBoundary == 0, "Top Y boundary should be 0. It is " + testCluster.TopYBoundary);

            testCluster.AddPoint(new Point(13, 60));
            Assert.IsTrue(testCluster.TopYBoundary == 60, "Top Y boundary should be 60. It is " + testCluster.TopYBoundary);

            testCluster.AddPoint(new Point(45, 3));
            Assert.IsTrue(testCluster.TopYBoundary == 3, "Top Y boundary should be 3. It is " + testCluster.TopYBoundary);

            testCluster.AddPoint(new Point(12, 4));
            Assert.IsTrue(testCluster.TopYBoundary == 3, "Top Y boundary should be 3. It is " + testCluster.TopYBoundary);

            testCluster.AddPoint(new Point(1, 2));
            Assert.IsTrue(testCluster.TopYBoundary == 2, "Top Y boundary should be 2. It is " + testCluster.TopYBoundary);
        }

        [TestMethod]
        public void TestBottomYBoundary()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.BottomYBoundary == 0, "Bottom Y boundary should be 0. It is " + testCluster.BottomYBoundary);

            testCluster.AddPoint(new Point(13, 60));
            Assert.IsTrue(testCluster.BottomYBoundary == 60, "Bottom Y boundary should be 60. It is " + testCluster.BottomYBoundary);

            testCluster.AddPoint(new Point(45, 3));
            Assert.IsTrue(testCluster.BottomYBoundary == 60, "Bottom Y boundary should be 60. It is " + testCluster.BottomYBoundary);

            testCluster.AddPoint(new Point(12, 76));
            Assert.IsTrue(testCluster.BottomYBoundary == 76, "Bottom Y boundary should be 76. It is " + testCluster.BottomYBoundary);

            testCluster.AddPoint(new Point(1, 2));
            Assert.IsTrue(testCluster.BottomYBoundary == 76, "Bottom Y boundary should be 76. It is " + testCluster.BottomYBoundary);
        }

        [TestMethod]
        public void TestIsComplete()
        {
            testCluster = new Cluster(720, 2);

            Assert.IsTrue(testCluster.IsComplete == false, "isComplete should be false. It is " + testCluster.IsComplete);

            testCluster.IsComplete = true;
            Assert.IsTrue(testCluster.IsComplete == true, "isComplete should be true. It is " + testCluster.IsComplete);

            testCluster.IsComplete = false;
            Assert.IsTrue(testCluster.IsComplete == false, "isComplete should be false. It is " + testCluster.IsComplete);
        }

        [TestMethod]
        public void TestAddPoint()
        {
            testCluster = new Cluster(720, 2);
            Assert.IsTrue(testCluster.Points != null, "points list should be null. It is " + testCluster.Points);
            Assert.IsTrue(testCluster.Points.Count == 0, "points list count should be 0. It is " + testCluster.Points.Count);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.IsTrue(testCluster.Points.Count == 2, "points list count should be 2. It is " + testCluster.Points.Count);
            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testCluster.Points[1]);

            testCluster.AddPoint(new Point(4, 12));

            Assert.IsTrue(testCluster.Points.Count == 3, "points list count should be 3. It is " + testCluster.Points.Count);
            Assert.IsTrue(testCluster.Points[2] == new Point(4, 12), "Second point should be 4,12. It is " + testCluster.Points[2]);
        }

        [TestMethod]
        public void TestPointsString()
        {
            testCluster = new Cluster(720, 2);
            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.IsTrue(testCluster.PointsString.Equals("(12, 8) (32, 1) "), "Points string should be '(12, 8) (32, 1)'.  It is " + testCluster.PointsString);

            testCluster.AddPoint(new Point(100, 11));

            Assert.IsTrue(testCluster.PointsString.Equals("(12, 8) (32, 1) (100, 11) "), "Points string should be '(12, 8) (32, 1) (100, 11)'.  It is " + testCluster.PointsString);
        }

        [TestMethod]
        public void TestMovePoint()
        {
            testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));
            testCluster.AddPoint(new Point(100, 10));

            testCluster.MovePoint(new Point(100, 10), new Point(110, 7));

            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8));
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1));
            Assert.IsFalse(testCluster.Points[2] == new Point(100, 10));
            Assert.IsTrue(testCluster.Points[2] == new Point(110, 7));

            testCluster.MovePoint(new Point(100, 10), new Point(200, 20));

            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8));
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1));
            Assert.IsFalse(testCluster.Points[2] == new Point(100, 10));
            Assert.IsTrue(testCluster.Points[2] == new Point(110, 7));
        }

        [TestMethod]
        public void TestRemovePoint()
        {
            testCluster = new Cluster(720, 2);
            Assert.IsTrue(testCluster.Points != null, "points list should be null. It is " + testCluster.Points);
            Assert.IsTrue(testCluster.Points.Count == 0, "points list count should be 0. It is " + testCluster.Points.Count);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.IsTrue(testCluster.Points.Count == 2, "points list count should be 2. It is " + testCluster.Points.Count);

            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testCluster.Points[1]);

            testCluster.RemovePoint(new Point(12, 8));

            Assert.IsTrue(testCluster.Points.Count == 1, "points list count should be 1. It is " + testCluster.Points.Count);
            Assert.IsTrue(testCluster.Points[0] == new Point(32, 1), "First point should be 32,1. It is " + testCluster.Points[0]);

        }

        [TestMethod]
        public void TestRemoveAddPoint()
        {
            testCluster = new Cluster(720, 2);
            Assert.IsTrue(testCluster.Points != null, "points list should be null. It is " + testCluster.Points);
            Assert.IsTrue(testCluster.Points.Count == 0, "points list count should be 0. It is " + testCluster.Points.Count);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.IsTrue(testCluster.Points.Count == 2, "points list count should be 2. It is " + testCluster.Points.Count);

            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testCluster.Points[1]);

            testCluster.RemovePoint(new Point(12, 8));

            Assert.IsTrue(testCluster.Points.Count == 1, "points list count should be 1. It is " + testCluster.Points.Count);
            Assert.IsTrue(testCluster.Points[0] == new Point(32, 1), "First point should be 32,1. It is " + testCluster.Points[0]);

            testCluster.AddPoint(new Point(4, 12));

            Assert.IsTrue(testCluster.Points.Count == 2, "points list count should be 3. It is " + testCluster.Points.Count);
            Assert.IsTrue(testCluster.Points[0] == new Point(32, 1), "First point should be 32,1. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(4, 12), "Second point should be 4,12. It is " + testCluster.Points[1]);

            testCluster.RemovePoint(new Point(4, 12));
            testCluster.RemovePoint(new Point(32, 1));

            Assert.IsTrue(testCluster.Points.Count == 0, "points list count should be 0. It is " + testCluster.Points.Count);
        }

        [TestMethod]
        public void TestMoveCluster()
        {
            testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));
            testCluster.AddPoint(new Point(100, 10));

            Assert.IsTrue(testCluster.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testCluster.Points[1]);
            Assert.IsTrue(testCluster.Points[2] == new Point(100, 10), "Second point should be 100,10. It is " + testCluster.Points[2]);

            testCluster.MoveCluster(10, 0);

            Assert.IsTrue(testCluster.Points[0] == new Point(22, 8), "First point should be 22,8. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(42, 1), "Second point should be 42,1. It is " + testCluster.Points[1]);
            Assert.IsTrue(testCluster.Points[2] == new Point(110, 10), "Second point should be 110,10. It is " + testCluster.Points[2]);

            testCluster.MoveCluster(20, -10);

            Assert.IsTrue(testCluster.Points[0] == new Point(42, -2), "First point should be 42, -2. It is " + testCluster.Points[0]);
            Assert.IsTrue(testCluster.Points[1] == new Point(62, -9), "Second point should be 62, -9. It is " + testCluster.Points[1]);
            Assert.IsTrue(testCluster.Points[2] == new Point(130, 0), "Second point should be 130, 0. It is " + testCluster.Points[2]);
        }

        [TestMethod]
        public void TestStartDepth1()
        {
            testCluster = new Cluster(720, 1);

            testCluster.AddPoint(new Point(12, 8));
            Assert.IsTrue(testCluster.StartDepth == 8, "Start depth should be 8mm.  It is " + testCluster.StartDepth);
            testCluster.AddPoint(new Point(32, 2));
            Assert.IsTrue(testCluster.StartDepth == 2, "Start depth should be 2mm.  It is " + testCluster.StartDepth);
            testCluster.AddPoint(new Point(100, 10));
            Assert.IsTrue(testCluster.StartDepth == 2, "Start depth should be 2mm.  It is " + testCluster.StartDepth);

            testCluster.SourceStartDepth = 100;
            Assert.IsTrue(testCluster.StartDepth == 102, "Start depth should be 102mm.  It is " + testCluster.StartDepth);

            testCluster.AddPoint(new Point(43, 1));

            Assert.IsTrue(testCluster.StartDepth == 101, "Start depth should be 101mm.  It is " + testCluster.StartDepth);
        }

        [TestMethod]
        public void TestStartDepth2()
        {
            testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));

            Assert.IsTrue(testCluster.StartDepth == 16, "Start depth should be 16mm.  It is " + testCluster.StartDepth);
            testCluster.AddPoint(new Point(32, 2));
            Assert.IsTrue(testCluster.StartDepth == 4, "Start depth should be 4mm.  It is " + testCluster.StartDepth);
            testCluster.AddPoint(new Point(100, 10));
            Assert.IsTrue(testCluster.StartDepth == 4, "Start depth should be 4mm.  It is " + testCluster.StartDepth);

            testCluster.SourceStartDepth = 100;
            Assert.IsTrue(testCluster.StartDepth == 104, "Start depth should be 104mm.  It is " + testCluster.StartDepth);

            testCluster.AddPoint(new Point(43, 1));

            Assert.IsTrue(testCluster.StartDepth == 102, "Start depth should be 102mm.  It is " + testCluster.StartDepth);
        }

        [TestMethod]
        public void TestEndDepth1()
        {
            testCluster = new Cluster(720, 1);

            testCluster.AddPoint(new Point(12, 8));

            Assert.IsTrue(testCluster.EndDepth == 8, "End depth should be 8mm.  It is " + testCluster.EndDepth);
            testCluster.AddPoint(new Point(32, 2));
            Assert.IsTrue(testCluster.EndDepth == 8, "End depth should be 8mm.  It is " + testCluster.EndDepth);
            testCluster.AddPoint(new Point(100, 10));
            Assert.IsTrue(testCluster.EndDepth == 10, "End depth should be 10mm.  It is " + testCluster.EndDepth);

            testCluster.SourceStartDepth = 100;
            Assert.IsTrue(testCluster.EndDepth == 110, "End depth should be 110mm.  It is " + testCluster.EndDepth);

            testCluster.AddPoint(new Point(43, 100));

            Assert.IsTrue(testCluster.EndDepth == 200, "End depth should be 200mm.  It is " + testCluster.EndDepth);
        }

        [TestMethod]
        public void TestEndDepth2()
        {
            testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));

            Assert.IsTrue(testCluster.EndDepth == 16, "End depth should be 16mm.  It is " + testCluster.EndDepth);
            testCluster.AddPoint(new Point(32, 2));
            Assert.IsTrue(testCluster.EndDepth == 16, "End depth should be 16mm.  It is " + testCluster.EndDepth);
            testCluster.AddPoint(new Point(100, 10));
            Assert.IsTrue(testCluster.EndDepth == 20, "End depth should be 20mm.  It is " + testCluster.EndDepth);

            testCluster.SourceStartDepth = 100;
            Assert.IsTrue(testCluster.EndDepth == 120, "End depth should be 120mm.  It is " + testCluster.EndDepth);

            testCluster.AddPoint(new Point(43, 100));

            Assert.IsTrue(testCluster.EndDepth == 300, "End depth should be 300mm.  It is " + testCluster.EndDepth);
        }

        [TestMethod]
        public void TestType()
        {
            testCluster = new Cluster(720, 2);
            testCluster.LargeBubbles = true;
            testCluster.FineDebris = true;

            Assert.IsTrue(testCluster.ClusterType.Contains("largeBubbles"), "Cluster should contain 'largeBubbles'. It is " + testCluster.ClusterType);
            Assert.IsTrue(testCluster.ClusterType.Contains("fineDebris"), "Cluster should contain 'fineDebris'. It is " + testCluster.ClusterType);

            testCluster.FineDebris = false;
            testCluster.Diamicton = true;

            Assert.IsTrue(testCluster.ClusterType.Contains("largeBubbles"), "Cluster should contain 'largeBubbles'. It is " + testCluster.ClusterType);
            Assert.IsFalse(testCluster.ClusterType.Contains("fineDebris"), "Cluster should not contain 'fineDebris'. It is " + testCluster.ClusterType);
            Assert.IsTrue(testCluster.ClusterType.Contains("diamicton"), "Cluster should contain 'diamicton'. It is " + testCluster.ClusterType);
        }

        [TestMethod]
        public void TestDetails()
        {
            DateTime time = DateTime.Now;

            testCluster = new Cluster(720, 2);
            testCluster.LargeBubbles = true;
            testCluster.FineDebris = true;

            testCluster.AddPoint(new Point(10, 20));
            testCluster.AddPoint(new Point(11, 15));

            testCluster.Description = "Cluster, description";
            testCluster.Group = "group1";
            testCluster.TimeAdded = time;
            testCluster.TimeLastModified = time;


            Assert.IsTrue(testCluster.GetDetails().Equals("15,20,10,11,10 20 11 15 ,largeBubbles fineDebris ,Cluster  description," + time + "," + time + ",group1"), "Details is wrong." + testCluster.GetDetails());
        }

        [TestMethod]
        public void TestDescription()
        {
            testCluster = new Cluster(720, 2);

            testCluster.Description = "A cluster";
            Assert.IsTrue(testCluster.Description.Equals("A cluster"), "Description should be 'A cluster'.  It is " + testCluster.Description);
        }

        [TestMethod]
        public void TestTimeAdded()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            testCluster = new Cluster(720, 1);

            DateTime timeAdded = Convert.ToDateTime(testCluster.TimeAdded, dtfi);

            timeAdded = timeAdded.AddDays(4.0);

            Cluster addedCluster = new Cluster(720, 1);

            Assert.IsFalse(addedCluster.TimeAdded.Equals(timeAdded), "Clusters times should not match. They are " + addedCluster.TimeAdded.ToString() + " and " + timeAdded.ToString());

            addedCluster.TimeAdded = timeAdded;

            Assert.IsTrue(addedCluster.TimeAdded.Equals(timeAdded), "Clusters times should match. They are " + addedCluster.TimeAdded.ToString() + " and " + timeAdded.ToString());
        }

        [TestMethod]
        public void TestTimeLastModified()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            testCluster = new Cluster(720, 1);

            DateTime timeModified = Convert.ToDateTime(testCluster.TimeAdded, dtfi);

            timeModified = timeModified.AddDays(4.0);

            Cluster addedCluster = new Cluster(720, 1);

            Assert.IsFalse(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should not match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedCluster.TimeLastModified = timeModified;

            Assert.IsTrue(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedCluster.AddPoint(new Point(10, 10));

            Assert.IsFalse(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should not match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());
        }
    }
}