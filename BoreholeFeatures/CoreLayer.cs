using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoreholeFeatures
{
    public sealed class CoreLayer : Layer
    {
        public int TopEdgeInterceptMm => Convert.ToInt32(TopEdgeIntercept
                                                         * (double)m_DepthResolution
                                                         + m_SourceStartDepth);

        public int BottomEdgeInterceptMm => Convert.ToInt32(TopEdgeIntercept
                                                            * (double)m_DepthResolution
                                                            + m_SourceStartDepth);

        public int TopEdgeIntercept { get; private set; }

        public double TopEdgeSlope { get; private set; }

        public int BottomEdgeIntercept { get; private set; }

        public double BottomEdgeSlope { get; private set; }

        private readonly LayerLine m_TopLine;
        private readonly LayerLine m_BottomLine;


        public CoreLayer(double firstSlope, 
                         int firstIntercept, 
                         double secondSlope, 
                         int secondIntercept, 
                         int sourceAzimuthResolution, 
                         int depthResolution)
        {
            m_TopDepthPixels = firstIntercept;
            m_BottomDepthPixels = secondIntercept;

            this.m_DepthResolution = depthResolution;
            this.m_SourceAzimuthResolution = sourceAzimuthResolution;

            m_TopLine = new LayerLine(firstSlope, firstIntercept, sourceAzimuthResolution);
            m_BottomLine = new LayerLine(secondSlope, secondIntercept, sourceAzimuthResolution);

            TopEdgeIntercept = m_TopLine.Intercept;
            TopEdgeSlope = m_TopLine.Slope;

            BottomEdgeIntercept = m_BottomLine.Intercept;
            BottomEdgeSlope = m_BottomLine.Slope;

            m_TimeAdded = DateTime.Now;
            m_TimeLastModified = DateTime.Now;

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
            if (edgeToMove == m_FirstEdge)
            {
                //Will not let the layer move below the second edge
                if (m_TopDepthPixels + yMoveBy <= m_BottomDepthPixels)
                {
                    MoveTopEdge(xMoveBy, yMoveBy);
                }
            }
            else if (edgeToMove == m_SecondEdge)
            {
                //Will not let it move above the first edge
                if (m_BottomDepthPixels + yMoveBy >= m_TopDepthPixels)
                {
                    MoveBottomEdge(xMoveBy, yMoveBy);
                }
            }
            else if (edgeToMove == m_BothEdges)
            {
                MoveTopEdge(xMoveBy, yMoveBy);
                MoveBottomEdge(xMoveBy, yMoveBy);
            }

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        private void MoveBottomEdge(int xMoveBy, int yMoveBy)
        {
            //Point point = new Point(bottomLine.Points[0].X + xMoveBy, bottomLine.Points[0].Y + yMoveBy);
            Point point = new Point(0 + xMoveBy, BottomEdgeIntercept + yMoveBy);

            BottomEdgeIntercept = (int)Math.Round(point.Y - (BottomEdgeSlope * (double)point.X));
            m_BottomLine.Change(m_BottomLine.Slope, BottomEdgeIntercept);

            //Effectively the same value topDepthPixels is used for borehole layer but added here to avoid errors
            m_BottomDepthPixels = m_BottomLine.Intercept;
            BottomEdgeIntercept = m_BottomLine.Intercept;

            BottomEdgeSlope = m_BottomLine.Slope;
        }

        private void MoveTopEdge(int xMoveBy, int yMoveBy)
        {    
            Point point = new Point(0 + xMoveBy, TopEdgeIntercept + yMoveBy);

            TopEdgeIntercept = (int)Math.Round(point.Y - (TopEdgeSlope * (double)point.X));
            m_TopLine.Change(m_TopLine.Slope, TopEdgeIntercept);
            
            //Effectively the same value topDepthPixels is used for borehole layer but added here to avoid errors
            m_TopDepthPixels = m_TopLine.Intercept;    
            TopEdgeIntercept = m_TopLine.Intercept;

            TopEdgeSlope = m_TopLine.Slope;
        }

        public void SetTopEdgeSlope(double firstEdgeSlope)
        {
            TopEdgeSlope = firstEdgeSlope;

            m_TopLine.Change(TopEdgeSlope, m_TopLine.Intercept);
            m_TopLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetTopEdgeIntercept(int firstEdgeIntercept)
        {
            this.TopEdgeIntercept = firstEdgeIntercept;

            m_TopLine.Change(m_TopLine.Slope, TopEdgeIntercept);
            
            m_TopLine.CalculatePoints();
            
            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetBottomEdgeSlope(double secondEdgeSlope)
        {
            this.BottomEdgeSlope = secondEdgeSlope;

            m_BottomLine.Change(BottomEdgeSlope, m_BottomLine.Intercept);
            
            m_BottomLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetBottomEdgeIntercept(int secondEdgeIntercept)
        {
            this.BottomEdgeIntercept = secondEdgeIntercept;

            m_BottomLine.Change(m_BottomLine.Slope, BottomEdgeIntercept);
            
            m_BottomLine.CalculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        # region Get methods

        /// <summary>
        /// Returns the y position of the first sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetTopYPoint(int xPoint)
        {
            return m_TopLine.GetY(xPoint);
        }

        /// <summary>
        /// Returns the y position of the second sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetBottomYPoint(int xPoint)
        {
            return m_BottomLine.GetY(xPoint);
        }

        private double GetTopEdgeSlopeMm()
        {
            return TopEdgeSlope * (double)m_DepthResolution;
        }

        private double GetBottomEdgeSlopeMm()
        {
            return BottomEdgeSlope * (double)m_DepthResolution;
        }

        /// <summary>
        /// Returns a string of all the layer's details:
        /// 
        /// Start Y                     - Start Y boundary in pixels
        /// End Y                       - End Y boundary in pixels
        /// First edge intercept        - Intercept of first edge in pixels
        /// First edge slope            - slope of first edge
        /// Second edge intercept       - Intercept of second edge in pixels
        /// Second edge slope           - slope of second edge        
        /// Layer Properties            - Contents of layer (space separated list)
        /// Layer description           - Description of layer
        /// Layer quality               - Quality of the layer (1-4)
        /// Time added                  - Time the layer was first added
        /// Time last modified          - Time the layer was last modified
        /// Group                       - The group the layer belongs to
        /// </summary>
        /// <returns>The layer's details</returns>
        public override string GetDetails()
        {
            //Remove commas from the description
            m_Description = m_Description.Replace(',', ' ');

            //Get the layer properties
            WriteLayerProperties();

            return m_LayerStartY + "," + 
                   m_LayerEndY + "," + 
                   TopEdgeIntercept + "," + 
                   TopEdgeSlope + "," + 
                   BottomEdgeIntercept + "," + 
                   BottomEdgeSlope + "," + 
                   m_LayerType + "," + 
                   m_Description + "," + 
                   m_Quality + "," + 
                   m_TimeAdded + "," + 
                   m_TimeLastModified + "," + 
                   Group;
        }

        public override int GetNumOfEdges()
        {
            if (TopEdgeIntercept == BottomEdgeIntercept && TopEdgeSlope == BottomEdgeSlope)
                return 1;

            return 2;
        }

        /// <summary>
        /// Returns the points of the layers first edge
        /// </summary>
        /// <returns>A List of the points along the first edge</returns>
        public override List<Point> GetTopEdgePoints()
        {
            return m_TopLine.Points;
        }

        /// <summary>
        /// Returns the points of the layers second edge
        /// </summary>
        /// <returns>A List of the points along the second edge</returns>
        public override List<Point> GetBottomEdgePoints()
        {
            return m_BottomLine.Points;
        }

        # endregion
    }
}
