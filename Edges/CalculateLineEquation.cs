using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Edges
{
    /// <summary>
    /// Calculates the equation of a line from a point on the line and the line direction in degrees
    /// Note: Degrees starts from due east and increases counter clockwise
    /// </summary>
    public class CalculateLineEquation
    {
        private Point point;
        private int direction;

        private double slope, intercept;

        #region properties

        public double Slope
        {
            get { return slope; }
        }

        public double Intercept
        {
            get { return intercept; }
        }

        #endregion properties

        #region constructor

        public CalculateLineEquation(Point point, int direction)
        {
            this.point = point;
            this.direction = direction;

            CalculateSlope();

            CalculateIntercept();
        }

        #endregion constructor

        private void CalculateSlope()
        {
            slope = Math.Tan(Math.PI * ((double)direction / 180.0));

            slope = 0 - slope;
        }

        private void CalculateIntercept()
        {
            // y = (slope*x) + b
            // b = y - (slope*x)

            intercept = (double)(point.Y - (slope * (double)point.X));

            //MessageBox.Show("point.Y:" + point.Y + " - (slope:" + slope + "* point.X:" + point.X + ") = intercept:" + intercept);
        }
    }
}
