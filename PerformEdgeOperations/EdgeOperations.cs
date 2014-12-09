using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Edges;
using DrawEdges.DrawEdgesFactory;

namespace PerformEdgeOperations
{
    /// <summary>
    /// Deals with the setting up and running of all edge processing operations.
    /// 
    /// Author: Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public class EdgeOperations
    {
        private int imageWidth, imageHeight;

        private bool edgeLinking, edgeRemoval, edgeJoining;

        private Bitmap originalImage;

        # region DefaultParameters

        private int edgeLinkingThreshold = 260;
        private int edgeLinkingMaxNeighbourDistance = 5;
        private int edgeRemovalThreshold = 320;
        private int edgeJoiningThreshold = 20;

        # endregion

        private bool[] edgeData;
        private List<Edge> foundEdges;
        private List<Edge> edgesBeforeOperations;
        private List<Edge> edgesAfterLink;
        private List<Edge> edgesAfterRemoval;
        private List<Edge> edgesAfterJoin;

        private bool horizontalWrap = true;

        private bool drawTestImages = false;
        private Color testDrawingBackgroundColour, testDrawingEdgeColour, testDrawingOverBackgroundColour;
        private bool testDrawMultiColouredEdges = false;

        private List<Edge> edges = new List<Edge>();

        # region Properties

        # region Edge detection parameters

        public int EdgeLinkingThreshold
        {
            get
            {
                return edgeLinkingThreshold;
            }
            set
            {
                edgeLinkingThreshold = value;
            }
        }

        public int EdgeLinkingDistance
        {
            get
            {
                return edgeLinkingMaxNeighbourDistance;
            }
            set
            {
                edgeLinkingMaxNeighbourDistance = value;
            }
        }

        public int EdgeRemovalThreshold
        {
            get
            {
                return edgeRemovalThreshold;
            }
            set
            {
                edgeRemovalThreshold = value;
            }
        }

        public int EdgeJoiningThreshold
        {
            get
            {
                return edgeJoiningThreshold;
            }
            set
            {
                edgeJoiningThreshold = value;
            }
        }

        # endregion

        public bool HorizontalWrap
        {
            get
            {
                return horizontalWrap;
            }
            set
            {
                horizontalWrap = value;
            }
        }

        public List<Edge> FoundEdges
        {
            get
            {
                return edgesAfterJoin;
            }
        }

        /// <summary>
        /// Returns a List of the Edges found by the canny edge detector before any operations were performed
        /// </summary>
        public List<Edge> EdgesBeforeOperations
        {
            get
            {
                return edgesBeforeOperations;
            }
        }

        # region Test properties

        public bool DrawTestImages
        {
            get
            {
                return drawTestImages;
            }
            set
            {
                drawTestImages = value;
            }
        }

        public Color TestDrawingBackgroundColour
        {
            get
            {
                return testDrawingBackgroundColour;
            }
            set
            {
                testDrawingBackgroundColour = value;
            }
        }

        public Color TestDrawingEdgeColour 
        { 
            get
            {
                return testDrawingEdgeColour;
            }
            set
            {
                testDrawingEdgeColour = value;
            }
       }

        public Color TestDrawingOverBackgroundColour 
        { 
            get
            {
                return testDrawingOverBackgroundColour;
            }
            set
            {
                testDrawingOverBackgroundColour = value;
            }
        }

        public bool TestDrawMultiColouredEdges 
        { 
            get
            {
                return testDrawMultiColouredEdges;
            }
            set
            {
                testDrawMultiColouredEdges = value;
            }
        }

        #endregion

        # endregion

        # region Constructor methods

        /// <summary>
        /// Constructor method which takes an int[] array as input 
        /// </summary>
        /// <param name="edgeData">The bool[] representation of the edges to perform the edge operations on</param>
        public EdgeOperations(bool[] edgeData, int imageWidth, int imageHeight)
        {
            this.edgeData = edgeData;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            initialiseDefaults();
        }

        /// <summary>
        /// Initialises all the operations and their values
        /// </summary>
        private void initialiseDefaults()
        {
            edgeLinking = true;
            edgeRemoval = true;
            edgeJoining = true;
        }

        public void SetTestImage(Bitmap originalImage)
        {
            this.originalImage = originalImage;
        }


        # endregion

        # region Edge detection methods

        public void Run()
        {
            findEdges();

            linkEdges();

            RemoveEdges();

            JoinEdges();
        }

        private void findEdges()
        {
            foundEdges = new List<Edge>();

            FindEdges findEdges = new FindEdges(edgeData, imageWidth, imageHeight);

            findEdges.SetHorizontalWrap(horizontalWrap);
            findEdges.find();

            foundEdges = findEdges.getEdges();

            edgesBeforeOperations = findEdges.getEdges();

            if (drawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(foundEdges, imageWidth, imageHeight);

                drawImage.setBackgroundColour(testDrawingBackgroundColour);
                drawImage.setEdgeColour(testDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(testDrawMultiColouredEdges);
                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("02a - Found edges.bmp");

                drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(originalImage);
                drawImage.getDrawnEdges().Save("02b - Found edges over original.bmp");
            }
        }

        private void linkEdges()
        {
            if (edgeLinking)
            {
                EdgeLinking linkEdges = new EdgeLinking(foundEdges, imageWidth, imageHeight, edgeLinkingThreshold);
                linkEdges.HorizontalWrap = horizontalWrap;
                linkEdges.MaximumNeighbourDistance = edgeLinkingMaxNeighbourDistance;
                linkEdges.Link();

                edgesAfterLink = linkEdges.LinkedEdges;

                if (drawTestImages)
                {
                    DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                    DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterLink, imageWidth, imageHeight);

                    drawImage.setBackgroundColour(testDrawingBackgroundColour);
                    drawImage.setEdgeColour(testDrawingEdgeColour);
                    drawImage.SetDrawMultiColouredEdges(testDrawMultiColouredEdges);
                    drawImage.drawEdgesImage();
                    drawImage.getDrawnEdges().Save("03a - After edge linking(" + edgeLinkingThreshold + ").bmp");

                    drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                    drawImage.drawEdgesOverBackgroundImage(originalImage);
                    drawImage.getDrawnEdges().Save("03b - After edge linking over original(" + edgeLinkingThreshold + ").bmp");
                }
            }
            else
                edgesAfterLink = foundEdges;
        }

        private void RemoveEdges()
        {
            if (edgeRemoval)
            {
                RemoveEdges removeEdges = new RemoveEdges(edgesAfterLink, edgeRemovalThreshold);
                removeEdges.removeEdges();

                edgesAfterRemoval = removeEdges.getEdges();

                if (drawTestImages)
                {
                    DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                    DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterRemoval, imageWidth, imageHeight);

                    drawImage.setBackgroundColour(testDrawingBackgroundColour);
                    drawImage.setEdgeColour(testDrawingEdgeColour);
                    drawImage.SetDrawMultiColouredEdges(testDrawMultiColouredEdges);
                    drawImage.drawEdgesImage();
                    drawImage.getDrawnEdges().Save("04a - After edge removal(" + edgeRemovalThreshold + ").bmp");

                    drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                    drawImage.drawEdgesOverBackgroundImage(originalImage);
                    drawImage.getDrawnEdges().Save("04b - After edge removal over original(" + edgeRemovalThreshold + ").bmp");
                }
            }
            else
                edgesAfterRemoval = edgesAfterLink;
        }

