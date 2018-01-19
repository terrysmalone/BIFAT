using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using EdgeFitting;
using NUnit.Framework;

namespace BoreholeFeautreAnnotationToolTests
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
        public void TestOrientation1()
        {
            Point beforePoint = new Point(34, 100);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 100);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 1, "Orientation should be 1.  It is " + orientation);
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|X|  
        /// |-|-|-| 
        /// </summary>
        [Test]
        public void TestOrientation2a()
        {
            Point beforePoint = new Point(34, 99);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 100);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 2, "Orientation should be 2.  It is " + orientation);
        }

        /// <summary>
        /// |-|-|-|  
        /// |X|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void TestOrientation2b()
        {
            Point beforePoint = new Point(34, 100);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 2, "Orientation should be 2.  It is " + orientation);
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void TestOrientation3()
        {
            Point beforePoint = new Point(34, 99);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 3, "Orientation should be 3.  It is " + orientation);
        }

        /// <summary>
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void TestOrientation4a()
        {
            Point beforePoint = new Point(34, 99);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(35, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 4, "Orientation should be 4.  It is " + orientation);
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void TestOrientation4b()
        {
            Point beforePoint = new Point(35, 99);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 4, "Orientation should be 4.  It is " + orientation);
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void TestOrientation5()
        {
            Point beforePoint = new Point(35, 99);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(35, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 5, "Orientation should be 5.  It is " + orientation);
        }

        /// <summary>
        /// |-|-|X|  
        /// |-|X|-|  
        /// |-|x|-| 
        /// </summary>
        [Test]
        public void TestOrientation6a()
        {
            Point beforePoint = new Point(35, 101);
            Point checkPoint = new Point(35, 100);
            Point afterPoint = new Point(36, 99);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 6, "Orientation should be 6.  It is " + orientation);
        }

        /// <summary>
        /// |-|X|-|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void TestOrientation6b()
        {
            Point beforePoint = new Point(35, 101);
            Point checkPoint = new Point(36, 100);
            Point afterPoint = new Point(36, 99);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 6, "Orientation should be 6.  It is " + orientation);
        }

        /// <summary>
        /// |-|-|X|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void TestOrientation7()
        {
            Point beforePoint = new Point(35, 101);
            Point checkPoint = new Point(36, 100);
            Point afterPoint = new Point(37, 99);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 7, "Orientation should be 7.  It is " + orientation);
        }

        /// <summary>
        /// |-|-|X|  
        /// |X|X|-|  
        /// |-|-|-| 
        /// </summary>
        [Test]
        public void TestOrientation8a()
        {
            Point beforePoint = new Point(35, 101);
            Point checkPoint = new Point(36, 101);
            Point afterPoint = new Point(37, 99);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 8, "Orientation should be 8.  It is " + orientation);
        }

        /// <summary>
        /// |-|-|-|  
        /// |-|X|X|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void TestOrientation8b()
        {
            Point beforePoint = new Point(35, 101);
            Point checkPoint = new Point(36, 100);
            Point afterPoint = new Point(37, 100);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 8, "Orientation should be 8.  It is " + orientation);
        }

        /// <summary>
        /// 7/
        /// |-|-|X|  
        /// |-|X|-|  
        /// |X|-|-| 
        /// </summary>
        [Test]
        public void TestPointAtLeftEdge()
        {
            Point beforePoint = new Point(359, 101);
            Point checkPoint = new Point(0, 100);
            Point afterPoint = new Point(1, 99);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 7, "Orientation should be 7.  It is " + orientation);
        }

        /// <summary>
        /// 4/
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|-|X|
        /// </summary>
        [Test]
        public void TestPointAtLeftEdgeButNotWrapping()
        {
            Point beforePoint = new Point(0, 99);
            Point checkPoint = new Point(0, 100);
            Point afterPoint = new Point(1, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 4, "Orientation should be 4.  It is " + orientation);
        }

        /// <summary>
        /// 3/
        /// |X|-|-|  
        /// |-|X|-|  
        /// |-|-|X| 
        /// </summary>
        [Test]
        public void TestPointAtRightEdge()
        {
            Point beforePoint = new Point(358, 99);
            Point checkPoint = new Point(359, 100);
            Point afterPoint = new Point(0, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 3, "Orientation should be 3.  It is " + orientation);
        }

        /// <summary>
        /// 5/
        /// |-|X|-|  
        /// |-|X|-|  
        /// |-|X|-| 
        /// </summary>
        [Test]
        public void TestPointAtRightEdgeButNotWrapping()
        {
            Point beforePoint = new Point(359, 99);
            Point checkPoint = new Point(359, 100);
            Point afterPoint = new Point(359, 101);

            PointOrientation pointOrientation = new PointOrientation(beforePoint, checkPoint, afterPoint, 360);

            int orientation = pointOrientation.Orientation;

            Assert.IsTrue(orientation == 5, "Orientation should be 5.  It is " + orientation);
        }

    }
}
