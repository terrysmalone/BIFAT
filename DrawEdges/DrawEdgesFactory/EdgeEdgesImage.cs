using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edges;
using System.Drawing;

namespace DrawEdges.DrawEdgesFactory
{
    public class EdgeEdgesImage : DrawEdgesImage
    {
        private List<Edge> imagePointsToDraw;

        /// <summary>
        /// Constructor method if original edge data is a List of Edges
        /// </summary>
        /// <param name="imagePointsToDraw">A List of Edges representing the edges</param>
        /// <param name="imageWidth">The width of the image</param>
        /// <param name="imageHeight">The height of the image</param> 
        public EdgeEdgesImage(List<Edge> imagePointsToDraw, int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.imagePointsToDraw = imagePointsToDraw;
        }

        internal override void drawEdges()
        {
            List<Point> currentEdgePoints = new List<Point>();
            Point currentPoint;
            int currentEdgeLength;

            currentColor = 0;

            for (int i = 0; i < imagePointsToDraw.Count; i++)
            {
                currentEdgePoints = imagePointsToDraw[i].Points;
                currentEdgeLength = currentEdgePoints.Count;

                Color currentEdgeColor;

                if (drawMultiColouredEdges == false)
                    currentEdgeColor = edgeColour;
                else
                {
                    currentColor++;

                    if (currentColor >= multiEdgeColours.Length)
                        currentColor = 0;

                    currentEdgeColor = multiEdgeColours[currentColor];
                }

                for (int j = 0; j < currentEdgeLength; j++)
                {
                    currentPoint = currentEdgePoints[j];

                    drawnImage.SetPixel(currentPoint.X, currentPoint.Y, currentEdgeColor);
                }
            }
        }

    }
}
