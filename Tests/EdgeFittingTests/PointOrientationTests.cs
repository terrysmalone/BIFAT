using System.Drawing;
using EdgeFitting;
using NUnit.Framework;

namespace EdgeFittingTests
{
    [TestFixture]
    public class PointOrientationTests
    {
        /// <summary>
        /// |-|-|-|  
        /// |X|X|X|  
        /// |-|-|-| 
        /// </summary>
        [Test]
        public void Horizontal_is_identified()
        {
            var beforePoint = new Point(34, 100);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 100);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.Horizontal));
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|X|  
        /// |-|-|-| 
        /// </summary>
        [Test]
        public void HorizontalLeadingOffset_is_identified_a()
        {
            var beforePoint = new Point(34, 99);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 100);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.HorizontalLeadingOffset));
        }

        /// <summary>
        /// |-|-|-|  
        /// |X|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void HorizontalLeadingOffset_is_identified_b()
        {
            var beforePoint = new Point(34, 100);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.HorizontalLeadingOffset));
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void LeadingDiagonal_is_identified()
        {
            var beforePoint = new Point(34, 99);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 101);
            
            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);
            
            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.LeadingDiagonal));
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void VerticalLeadingOffset_is_identified_a()
        {
            var beforePoint = new Point(34, 99);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(35, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.VerticalLeadingOffset));
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void VerticalLeadingOffset_is_identified_b()
        {
            var beforePoint = new Point(35, 99);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.VerticalLeadingOffset));
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void Vertical_is_identified()
        {
            var beforePoint = new Point(35, 99);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(35, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.Vertical));
        }

        /// <summary>
        /// |-|-|X|  
        /// |-|X|-|  
        /// |-|x|-| 
        /// </summary>
        [Test]
        public void VerticalCounterOffset_is_identified_a()
        {
            var beforePoint = new Point(35, 101);
            var checkPoint = new Point(35, 100);
            var afterPoint = new Point(36, 99);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.VerticalCounterOffset));
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void VerticalCounterOffset_is_identified_b()
        {
            var beforePoint = new Point(35, 101);
            var checkPoint = new Point(36, 100);
            var afterPoint = new Point(36, 99);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.VerticalCounterOffset));
        }

        /// <summary>
        /// |-|-|X|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void CounterDiagonal_is_identified()
        {
            var beforePoint = new Point(35, 101);
            var checkPoint = new Point(36, 100);
            var afterPoint = new Point(37, 99);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.CounterDiagonal));
        }

        /// <summary>
        /// |-|-|X|  
        /// |X|X|-|  
        /// |-|-|-| 
        /// </summary>
        [Test]
        public void HorizontalCounterOffset_is_identified_a()
        {
            var beforePoint = new Point(35, 101);
            var checkPoint = new Point(36, 101);
            var afterPoint = new Point(37, 100);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.HorizontalCounterOffset));
        }

        /// <summary>
        /// |-|-|-|  
        /// |-|X|X|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void HorizontalCounterOffset_is_identified_b()
        {
            var beforePoint = new Point(35, 101);
            var checkPoint = new Point(36, 100);
            var afterPoint = new Point(37, 100);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.HorizontalCounterOffset));
        }

        /// <summary>
        /// 7/
        /// |-|-|X|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void CounterDiagonal_at_left_edge_is_identified()
        {
            var beforePoint = new Point(359, 101);
            var checkPoint = new Point(0, 100);
            var afterPoint = new Point(1, 99);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.CounterDiagonal));
        }

        /// <summary>
        /// 4/
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|-|X|
        /// </summary>
        [Test]
        public void VerticalLeadingOffset_at_left_edge_but_not_wrapping_is_identified()
        {
            var beforePoint = new Point(0, 99);
            var checkPoint = new Point(0, 100);
            var afterPoint = new Point(1, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.VerticalLeadingOffset));
        }

        /// <summary>
        /// 3/
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void LeadingDiagonal_at_right_edge_is_identified()
        {
            var beforePoint = new Point(358, 99);
            var checkPoint = new Point(359, 100);
            var afterPoint = new Point(0, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.LeadingDiagonal));
        }

        /// <summary>
        /// 5/
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void Vertical_at_right_edge_but_not_wrapping_is_identified()
        {
            var beforePoint = new Point(359, 99);
            var checkPoint = new Point(359, 100);
            var afterPoint = new Point(359, 101);

            var pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            var orientation = pointOrientation.Orientation;

            Assert.That(orientation, Is.EqualTo(Orientation.Vertical));
        }

    }
}
