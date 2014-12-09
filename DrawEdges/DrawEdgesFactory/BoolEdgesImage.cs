using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrawEdges.DrawEdgesFactory
{
    public class BoolEdgesImage : DrawEdgesImage
    {
        private bool[] edgeData;

        /// <summary>
        /// Constructor method if original edge data is a bool[] array 
        /// </summary>
        /// <param name="edgeData">A bool[] array representing the edges</param>
        /// <param name="imageWidth">The width of the image</param>
        /// <param name="imageHeight">The height of the image</param>
        public BoolEdgesImage(bool[] edgeData, int imageWidth, int imageHeight)
        {
            this.edgeData = edgeData;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
        }

        internal override void drawEdges()
        {
            for (int width = 0; width < imageWidth; width++)
            {
                for (int height = 0; height < imageHeight; height++)
                {
                    if (edgeData[width + (height * imageWidth)] == true)
                    {
                        //if (drawMultiColouredEdges == false)
                            drawnImage.SetPixel(width, height, edgeColour);
                        //else
                        //{
                        //    currentColor++;

                        //    if (currentColor >= multiEdgeColours.Length)
                        //        currentColor = 0;

                        //    edgeColour = multiEdgeColours[currentColor];

                        //    drawnImage.SetPixel(width, height, edgeColour);
                      //  }
                    }
                }
            }
        }
    }

}
