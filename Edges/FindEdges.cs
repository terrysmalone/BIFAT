using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Edges
{
    /// <summary>
    /// A class which searches an array of boolean data for edges and creates a List of
    /// Edge objects
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1
    /// </summary>
    public class FindEdges
    {
        private List<Edge> edges = new List<Edge>();
        private Edge currentEdge;
        private List<Point> currentEdgePoints = new List<Point>();

        private int imageWidth, imageHeight;

        private bool[] pixelChecked;
        private bool[] edgeData;

        private bool horizontalWrap = true;

        private int currentEdgeLength;

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="edgeData">The boolean array to check for edges in</param>
        /// <param name="imageWidth">The width of the source image</param>
        /// <param name="imageHeight">The height of the source image</param>
        public FindEdges(bool[] edgeData, int imageWidth, int imageHeight)
        {
            this.edgeData = edgeData;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            pixelChecked = new bool[imageWidth * imageHeight];
        }

        /// <summary>
        /// Method which checks the source bool[] data and returns an arrayList of separate
        /// edges
        /// </summary>
        public void find()
        {
            for (int yPos = 0; yPos < imageHeight; yPos++)
            {
                for (int xPos = 0; xPos < imageWidth; xPos++)
                {
                    checkForNewEdge(xPos, yPos);
                }
            }
        }

        /// <summary>
        /// Checks if there is a new edge at the given x, y pos
        /// </summary>
        /// <param name="xPos">The x postion of the pixel</param>
        /// <param name="yPos">The x postion of the pixel</param>
        private void checkForNewEdge(int xPos, int yPos)
        {
            int pixelPosition = xPos + (yPos * imageWidth);

            if (edgeData[pixelPosition] == true && pixelChecked[pixelPosition] == false)
            {
                currentEdge = startNewEdge(xPos, yPos);

                currentEdge.SortEdge();
                edges.Add(currentEdge);
            }
        }
        /// <summary>
        /// Method which creates an edge and calculates all its points
        /// </summary>
        /// <param name="xPos">The start x point of the found edge</param>
        /// <param name="yPos">The start y point of the found edge</param>
        /// <returns>The complete edge</returns>
        private Edge startNewEdge(int xPos, int yPos)
        {
            currentEdge = new Edge(imageWidth);

            checkNeighbours(xPos, yPos);

            return currentEdge;
        }

        # region checkNeighbours methods

        /// <summary>
        /// Method which adds a point to the current points ArrayList, marks
        /// it as checked and iteratively checks all the points
        /// neighbours to see if it is connected to an edge
        /// </summary>
        /// <param name="currentX">The current points x position</param>
        /// <param name="currentY">The current points y position</param>
        private void checkNeighbours(int currentX, int currentY)
        {
            currentEdge.AddPoint(currentX, currentY);
            currentEdgeLength++;

            pixelChecked[currentX + (currentY * imageWidth)] = true;

            checkNorthWest(currentX, currentY);
            checkWest(currentX, currentY);
            checkSouthWest(currentX, currentY);
            checkSouth(currentX, currentY);
            checkSouthEast(currentX, currentY);
            checkEast(currentX, currentY);
            checkNorthEast(currentX, currentY);
            checkNorth(currentX, currentY);
        }

        /// <summary>
        /// Checks the pixel northwest of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkNorthWest(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX > 0 && currentY > 0)
            {
                neighbourX = currentX - 1;
                neighbourY = currentY - 1;
                check(neighbourX, neighbourY);
            }
            else if (currentX == 0 && currentY > 0)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX + imageWidth - 1;
                    neighbourY = currentY - 1;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel west of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkWest(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX > 0)
            {
                neighbourX = currentX - 1;
                neighbourY = currentY;
                check(neighbourX, neighbourY);
            }
            else if (currentX == 0)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX + imageWidth - 1;
                    neighbourY = currentY;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel south-west of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkSouthWest(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX > 0 && currentY < imageHeight - 1)
            {
                neighbourX = currentX - 1;
                neighbourY = currentY + 1;
                check(neighbourX, neighbourY);
            }
            else if (currentX == 0 && currentY < imageHeight - 1)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX + imageWidth - 1;
                    neighbourY = currentY + 1;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel south of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkSouth(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentY < imageHeight - 1)
            {
                neighbourX = currentX;
                neighbourY = currentY + 1;
                check(neighbourX, neighbourY);
            }
        }

        /// <summary>
        /// Checks the pixel south-east of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkSouthEast(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX < imageWidth - 1 && currentY < imageHeight - 1)
            {
                neighbourX = currentX + 1;
                neighbourY = currentY + 1;
                check(neighbourX, neighbourY);
            }
            else if (currentX == imageWidth - 1 && currentY < imageHeight - 1)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX - (imageWidth - 1);
                    neighbourY = currentY + 1;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel east of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkEast(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX < imageWidth - 1)
            {
                neighbourX = currentX + 1;
                neighbourY = currentY;
                check(neighbourX, neighbourY);
            }
            else if (currentX == imageWidth - 1)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX - (imageWidth - 1);
                    neighbourY = currentY;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel north-east of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkNorthEast(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentX < imageWidth - 1 && currentY > 0)
            {
                neighbourX = currentX + 1;
                neighbourY = currentY - 1;
                check(neighbourX, neighbourY);
            }
            else if (currentX == imageWidth - 1 && currentY > 0)
            {
                if (horizontalWrap)
                {
                    neighbourX = currentX - (imageWidth - 1);
                    neighbourY = currentY - 1;
                    check(neighbourX, neighbourY);
                }
            }
        }

        /// <summary>
        /// Checks the pixel north of the current pixel
        /// </summary>
        /// <param name="currentX">The X position of the current pixel</param>
        /// <param name="currentY">The Y position of the current pixel</param>
        private void checkNorth(int currentX, int currentY)
        {
            int neighbourX, neighbourY;

            if (currentY > 0)
            {
                neighbourX = currentX;
                neighbourY = currentY - 1;
                check(neighbourX, neighbourY);
            }
        }

        /// <summary>
        /// Method which checks if a point is an edge and if so calls a method to check
        /// its neighbours
        /// </summary>
        /// <param name="pointX">The current points x position</param>
        /// <param name="pointY">The current points y position</param>
        /// <returns>True if there is an adjoining point</returns>
        private bool check(int pointX, int pointY)
        {
            int currentPosition = pointX + (pointY * imageWidth);

            if (edgeData[currentPosition] == true && pixelChecked[currentPosition] == false)
            {
                checkNeighbours(pointX, pointY);

                return true;
            }

            return false;
        }

        # endregion

        # region Set methods

        public void SetHorizontalWrap(bool horizontalWrap)
        {
            this.horizontalWrap = horizontalWrap;
        }

        # endregion

        # region get methods

        /// <summary>
        /// Method which returns the ArrayList of Edges
        /// </summary>
        /// <returns>An ArrayList of all of the edges found in the image</returns>
        public List<Edge> getEdges()
        {
            return edges;
        }

        /// <summary>
        /// Method which returns the number of edges found in the image
        /// </summary>
        /// <returns>The number of edges</returns>
        public int getNumberOfEdges()
        {
            return edges.Count;
        }

        # endregion
    }

}
