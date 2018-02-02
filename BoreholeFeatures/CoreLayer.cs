using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace BoreholeFeatures
{
    [DefaultPropertyAttribute("Description")]
    public sealed class CoreLayer : Layer
    {
        # region Property grid methods

        [CategoryAttribute("\t\tTop Edge"), DescriptionAttribute("The intercept of the layer's top edge in millimetres")]
        [DisplayName("Intercept (mm)")]
        public int TopEdgeIntercept
        {
            get
            {
                return TopEdgeInterceptMm;
            }

        }

        [CategoryAttribute("\t\tTop Edge"), DescriptionAttribute("The slope of the layer's top edge")]
        [DisplayName("Slope")]
        public double TopEdgeSlope
        {
            get
            {
                return TopEdgeSlope;
            }

        }

        [CategoryAttribute("\tBottom Edge"), DescriptionAttribute("The intercept of the layer's bottom edge in millimetres")]
        [DisplayName("Intercept (mm)")]
        public int BottomEdgeIntercept
        {
            get
            {
                return BottomEdgeInterceptMm;
            }

        }

        [CategoryAttribute("\tBottom Edge"), DescriptionAttribute("The slope of the layer's bottom edge")]
        [DisplayName("Slope")]
        public double BottomEdgeSlope
        {
            get
            {
                return BottomEdgeSlope;
            }
        }

        # endregion

        //private int topEdgeIntercept, bottomEdgeIntercept;
        //private double topEdgeSlope, bottomEdgeSlope;

        private LayerLine topLine;
        private LayerLine bottomLine;

        //private List<Point> topLinePoints;
        //private List<Point> bottomLinePoints;
        
        public CoreLayer(double firstSlope, int firstIntercept, double secondSlope, int secondIntercept, int sourceAzimuthResolution, int depthResolution)
        {
            this.topDepthPixels = firstIntercept;
            this.bottomDepthPixels = secondIntercept;

            this.depthResolution = depthResolution;
            this.sourceAzimuthResolution = sourceAzimuthResolution;

            topLine = new LayerLine(firstSlope, firstIntercept, sourceAzimuthResolution);
            bottomLine = new LayerLine(secondSlope, secondIntercept, sourceAzimuthResolution);

            topEdgeIntercept = topLine.Intercept;
            topEdgeSlope = topLine.Slope;

            bottomEdgeIntercept = bottomLine.Intercept;
            bottomEdgeSlope = bottomLine.Slope;

            timeAdded = DateTime.Now;
            timeLastModified = DateTime.Now;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Moves one or both of the layers edges 
        /// </summary>
        /// <param name="sineToMove">The edge to move: 1 or 2 (3 for both)</param>
        /// <param name="xMoveBy">The amount to move along the x-axis in pixels</param>
        /// <param name="yMoveBy">The amount to move along the y-axis in pixels</param>
        public override void MoveEdge(int edgeToMove, int xMoveBy, int yMoveBy)
        {
            if (edgeToMove == FIRST_EDGE)
            {
                //Will not let the layer move below the second edge
                if (topDepthPixels + yMoveBy <= bottomDepthPixels)
                {
                    MoveTopEdge(xMoveBy, yMoveBy);
                }
            }
            else if (edgeToMove == SECOND_EDGE)
            {
                //Will not let it move above the first edge
                if (bottomDepthPixels + yMoveBy >= topDepthPixels)
                {
                    MoveBottomEdge(xMoveBy, yMoveBy);
                }
            }
            else if (edgeToMove == BOTH_EDGES)
            {
                MoveTopEdge(xMoveBy, yMoveBy);
                MoveBottomEdge(xMoveBy, yMoveBy);
            }

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        private void MoveBottomEdge(int xMoveBy, int yMoveBy)
        {
            //Point point = new Point(bottomLine.Points[0].X + xMoveBy, bottomLine.Points[0].Y + yMoveBy);
            Point point = new Point(0 + xMoveBy, bottomEdgeIntercept + yMoveBy);

            bottomEdgeIntercept = (int)Math.Round(point.Y - (bottomEdgeSlope * (double)point.X));
            bottomLine.Change(bottomLine.Slope, bottomEdgeIntercept);

            //Effectively the same value topDepthPixels is used for borehole layer but added here to avoid errors
            bottomDepthPixels = bottomLine.Intercept;
            bottomEdgeIntercept = bottomLine.Intercept;

            bottomEdgeSlope = bottomLine.Slope;
        }

        private void MoveTopEdge(int xMoveBy, int yMoveBy)
        {    
            Point point = new Point(0 + xMoveBy, topEdgeIntercept + yMoveBy);

            topEdgeIntercept = (int)Math.Round(point.Y - (topEdgeSlope * (double)point.X));
            topLine.Change(topLine.Slope, topEdgeIntercept);
            
            //Effectively the same value topDepthPixels is used for borehole layer but added here to avoid errors
            topDepthPixels = topLine.Intercept;    
            topEdgeIntercept = topLine.Intercept;

            topEdgeSlope = topLine.Slope;
        }

        public override void ChangeTopAmplitudeBy(int amount)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        public override void ChangeBottomAmplitudeBy(int amount)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        # region set methods

        public override void SetTopEdgeDepth(int topDepthPixels)
        {
            //throw new NotImplementedException();
        }

        public override void SetTopSineAzimuth(int topAzimuth)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        public override void SetTopSineAmplitude(int topAmplitude)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        public override void SetBottomEdgeDepth(int bottomDepthPixels)
        {
            //throw new NotImplementedException();
        }

        public override void SetBottomSineAzimuth(int bottomtopAzimuth)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        public override void SetBottomSineAmplitude(int bottomAmplitude)
        {
            //Not implementable in core layer
            //throw new NotImplementedException();
        }

        public override void SetTopEdgeSlope(double firstEdgeSlope)
        {
            this.topEdgeSlope = firstEdgeSlope;

            topLine.Change(topEdgeSlope, topLine.Intercept);
            topLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetTopEdgeIntercept(int firstEdgeIntercept)
        {
            this.topEdgeIntercept = firstEdgeIntercept;

            topLine.Change(topLine.Slope, topEdgeIntercept);
            
            topLine.CalculatePoints();
            
            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetBottomEdgeSlope(double secondEdgeSlope)
        {
            this.bottomEdgeSlope = secondEdgeSlope;

            bottomLine.Change(bottomEdgeSlope, bottomLine.Intercept);
            
            bottomLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetBottomEdgeIntercept(int secondEdgeIntercept)
        {
            this.bottomEdgeIntercept = secondEdgeIntercept;

            bottomLine.Change(bottomLine.Slope, bottomEdgeIntercept);

            //topLine.CalculatePoints();
            bottomLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        # endregion

        # region Get methods

        /// <summary>
        /// Returns the y position of the first sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetTopYPoint(int xPoint)
        {
            return topLine.GetY(xPoint);
        }

        /// <summary>
        /// Returns the y position of the second sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int getBottomYPoint(int xPoint)
        {
            return bottomLine.GetY(xPoint);
        }

        private double GetTopEdgeSlopeMM()
        {
            return topEdgeSlope * (double)depthResolution;
        }

        private double GetBottomEdgeSlopeMM()
        {
            return bottomEdgeSlope * (double)depthResolution;
        }

        /// <summary>
        /// Returns a string of all the layer's details:
        /// 
        /// Start Y                     - Start Y boundary in pixels
        /// End Y                       - End Y boundary in pixels
        /// First edge intercept        - Intercept of first edge in pixels
        /// First edge slope            - slope of first edge
        /// Second edge intercept        - Intercept of second edge in pixels
        /// Second edge slope            - slope of second edge        
        /// Layer Type                  - Contents of layer (space separated list)
        /// Layer description           - Description of layer
        /// Layer quality               - Quality of the layer (1-4)
        /// Time added                  - Time the layer was first added
        /// Time last modified          - Time the layer was last modified
        /// Group                       - The group the layer belongs to
        /// </summary>
        /// <returns>The layer's details</returns>
        public override String GetDetails()
        {
            String details;

            //Remove commas from the description
            description = description.Replace(',', ' ');

            //Get the layer type
            WriteLayerProperties();

            details = layerStartY + "," + layerEndY + "," + topEdgeIntercept + "," + topEdgeSlope + "," + bottomEdgeIntercept + "," + bottomEdgeSlope + "," + layerType + "," + description + "," + quality + "," + timeAdded + "," + timeLastModified + "," + Group;

            return details;
        }

        public override int GetNumOfEdges()
        {
            if (topEdgeIntercept == bottomEdgeIntercept && topEdgeSlope == bottomEdgeSlope)
                return 1;
            else
                return 2;
        }

        /// <summary>
        /// Returns the points of the layers first edge
        /// </summary>
        /// <returns>A List of the points along the first edge</returns>
        public override List<Point> GetTopEdgePoints()
        {
            return topLine.Points;
        }

        /// <summary>
        /// Returns the points of the layers second edge
        /// </summary>
        /// <returns>A List of the points along the second edge</returns>
        public override List<Point> GetBottomEdgePoints()
        {
            return bottomLine.Points;
        }

        # endregion
    }
}
