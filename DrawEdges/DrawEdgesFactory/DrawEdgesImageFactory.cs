using System;
using System.Collections.Generic;
using System.Drawing;
using Edges;
using BoreholeFeatures;

namespace DrawEdges.DrawEdgesFactory
{
    public class DrawEdgesImageFactory
    {
        protected String type;

        /// <summary>
        /// Constructor method which accepts the type of edge input as a string:
        /// "Bool" - Array of bool values
        /// "ImagePoint" - List of ImagePoints
        /// "Edge" - List of Edges
        /// "Point" - List of Points
        /// </summary>
        /// <param name="type"></param>
        public DrawEdgesImageFactory(String type)
        {
            this.type = type;
        }

        public DrawEdgesImage setUpDrawEdges(object imageData, int imageWidth, int imageHeight)
        {
            if (type.Equals("Bool"))
                return new BoolEdgesImage((bool[])imageData, imageWidth, imageHeight);
            else if (type.Equals("Point"))
                return new PointEdgesImage((List<Point>)imageData, imageWidth, imageHeight);
            else if (type.Equals("Edge"))
                return new EdgeEdgesImage((List<Edge>)imageData, imageWidth, imageHeight);
            else
                throw new FormatException("Incorrect data type");
        }
    }
}
