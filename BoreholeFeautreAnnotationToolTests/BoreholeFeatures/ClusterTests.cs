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
        public void Cluster_can_be_moved_x()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, 1));
            testCluster.AddPoint(new Point(100, 10));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, 1)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(100, 10)));

            testCluster.MoveCluster(10, 0);

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(22, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(42, 1)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(110, 10)));
        }

        [Test]
        public void Cluster_can_be_moved_y()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            testCluster.AddPoint(new Point(32, -13));
            testCluster.AddPoint(new Point(100, 10));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, -13)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(100, 10)));
            
            testCluster.MoveCluster(0, -10);

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(12, -2)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(32, -23)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(100, 0)));
        }

        [Test]
        public void Cluster_can_be_moved_by_both_points()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(23, 8));
            testCluster.AddPoint(new Point(198, -13));
            testCluster.AddPoint(new Point(-54, 54));

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(23, 8)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(198, -13)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(-54, 54)));

            testCluster.MoveCluster(340, 143);

            Assert.That(testCluster.Points[0], Is.EqualTo(new Point(363, 151)));
            Assert.That(testCluster.Points[1], Is.EqualTo(new Point(538, 130)));
            Assert.That(testCluster.Points[2], Is.EqualTo(new Point(286, 197)));
        }

        [Test]
        public void Start_depth_is_correct_for_resolution_of_1()
        {
            var testCluster = new Cluster(720, 1);

            testCluster.AddPoint(new Point(12, 8));
            Assert.That(testCluster.StartDepth, Is.EqualTo(8));

            testCluster.AddPoint(new Point(32, 2));
            Assert.That(testCluster.StartDepth, Is.EqualTo(2));

            testCluster.AddPoint(new Point(100, 10));
            Assert.That(testCluster.StartDepth, Is.EqualTo(2));

            testCluster.SourceStartDepth = 100;
            Assert.That(testCluster.StartDepth, Is.EqualTo(102));

            testCluster.AddPoint(new Point(43, 1));
            Assert.That(testCluster.StartDepth, Is.EqualTo(101));
        }

        [Test]
        public void Start_depth_is_correct_for_resolution_of_2()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            Assert.That(testCluster.StartDepth, Is.EqualTo(16));

            testCluster.AddPoint(new Point(32, 2));
            Assert.That(testCluster.StartDepth, Is.EqualTo(4));

            testCluster.AddPoint(new Point(100, 10));
            Assert.That(testCluster.StartDepth, Is.EqualTo(4));

            testCluster.SourceStartDepth = 100;
            Assert.That(testCluster.StartDepth, Is.EqualTo(104));

            testCluster.AddPoint(new Point(43, 1));
            Assert.That(testCluster.StartDepth, Is.EqualTo(102));
        }

        [Test]
        public void EndDepth_is_correct_for_resolution_of_1()
        {
            var testCluster = new Cluster(720, 1);

            testCluster.AddPoint(new Point(12, 8));
            Assert.That(testCluster.EndDepth, Is.EqualTo(8));

            testCluster.AddPoint(new Point(32, 2));
            Assert.That(testCluster.EndDepth, Is.EqualTo(8));

            testCluster.AddPoint(new Point(100, 10));
            Assert.That(testCluster.EndDepth, Is.EqualTo(10));

            testCluster.SourceStartDepth = 100;
            Assert.That(testCluster.EndDepth, Is.EqualTo(110));

            testCluster.AddPoint(new Point(43, 100));
            Assert.That(testCluster.EndDepth, Is.EqualTo(200));
        }

        [Test]
        public void EndDepth_is_correct_for_resolution_of_2()
        {
            var testCluster = new Cluster(720, 2);

            testCluster.AddPoint(new Point(12, 8));
            Assert.That(testCluster.EndDepth, Is.EqualTo(16));

            testCluster.AddPoint(new Point(32, 2));
            Assert.That(testCluster.EndDepth, Is.EqualTo(16));

            testCluster.AddPoint(new Point(100, 10));
            Assert.That(testCluster.EndDepth, Is.EqualTo(20));

            testCluster.SourceStartDepth = 100;
            Assert.That(testCluster.EndDepth, Is.EqualTo(120));

            testCluster.AddPoint(new Point(43, 100));
            Assert.That(testCluster.EndDepth, Is.EqualTo(300));
        }

        [Test]
        public void TestType()
        {
            var testCluster = new Cluster(720, 2)
            {
                LargeBubbles = true,
                FineDebris = true
            };

            Assert.That(testCluster.ClusterType.Contains("largeBubbles"), Is.True);
            Assert.That(testCluster.ClusterType.Contains("fineDebris"), Is.True);

            testCluster.FineDebris = false;
            testCluster.Diamicton = true;

            Assert.That(testCluster.ClusterType.Contains("largeBubbles"), Is.True);
            Assert.That(testCluster.ClusterType.Contains("fineDebris"), Is.False);
            Assert.That(testCluster.ClusterType.Contains("diamicton"), Is.True);
        }

        [Test]
        public void Details_are_correct()
        {
            var time = DateTime.Now;

            var testCluster = new Cluster(720, 2)
            {
                LargeBubbles = true,
                FineDebris = true,
                Description = "Cluster, description",
                Group = "group1",
                TimeAdded = time,
                TimeLastModified = time
            };

            testCluster.AddPoint(new Point(10, 20));
            testCluster.AddPoint(new Point(11, 15));

            Assert.That(testCluster.GetDetails(), Is.EqualTo("15,20,10,11,10 20 11 15 ,largeBubbles fineDebris ,Cluster  description," + time + "," + time + ",group1"));
        }

        [Test]
        public void Dexcription_is_correct()
        {
            var testCluster = new Cluster(720, 2) {Description = "A cluster"};

            Assert.That(testCluster.Description, Is.EqualTo("A cluster"));
        }

        [Test]
        public void TimeAdded_is_correct()
        {
            var dtfi = new DateTimeFormatInfo
            {
                ShortDatePattern = "dd-MM-yyyy",
                DateSeparator = "-"
            };

            var testCluster = new Cluster(720, 1);

            var timeAdded = Convert.ToDateTime(testCluster.TimeAdded, dtfi);
            timeAdded = timeAdded.AddDays(4.0);

            var addedCluster = new Cluster(720, 1);

            Assert.That(addedCluster.TimeAdded.Equals(timeAdded), Is.False);

            addedCluster.TimeAdded = timeAdded;

            Assert.That(addedCluster.TimeAdded, Is.EqualTo(timeAdded));
}

        [Test]
        public void TimeLastModified_is_correct()
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
            
            Assert.That(addedCluster.TimeLastModified, Is.Not.EqualTo(timeModified));

            addedCluster.TimeLastModified = timeModified;

            Assert.That(addedCluster.TimeLastModified, Is.EqualTo(timeModified));

            addedCluster.AddPoint(new Point(10, 10));

            Assert.That(addedCluster.TimeLastModified.Equals(timeModified), Is.False);
        }
    }
}