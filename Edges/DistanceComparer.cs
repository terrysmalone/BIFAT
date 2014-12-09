using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Edges
{
    /// <summary>
    /// Class which overrides IComparer in order to allow the sorting of points by how close they are to 
    /// (0,0) in ascending order
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class DistanceComparer : IComparer<Point>
    {
        public int Compare(Point one, Point two)
        {
            Point zero = new Point(0, 0);
            double distance1, distance2;

            double xDistance = Math.Max(one.X, zero.X) - Math.Min(one.X, zero.X);
            double yDistance = Math.Max(one.Y, zero.Y) - Math.Min(one.Y, zero.Y);

            distance1 = Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance));

            xDistance = Math.Max(two.X, zero.X) - Math.Min(two.X, zero.X);
            yDistance = Math.Max(two.Y, zero.Y) - Math.Min(two.Y, zero.Y);

            distance2 = Math.Sqrt((xDistance * xDistance) + (yDistance * yDistance));

            return (int)((distance1 - distance2) * 100);
        }
    }

}
