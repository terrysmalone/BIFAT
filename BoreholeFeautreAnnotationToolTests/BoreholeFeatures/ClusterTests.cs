using System;
using System.Drawing;
using System.Globalization;
using BoreholeFeatures;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests.BoreholeFeatures
{
    public class ClusterTests
    {
        [Test]
        public void Constructor_properties_are_correct()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(0));
            Assert.That(testCluster.TopYBoundary, Is.EqualTo(0));
            Assert.That(testCluster.LeftXBoundary, Is.EqualTo(0));
            Assert.That(testCluster.RightXBoundary, Is.EqualTo(0));

            Assert.That(testCluster.IsComplete, Is.EqualTo(false));
            Assert.That(testCluster.Points, Is.Not.Null);
            Assert.That(testCluster.Points.Count, Is.EqualTo(0));
            
            Assert.That(testCluster.PointsString, Is.EqualTo(string.Empty));

            Assert.That(testCluster.StartDepth, Is.EqualTo(0));
            Assert.That(testCluster.EndDepth, Is.EqualTo(0));

            Assert.That(testCluster.ClusterType, Is.EqualTo(string.Empty));
        }

        [Test]
        public void LeftXBoundary_updates()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.LeftXBoundary, Is.EqualTo(0));
            
            testCluster.AddPoint(new Point(13, 60));
            Assert.That(testCluster.LeftXBoundary, Is.EqualTo(13));

            testCluster.AddPoint(new Point(45, 3));
            Assert.That(testCluster.LeftXBoundary, Is.EqualTo(13));

            testCluster.AddPoint(new Point(12, 1));
            Assert.That(testCluster.LeftXBoundary, Is.EqualTo(12));
        }

        [Test]
        public void RightXBoundary_updates()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.RightXBoundary, Is.EqualTo(0));
            
            testCluster.AddPoint(new Point(13, 60));
            Assert.That(testCluster.RightXBoundary, Is.EqualTo(13));

            testCluster.AddPoint(new Point(45, 3));
            Assert.That(testCluster.RightXBoundary, Is.EqualTo(45));

            testCluster.AddPoint(new Point(12, 1));
            Assert.That(testCluster.RightXBoundary, Is.EqualTo(45));
        }

        [Test]
        public void TopYBoundary_updates()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.TopYBoundary, Is.EqualTo(0));

            testCluster.AddPoint(new Point(13, 60));
            Assert.That(testCluster.TopYBoundary, Is.EqualTo(60));

            testCluster.AddPoint(new Point(45, 3));
            Assert.That(testCluster.TopYBoundary, Is.EqualTo(3));

            testCluster.AddPoint(new Point(12, 4));
            Assert.That(testCluster.TopYBoundary, Is.EqualTo(3));

            testCluster.AddPoint(new Point(1, 2));
            Assert.That(testCluster.TopYBoundary, Is.EqualTo(2));
        }

        [Test]
        public void BottomYBoundary_updates()
        {
            var testCluster = new Cluster(720, 2);
            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(0));

            testCluster.AddPoint(new Point(13, 60));
            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(60));

            testCluster.AddPoint(new Point(45, 3));
            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(60));

            testCluster.AddPoint(new Point(12, 76));
            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(76));

            testCluster.AddPoint(new Point(1, 2));
            Assert.That(testCluster.BottomYBoundary, Is.EqualTo(76));
        }

        [Test]
        public void AddPoint_behaves_as_expected()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.Points, Is.Not.Null);
            Assert.That(testCluster.Points.Count, Is.EqualTo(0));
            
            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.That(testCluster.Points.Count, Is.EqualTo(2));
            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));
            
            testCluster.AddPoint(new Point(4, 12));

            Assert.That(testCluster.Points.Count, Is.EqualTo(3));
            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(4, 12)));
        }

        [Test]
        public void PointsString_is_correct()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));
            Assert.That(testCluster.PointsString, Is.EqualTo("(12, 8) (32, 1) "));
            
            testCluster.AddPoint(new Point(100, 11));
            Assert.That(testCluster.PointsString, Is.EqualTo("(12, 8) (32, 1) (100, 11) "));
        }

        [Test]
        public void Points_can_be_moved()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));
            testCluster.AddPoint(new Point(100, 10));

            testCluster.MovePoint(new Point(100, 10), new Point(110, 7));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));
            Assert.That(testCluster.Points[2], Is.Not.EqualTo(new Point(100, 10))); //Old point
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(110, 7)));
            
            testCluster.MovePoint(new Point(100, 10), new Point(200, 20));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));
            Assert.That(testCluster.Points[2], Is.Not.EqualTo(new Point(100, 10)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(110, 7)));
        }

        [Test]
        public void Points_can_be_removed()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.Points, Is.Not.Null);
            Assert.That(testCluster.Points.Count, Is.EqualTo(0));
            
            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.That(testCluster.Points.Count, Is.EqualTo(2));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));

            testCluster.RemovePoint(new Point(12, 8));

            Assert.That(testCluster.Points.Count, Is.EqualTo(1));
            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(32, 1)));
        }

        [Test]
        public void Points_can_be_removed_and_added()
        {
            var testCluster = new Cluster(720, 2);

            Assert.That(testCluster.Points, Is.Not.Null);
            Assert.That(testCluster.Points.Count, Is.EqualTo(0));

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));

            Assert.That(testCluster.Points.Count, Is.EqualTo(2));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));

            testCluster.RemovePoint(new Point(12, 8));

            Assert.That(testCluster.Points.Count, Is.EqualTo(1));
            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(32, 1)));

            testCluster.AddPoint(new Point(4, 12));

            Assert.That(testCluster.Points.Count, Is.EqualTo(2));
            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(32, 1)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(4, 12)));
            
            testCluster.RemovePoint(new Point(4, 12));
            testCluster.RemovePoint(new Point(32, 1));

            Assert.That(testCluster.Points.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestMoveCluster()
        {
            var testCluster = new Cluster(720, 2);

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

        [Test]
        public void TestStartDepth1()
        {
            var testCluster = new Cluster(720, 1);

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

        [Test]
        public void TestStartDepth2()
        {
            var testCluster = new Cluster(720, 2);

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

        [Test]
        public void TestEndDepth1()
        {
            var testCluster = new Cluster(720, 1);

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

        [Test]
        public void TestEndDepth2()
        {
            var testCluster = new Cluster(720, 2);

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

        [Test]
        public void TestType()
        {
            var testCluster = new Cluster(720, 2);

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

        [Test]
        public void TestDetails()
        {
            var time = DateTime.Now;

            var testCluster = new Cluster(720, 2);

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

        [Test]
        public void TestDescription()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.Description = "A cluster";
            Assert.IsTrue(testCluster.Description.Equals("A cluster"), "Description should be 'A cluster'.  It is " + testCluster.Description);
        }

        [Test]
        public void TestTimeAdded()
        {
            var dtfi = new DateTimeFormatInfo
            {
                ShortDatePattern = "dd-MM-yyyy",
                DateSeparator = "-"
            };

            var testCluster = new Cluster(720, 1);

            DateTime timeAdded = Convert.ToDateTime(testCluster.TimeAdded, dtfi);

            timeAdded = timeAdded.AddDays(4.0);

            var addedCluster = new Cluster(720, 1);

            Assert.IsFalse(addedCluster.TimeAdded.Equals(timeAdded), "Clusters times should not match. They are " + addedCluster.TimeAdded.ToString() + " and " + timeAdded.ToString());

            addedCluster.TimeAdded = timeAdded;

            Assert.IsTrue(addedCluster.TimeAdded.Equals(timeAdded), "Clusters times should match. They are " + addedCluster.TimeAdded.ToString() + " and " + timeAdded.ToString());
        }

        [Test]
        public void TestTimeLastModified()
        {
            var dtfi = new DateTimeFormatInfo
            {
                ShortDatePattern = "dd-MM-yyyy",
                DateSeparator = "-"
            };

            var testCluster = new Cluster(720, 1);

            var timeModified = Convert.ToDateTime(testCluster.TimeAdded, dtfi);

            timeModified = timeModified.AddDays(4.0);

            var addedCluster = new Cluster(720, 1);

            Assert.IsFalse(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should not match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedCluster.TimeLastModified = timeModified;

            Assert.IsTrue(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());

            addedCluster.AddPoint(new Point(10, 10));

            Assert.IsFalse(addedCluster.TimeLastModified.Equals(timeModified), "Cluster times should not match. They are " + addedCluster.TimeLastModified.ToString() + " and " + timeModified.ToString());
        }
    }
}