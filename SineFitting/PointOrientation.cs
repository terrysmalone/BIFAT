using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EdgeFitting
{
    /// <summary>
    /// Class which calculates the orientation of a point from the two points surrounding it.  Possible orientations are:
    /// 1:       2:                3:       4:                5:       6:                7:       8:
    /// |-|-|-|  |X|-|-|  |-|-|-|  |X|-|-|  |X|-|-|  |-|X|-|  |-|X|-|  |-|-|X|  |-|X|-|  |-|-|X|  |-|-|X|  |-|-|-|
    /// |X|X|X|  |-|X|X|  |X|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |-|X|-|  |X|X|-|  |-|X|X|
    /// |-|-|-|  |-|-|-|  |-|-|X|  |-|-|X|  |-|X|-|  |-|-|X|  |-|X|-|  |-|X|-|  |X|-|-|  |X|-|-|  |-|-|-|  |X|-|-|
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class PointOrientation
    {
        Point beforePoint, checkPoint, afterPoint;
        private int imageWidth;
        private int orientation;

        private int beforeX, beforeY, checkX, checkY, afterX, afterY;

        #region properties

        /// <summary>
        /// Returns the orientation of the given point
        /// </summary>
        /// <returns></returns>
        public int Orientation
        {
            get { return orientation; }
        }

        #endregion properties

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="leftPoint">Point before the  point to check</param>
        /// <param name="midPoint">The point to check</param>
        /// <param name="rightPoint">Point after the  point to check</param>
        public PointOrientation(Point beforePoint, Point checkPoint, Point afterPoint, int imageWidth)
        {
            this.beforePoint = beforePoint;
            this.checkPoint = checkPoint;
            this.afterPoint = afterPoint;

            this.imageWidth = imageWidth;

            calculatePointOrientation();
        }

        private void calculatePointOrientation()
        {
            initialiseValues();

            if (doPointsSpan1Column())                                                //orientation 1
                orientation = 1;
            else if (doPointsSpan3Columns())                       //orientation 2,3,7 or 8
            {
                if (beforeY == checkY - 1 && afterY == checkY + 1)                  //orientation 3
                    orientation = 3;
                else if (beforeY == checkY + 1 && afterY == checkY - 1)             //orientation 7
                    orientation = 7;
                else if (beforeY == afterY - 1)                                     //orientation 2
                    orientation = 2;
                else                                                                //orientation 8
                    orientation = 8;
            }
            else                                                                //Orientation 4, 5 or 6
            {
                if (beforeX == afterX)                                            //orientation 5
                    orientation = 5;
                else if (beforeX == afterX - 1 && beforeY < afterY)               //orientation 4  
                    orientation = 4;
                else
                    orientation = 6;                                            //orientation 6  
            }
        }

        /// <summary>
        /// Checks if points span one column
        /// </summary>
        /// <returns>True if all points are in same column</returns>
        private bool doPointsSpan1Column()
        {
            if (beforeY == afterY && beforeY == checkY)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if points span 3 columns
        /// </summary>
        /// <returns>True if points span 3 columns</returns>
        private bool doPointsSpan3Columns()
        {
            if (beforeX < checkX && afterX > checkX)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Initialises the three Point's values and checks if any wrap over the image
        /// </summary>
        private void initialiseValues()
        {
            checkX = checkPoint.X;
            checkY = checkPoint.Y;

            beforeY = beforePoint.Y;
            afterY = afterPoint.Y;

            checkIfPointIsAtLeftBoundary();

            checkIfPointIsAtRightBoundary();
        }

        /// <summary>
        /// If point to check is at left boundary beforeX is set to minus 1.  
        /// </summary>
        private void checkIfPointIsAtLeftBoundary()
        {
            if (checkX == 0 && beforePoint.X != 0)
            {
                beforeX = -1;
            }
            else
            {
                beforeX = beforePoint.X;
            }
        }

        /// <summary>
        /// If point to check is at left boundary afterXX is set to image width.  
        /// </summary>
        private void checkIfPointIsAtRightBoundary()
        {
            if (checkX == imageWidth - 1 && afterPoint.X != imageWidth - 1)
            {
                afterX = imageWidth;
            }
            else
            {
                afterX = afterPoint.X;
            }
        }
    }

}
