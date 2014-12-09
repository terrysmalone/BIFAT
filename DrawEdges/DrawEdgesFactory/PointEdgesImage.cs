using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawEdges.DrawEdgesFactory
{
    public class PointEdgesImage : DrawEdgesImage
    {
        private List<Point> imagePointsToDraw;
        /// <summary>
        /// Constructor method if original edge data is a List of ImagePoints
        /// </summary>
        /// <param name="imagePointsToDraw">A List of ImagePoints representing the edges</param>
        /// <param name="imageWidth">The width of the image</param>
        /// <param name="imageHeight">The height of the image</param>        
        public PointEdgesImage(List<Point> imagePointsToDraw, int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            this.imagePointsToDraw = imagePointsToDraw;
        }

        internal override void drawEdges()
        {
            Point currentPoint;

            for (int i = 0; i < imagePointsToDraw.Count; i++)
            {
                currentPoint = imagePointsToDraw[i];

                if (drawMultiColouredEdges == false)
                    drawnImage.SetPixel(currentPoint.X, currentPoint.Y, edgeColour);
                else
                {
                    currentColor++;

                    if (currentColor >= multiEdgeColours.Length)
                        currentColor = 0;

                    edgeColour = multiEdgeColours[currentColor];

                    drawnImage.SetPixel(currentPoint.X, currentPoint.Y, edgeColour);
                }
            }
        }
    }
}
