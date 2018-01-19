using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoreholeFeatures;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestFixture]
    public class InclusionTests
    {
        Inclusion testInclusion;

        [Test]
        public void TestConstructor()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.BottomYBoundary == 0, "Bottom Y boundary should be 0. It is " + testInclusion.BottomYBoundary);
            Assert.IsTrue(testInclusion.TopYBoundary == 0, "Top Y boundary should be 0. It is " + testInclusion.TopYBoundary);
            Assert.IsTrue(testInclusion.LeftXBoundary == 0, "Left X boundary should be 0. It is " + testInclusion.LeftXBoundary);
            Assert.IsTrue(testInclusion.RightXBoundary == 0, "Right X boundary should be 0. It is " + testInclusion.RightXBoundary);

            Assert.IsTrue(testInclusion.IsComplete == false, "isComplete should be false. It is " + testInclusion.IsComplete);
            Assert.IsTrue(testInclusion.Points != null, "points list should be null. It is " + testInclusion.Points);
            Assert.IsTrue(testInclusion.Points.Count == 0, "points list count should be 0. It is " + testInclusion.Points.Count);

            Assert.IsTrue(testInclusion.PointsString == "", "points string should be ''. It is " + testInclusion.PointsString);

            Assert.IsTrue(testInclusion.StartDepth == 0, "Start Depth should be 0. It is " + testInclusion.StartDepth);
            Assert.IsTrue(testInclusion.EndDepth == 0, "End Depth should be 0. It is " + testInclusion.EndDepth);

            Assert.IsTrue(testInclusion.InclusionType == "", "Type should be ''. It is " + testInclusion.InclusionType);
        }

        [Test]
        public void TestLeftXBoundary()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.LeftXBoundary == 0, "Left X boundary should be 0. It is " + testInclusion.LeftXBoundary);

            testInclusion.AddPoint(new Point(13, 60));
            Assert.IsTrue(testInclusion.LeftXBoundary == 13, "Left X boundary should be 13. It is " + testInclusion.LeftXBoundary);

            testInclusion.AddPoint(new Point(45, 3));
            Assert.IsTrue(testInclusion.LeftXBoundary == 13, "Left X boundary should be 13. It is " + testInclusion.LeftXBoundary);

            testInclusion.AddPoint(new Point(12, 1));
            Assert.IsTrue(testInclusion.LeftXBoundary == 12, "Left X boundary should be 12. It is " + testInclusion.LeftXBoundary);

        }

        [Test]
        public void TestRightXBoundary()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.RightXBoundary == 0, "Right X boundary should be 0. It is " + testInclusion.RightXBoundary);

            testInclusion.AddPoint(new Point(13, 60));
            Assert.IsTrue(testInclusion.RightXBoundary == 13, "Right X boundary should be 13. It is " + testInclusion.RightXBoundary);

            testInclusion.AddPoint(new Point(45, 3));
            Assert.IsTrue(testInclusion.RightXBoundary == 45, "Right X boundary should be 45. It is " + testInclusion.RightXBoundary);

            testInclusion.AddPoint(new Point(12, 1));
            Assert.IsTrue(testInclusion.RightXBoundary == 45, "Right X boundary should be 45. It is " + testInclusion.RightXBoundary);
        }

        [Test]
        public void TestTopYBoundary()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.TopYBoundary == 0, "Top Y boundary should be 0. It is " + testInclusion.TopYBoundary);

            testInclusion.AddPoint(new Point(13, 60));
            Assert.IsTrue(testInclusion.TopYBoundary == 60, "Top Y boundary should be 60. It is " + testInclusion.TopYBoundary);

            testInclusion.AddPoint(new Point(45, 3));
            Assert.IsTrue(testInclusion.TopYBoundary == 3, "Top Y boundary should be 3. It is " + testInclusion.TopYBoundary);

            testInclusion.AddPoint(new Point(12, 4));
            Assert.IsTrue(testInclusion.TopYBoundary == 3, "Top Y boundary should be 3. It is " + testInclusion.TopYBoundary);

            testInclusion.AddPoint(new Point(1, 2));
            Assert.IsTrue(testInclusion.TopYBoundary == 2, "Top Y boundary should be 2. It is " + testInclusion.TopYBoundary);
        }

        [Test]
        public void TestBottomYBoundary()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.BottomYBoundary == 0, "Bottom Y boundary should be 0. It is " + testInclusion.BottomYBoundary);

            testInclusion.AddPoint(new Point(13, 60));
            Assert.IsTrue(testInclusion.BottomYBoundary == 60, "Bottom Y boundary should be 60. It is " + testInclusion.BottomYBoundary);

            testInclusion.AddPoint(new Point(45, 3));
            Assert.IsTrue(testInclusion.BottomYBoundary == 60, "Bottom Y boundary should be 60. It is " + testInclusion.BottomYBoundary);

            testInclusion.AddPoint(new Point(12, 76));
            Assert.IsTrue(testInclusion.BottomYBoundary == 76, "Bottom Y boundary should be 76. It is " + testInclusion.BottomYBoundary);

            testInclusion.AddPoint(new Point(1, 2));
            Assert.IsTrue(testInclusion.BottomYBoundary == 76, "Bottom Y boundary should be 76. It is " + testInclusion.BottomYBoundary);
        }

        [Test]
        public void TestIsComplete()
        {
            testInclusion = new Inclusion(720, 2);

            Assert.IsTrue(testInclusion.IsComplete == false, "isComplete should be false. It is " + testInclusion.IsComplete);

            testInclusion.IsComplete = true;
            Assert.IsTrue(testInclusion.IsComplete == true, "isComplete should be true. It is " + testInclusion.IsComplete);

            testInclusion.IsComplete = false;
            Assert.IsTrue(testInclusion.IsComplete == false, "isComplete should be false. It is " + testInclusion.IsComplete);
        }

        [Test]
        public void TestAddPoint()
        {
            testInclusion = new Inclusion(720, 2);
            Assert.IsTrue(testInclusion.Points != null, "points list should be null. It is " + testInclusion.Points);
            Assert.IsTrue(testInclusion.Points.Count == 0, "points list count should be 0. It is " + testInclusion.Points.Count);

            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));

            Assert.IsTrue(testInclusion.Points.Count == 2, "points list count should be 2. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[1]);

            testInclusion.AddPoint(new Point(4, 12));

            Assert.IsTrue(testInclusion.Points.Count == 3, "points list count should be 3. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[2] == new Point(4, 12), "Second point should be 4,12. It is " + testInclusion.Points[2]);
        }

        [Test]
        public void TestPointsString()
        {
            testInclusion = new Inclusion(720, 2);
            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));

            Assert.IsTrue(testInclusion.PointsString.Equals("(12, 8) (32, 1) "), "Points string should be '(12, 8) (32, 1)'.  It is " + testInclusion.PointsString);

            testInclusion.AddPoint(new Point(100, 11));

            Assert.IsTrue(testInclusion.PointsString.Equals("(12, 8) (32, 1) (100, 11) "), "Points string should be '(12, 8) (32, 1) (100, 11)'.  It is " + testInclusion.PointsString);
        }

        [Test]
        public void TestMovePoint()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));
            testInclusion.AddPoint(new Point(100, 10));

            testInclusion.MovePoint(new Point(100, 10), new Point(110, 7));

            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8));
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1));
            Assert.IsFalse(testInclusion.Points[2] == new Point(100, 10));
            Assert.IsTrue(testInclusion.Points[2] == new Point(110, 7));

            testInclusion.MovePoint(new Point(100, 10), new Point(200, 20));

            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8));
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1));
            Assert.IsFalse(testInclusion.Points[2] == new Point(100, 10));
            Assert.IsTrue(testInclusion.Points[2] == new Point(110, 7));
        }

        [Test]
        public void TestRemovePoint()
        {
            Point deletePoint = new Point(12, 8);

            testInclusion = new Inclusion(720, 2);
            Assert.IsTrue(testInclusion.Points != null, "points list should be null. It is " + testInclusion.Points);
            Assert.IsTrue(testInclusion.Points.Count == 0, "points list count should be 0. It is " + testInclusion.Points.Count);

            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));

            Assert.IsTrue(testInclusion.Points.Count == 2, "points list count should be 2. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[1]);

            testInclusion.RemovePoint(new Point(12, 8));

            Assert.IsTrue(testInclusion.Points.Count == 1, "points list count should be 1. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[0]);
        }

        [Test]
        public void TestRemoveAddPoint()
        {
            Point deletePoint = new Point(12, 8);

            testInclusion = new Inclusion(720, 2);
            Assert.IsTrue(testInclusion.Points != null, "points list should be null. It is " + testInclusion.Points);
            Assert.IsTrue(testInclusion.Points.Count == 0, "points list count should be 0. It is " + testInclusion.Points.Count);

            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));

            Assert.IsTrue(testInclusion.Points.Count == 2, "points list count should be 2. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[1]);

            //testCluster.removePoint(new Point(12, 8));
            testInclusion.RemovePoint(deletePoint);
            Assert.IsTrue(testInclusion.Points.Count == 1, "points list count should be 1. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[0]);


            testInclusion.AddPoint(new Point(4, 12));

            Assert.IsTrue(testInclusion.Points.Count == 2, "points list count should be 3. It is " + testInclusion.Points.Count);
            Assert.IsTrue(testInclusion.Points[0] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(4, 12), "Second point should be 4,12. It is " + testInclusion.Points[1]);

            testInclusion.RemovePoint(new Point(4, 12));
            testInclusion.RemovePoint(new Point(32, 1));

            Assert.IsTrue(testInclusion.Points.Count == 0, "points list count should be 0. It is " + testInclusion.Points.Count);
        }

        [Test]
        public void TestMoveInclusion()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.AddPoint(new Point(12, 8));
            testInclusion.AddPoint(new Point(32, 1));
            testInclusion.AddPoint(new Point(100, 10));

            Assert.IsTrue(testInclusion.Points[0] == new Point(12, 8), "First point should be 12,8. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(32, 1), "Second point should be 32,1. It is " + testInclusion.Points[1]);
            Assert.IsTrue(testInclusion.Points[2] == new Point(100, 10), "Second point should be 100,10. It is " + testInclusion.Points[2]);

            testInclusion.MoveInclusion(10, 0);

            Assert.IsTrue(testInclusion.Points[0] == new Point(22, 8), "First point should be 22,8. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(42, 1), "Second point should be 42,1. It is " + testInclusion.Points[1]);
            Assert.IsTrue(testInclusion.Points[2] == new Point(110, 10), "Second point should be 110,10. It is " + testInclusion.Points[2]);

            testInclusion.MoveInclusion(20, -10);

            Assert.IsTrue(testInclusion.Points[0] == new Point(42, -2), "First point should be 42, -2. It is " + testInclusion.Points[0]);
            Assert.IsTrue(testInclusion.Points[1] == new Point(62, -9), "Second point should be 62, -9. It is " + testInclusion.Points[1]);
            Assert.IsTrue(testInclusion.Points[2] == new Point(130, 0), "Second point should be 130, 0. It is " + testInclusion.Points[2]);
        }

        [Test]
        public void TestStartDepth()
        {
            testInclusion = new Inclusion(720, 1);

            testInclusion.AddPoint(new Point(12, 8));
            Assert.IsTrue(testInclusion.StartDepth == 8, "Start depth should be 8mm.  It is " + testInclusion.StartDepth);
            testInclusion.AddPoint(new Point(32, 2));
            Assert.IsTrue(testInclusion.StartDepth == 2, "Start depth should be 2mm.  It is " + testInclusion.StartDepth);
            testInclusion.AddPoint(new Point(100, 10));
            Assert.IsTrue(testInclusion.StartDepth == 2, "Start depth should be 2mm.  It is " + testInclusion.StartDepth);

            testInclusion.SourceStartDepth = 100;
            Assert.IsTrue(testInclusion.StartDepth == 102, "Start depth should be 102mm.  It is " + testInclusion.StartDepth);

            testInclusion.AddPoint(new Point(43, 1));

            Assert.IsTrue(testInclusion.StartDepth == 101, "Start depth should be 101mm.  It is " + testInclusion.StartDepth);
        }

        [Test]
        public void TestStartDepth2MM()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.AddPoint(new Point(12, 8));

            Assert.IsTrue(testInclusion.StartDepth == 16, "Start depth should be 16mm.  It is " + testInclusion.StartDepth);
            testInclusion.AddPoint(new Point(32, 2));
            Assert.IsTrue(testInclusion.StartDepth == 4, "Start depth should be 4mm.  It is " + testInclusion.StartDepth);
            testInclusion.AddPoint(new Point(100, 10));
            Assert.IsTrue(testInclusion.StartDepth == 4, "Start depth should be 4mm.  It is " + testInclusion.StartDepth);

            testInclusion.SourceStartDepth = 100;
            Assert.IsTrue(testInclusion.StartDepth == 104, "Start depth should be 104mm.  It is " + testInclusion.StartDepth);

            testInclusion.AddPoint(new Point(43, 1));

            Assert.IsTrue(testInclusion.StartDepth == 102, "Start depth should be 102mm.  It is " + testInclusion.StartDepth);
        }

        [Test]
        public void TestEndDepth1()
        {
            testInclusion = new Inclusion(720, 1);

            testInclusion.AddPoint(new Point(12, 8));

            Assert.IsTrue(testInclusion.EndDepth == 8, "End depth should be 8mm.  It is " + testInclusion.EndDepth);
            testInclusion.AddPoint(new Point(32, 2));
            Assert.IsTrue(testInclusion.EndDepth == 8, "End depth should be 8mm.  It is " + testInclusion.EndDepth);
            testInclusion.AddPoint(new Point(100, 10));
            Assert.IsTrue(testInclusion.EndDepth == 10, "End depth should be 10mm.  It is " + testInclusion.EndDepth);

            testInclusion.SourceStartDepth = 100;
            Assert.IsTrue(testInclusion.EndDepth == 110, "End depth should be 110mm.  It is " + testInclusion.EndDepth);

            testInclusion.AddPoint(new Point(43, 100));

            Assert.IsTrue(testInclusion.EndDepth == 200, "End depth should be 200mm.  It is " + testInclusion.EndDepth);
        }

        [Test]
        public void TestEndDepth2()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.AddPoint(new Point(12, 8));

            Assert.IsTrue(testInclusion.EndDepth == 16, "End depth should be 16mm.  It is " + testInclusion.EndDepth);
            testInclusion.AddPoint(new Point(32, 2));
            Assert.IsTrue(testInclusion.EndDepth == 16, "End depth should be 16mm.  It is " + testInclusion.EndDepth);
            testInclusion.AddPoint(new Point(100, 10));
            Assert.IsTrue(testInclusion.EndDepth == 20, "End depth should be 20mm.  It is " + testInclusion.EndDepth);

            testInclusion.SourceStartDepth = 100;
            Assert.IsTrue(testInclusion.EndDepth == 120, "End depth should be 120mm.  It is " + testInclusion.EndDepth);

            testInclusion.AddPoint(new Point(43, 100));

            Assert.IsTrue(testInclusion.EndDepth == 300, "End depth should be 300mm.  It is " + testInclusion.EndDepth);
        }

        [Test]
        public void TestType()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.InclusionType = "Clast";

            Assert.IsTrue(testInclusion.InclusionType.Equals("Clast"), "Type should be 'Clast'. It is " + testInclusion.InclusionType);
        }

        [Test]
        public void TestDetails()
        {
            DateTime time = DateTime.Now;

            testInclusion = new Inclusion(720, 2);
            testInclusion.InclusionType = "Inclusion type";

            testInclusion.AddPoint(new Point(10, 20));
            testInclusion.AddPoint(new Point(11, 15));

            testInclusion.Description = "Inclusion, description";
            testInclusion.Group = "group1";
            testInclusion.TimeAdded = time;
            testInclusion.TimeLastModified = time;


            Assert.IsTrue(testInclusion.getDetails().Equals("15,20,10,11,10 20 11 15 ,Inclusion type,Inclusion  description," + time + "," + time + ",group1"), "Details is wrong." + testInclusion.getDetails());

        }

        [Test]
        public void TestDescription()
        {
            testInclusion = new Inclusion(720, 2);

            testInclusion.Description = "An inclusion";
            Assert.IsTrue(testInclusion.Description.Equals("An inclusion"), "Description should be 'An inclusion'.  It is " + testInclusion.Description);
        }

        [Test]
        public void TestTimeAdded()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            testInclusion = new Inclusion(720, 1);

            DateTime timeAdded = Convert.ToDateTime(testInclusion.TimeAdded, dtfi);

            timeAdded = timeAdded.AddDays(4.0);

            Inclusion addedInclusion = new Inclusion(720, 1);

            Assert.IsFalse(addedInclusion.TimeAdded.Equals(timeAdded), "Clusters times should not match. They are " + addedInclusion.TimeAdded.ToString() + " and " + timeAdded.ToString());

            addedInclusion.TimeAdded = timeAdded;

            Assert.IsTrue(addedInclusion.TimeAdded.Equals(timeAdded), "Clusters times should match. They are " + addedInclusion.TimeAdded.ToString() + " and " + timeAdded.ToString());
        }

        [Test]
        public void TestTimeLastModified()
        {
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
            dtfi.ShortDatePattern = "dd-MM-yyyy";
            dtfi.DateSeparator = "-";

            testInclusion = new Inclusion(720, 1);

            DateTime timeModified = Convert.ToDateTime(testInclusion.TimeAdded, dtfi);

            timeModified = timeModified.AddDays(4.0);

            Inclusion addedInclusion = new Inclusion(720, 1);

            Assert.IsFalse(addedInclusion.TimeLastModified.Equals(timeModified), "Inclusions times should not match. They are " + addedInclusion.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedInclusion.TimeLastModified = timeModified;

            Assert.IsTrue(addedInclusion.TimeLastModified.Equals(timeModified), "Inclusions times should match. They are " + addedInclusion.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedInclusion.AddPoint(new Point(10, 10));

            Assert.IsFalse(addedInclusion.TimeLastModified.Equals(timeModified), "Inclusions times should not match. They are " + addedInclusion.TimeLastModified.ToString() + " and " + timeModified.ToString());
        }
    }
}