        private void JoinEdges()
        {
            if (edgeJoining)
            {
                JoinEdges joinEdges = new JoinEdges(edgesAfterRemoval, edgeJoiningThreshold, imageWidth, imageHeight);
                joinEdges.SetHorizontalWrap(horizontalWrap);
                joinEdges.join();

                edgesAfterJoin = joinEdges.getJoinedEdges();

                if (drawTestImages)
                {
                    DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                    DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterJoin, imageWidth, imageHeight);

                    drawImage.setBackgroundColour(testDrawingBackgroundColour);
                    drawImage.setEdgeColour(testDrawingEdgeColour);
                    drawImage.SetDrawMultiColouredEdges(testDrawMultiColouredEdges);
                    drawImage.drawEdgesImage();
                    drawImage.getDrawnEdges().Save("05a - After edge joining(" + edgeJoiningThreshold + ").bmp");

                    drawImage.setEdgeColour(testDrawingOverBackgroundColour);
                    drawImage.drawEdgesOverBackgroundImage(originalImage);
                    drawImage.getDrawnEdges().Save("05b - After edge joining over original(" + edgeJoiningThreshold + ").bmp");
                }
            }
            else
                edgesAfterJoin = edgesAfterRemoval;
        }

        private Bitmap drawEdgesOverBlankImageImage()
        {
            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
            DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterJoin, imageWidth, imageHeight);

            drawImage.setBackgroundColour(testDrawingBackgroundColour);
            drawImage.setEdgeColour(testDrawingEdgeColour);
            drawImage.SetDrawMultiColouredEdges(testDrawMultiColouredEdges);

            drawImage.drawEdgesImage();
            Bitmap image = drawImage.getDrawnEdges();

            return image;
        }

        private Bitmap drawEdgesOverOriginalImage()
        {
            DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
            DrawEdgesImage drawImage = factory.setUpDrawEdges(edgesAfterJoin, imageWidth, imageHeight);

            drawImage.setEdgeColour(testDrawingOverBackgroundColour);
            drawImage.drawEdgesOverBackgroundImage(originalImage);
            Bitmap image = drawImage.getDrawnEdges();

            return image;
        }

        # endregion

        #region Get methods

        public Bitmap getEdgesImageOverBlank()
        {
            Bitmap edgesOverBlankImage = drawEdgesOverBlankImageImage();

            return edgesOverBlankImage;
        }

        public Bitmap getEdgesImageOverBackground()
        {
            Bitmap edgesOverOriginalImage = drawEdgesOverOriginalImage();

            return edgesOverOriginalImage;
        }

        # endregion
    }

}
