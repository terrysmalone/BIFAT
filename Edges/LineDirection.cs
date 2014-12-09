using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Edges
{
    /// <summary>
    /// Calculates the direction of a line, represented by two points. 
    /// Direction is from 0-359 beginning at East and travelling clockwise
    /// </summary>
    public class LineDirection
    {
        private int direction;

        private Point startPoint;
        private Point endPoint;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint">The start of the line</param>
        /// <param name="endPoint">The end of the line</param>
        public LineDirection(Point startPoint, Point endPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        public void Calculate()
        {
            int yDiff = startPoint.Y - endPoint.Y;
            int xDiff = endPoint.X - startPoint.X;

            double angle = Math.Atan2(yDiff, xDiff) * (float)(180 / Math.PI);

            if (angle < 0)
                angle = 360 + angle;

            //MessageBox.Show("Angle: " + angle);

            direction = Convert.ToInt32(angle);
        }

        public int GetDirection()
        {
            return direction;
        }
    }
}
