using System;
using System.Collections.Generic;
using System.Drawing;

namespace BoreholeFeatures
{
    public sealed class BoreholeLayer : Layer
    {
        public int TopSineAmplitude { get; private set; }

        public int TopSineAmplitudeInMm => (int)(TopSineAmplitude * (double)m_DepthResolution);

        public int TopSineAzimuth { get; private set; }

        public int TopSineAzimuthInMm => (int)(BottomSineAzimuth * (double)m_DepthResolution);

        public int SecondSineDepth => BottomEdgeDepthMm;
        
        public int BottomSineAzimuth { get; private set; }

        public int BottomSineAzimuthInMm => (int)(BottomSineAzimuth * (double)m_DepthResolution);

        public int BottomSineAmplitude { get; private set; }

        public int BottomSineAmplitudeInMm => (int)(BottomSineAmplitude * (double)m_DepthResolution);
        
        private readonly SineWave m_TopSine;

        private readonly SineWave m_BottomSine;

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="firstDepth">The depth of the first SineWave</param>
        /// <param name="firstAmplitude">The amplitude of the first SineWave</param>
        /// <param name="firstAzimuth">The azimuth of the first SineWave</param>
        /// <param name="secondDepth">The depth of the second SineWave</param>
        /// <param name="secondAmplitude">The amplitude of the second SineWave</param>
        /// <param name="secondAzimuth">The azimuth of the second SineWave</param>
        /// <param name="sourceAzimuthResolution">The azimuth resolution of the borehole</param>
        /// <param name="depthResolution">The depth resolution of the borehole</param>
        public BoreholeLayer(int firstDepth, 
                             int firstAmplitude, 
                             int firstAzimuth, 
                             int secondDepth, 
                             int secondAmplitude, 
                             int secondAzimuth, 
                             int sourceAzimuthResolution, 
                             int depthResolution)
        {
            m_TopDepthPixels = firstDepth;
            TopSineAmplitude = firstAmplitude;
            TopSineAzimuth = firstAzimuth;
            m_BottomDepthPixels = secondDepth;
            BottomSineAmplitude = secondAmplitude;
            BottomSineAzimuth = secondAzimuth;
            m_SourceAzimuthResolution = sourceAzimuthResolution;
            m_DepthResolution = depthResolution;

            m_TopSine = new SineWave(firstDepth, firstAzimuth, firstAmplitude, sourceAzimuthResolution);
            m_BottomSine = new SineWave(secondDepth, secondAzimuth, secondAmplitude, sourceAzimuthResolution);
            
            m_TimeAdded = DateTime.Now;
            m_TimeLastModified = DateTime.Now;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Changes the amplitude of top (lowest y value) sine of the layer by a specified amount
        /// </summary>
        public void ChangeTopAmplitudeBy(int changeAmplitudeBy)
        {
            TopSineAmplitude = TopSineAmplitude + changeAmplitudeBy;

            if (TopSineAmplitude < 0)
                TopSineAmplitude = 0;

            m_TopSine.change(m_TopDepthPixels, TopSineAzimuth, TopSineAmplitude);
            
            m_TimeLastModified = DateTime.Now;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Changes the amplitude of bottom (highest y value) sine of the layer by a specified amount
        /// </summary>
        public void ChangeBottomAmplitudeBy(int changeAmplitudeBy)
        {
            BottomSineAmplitude = BottomSineAmplitude + changeAmplitudeBy;

            if (BottomSineAmplitude < 0)
                BottomSineAmplitude = 0;

            m_BottomSine.change(m_BottomDepthPixels, BottomSineAzimuth, BottomSineAmplitude);
            
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
        public override void MoveEdge(int sineToMove, int xMoveBy, int yMoveBy)
        {
            if (sineToMove == m_FirstEdge)
            {
                //Will not let the layer move below the second edge
                if (m_TopDepthPixels + yMoveBy <= m_BottomDepthPixels)
                {
                    MoveFirstSine(xMoveBy, yMoveBy);
                }
            }
            else if (sineToMove == m_SecondEdge)
            {
                //Will not let it move above the first edge
                if (m_BottomDepthPixels + yMoveBy >= m_TopDepthPixels)
                {
                    MoveSecondSine(xMoveBy, yMoveBy);
                }
            }
            else if (sineToMove == m_BothEdges)
            {
                MoveFirstSine(xMoveBy, yMoveBy);
                MoveSecondSine(xMoveBy, yMoveBy);
            }

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Moves the layers first sine by the specified amount
        /// </summary>
        /// <param name="xMoveBy">Amount to move along the x-axis</param>
        /// <param name="yMoveBy">Amount to move along the y-axis</param>
        private void MoveFirstSine(int xMoveBy, int yMoveBy)
        {
            m_TopDepthPixels = m_TopDepthPixels + yMoveBy;
            TopSineAzimuth = TopSineAzimuth + (int)(xMoveBy / (m_SourceAzimuthResolution / 360.0));

            if (TopSineAzimuth > 360)
                TopSineAzimuth -= 360;
            else if (TopSineAzimuth < 0)
                TopSineAzimuth += 360;

            m_TopSine.change(m_TopDepthPixels, TopSineAzimuth, TopSineAmplitude);
        }

        /// <summary>
        /// Moves the layers second sine by the specified amount
        /// </summary>
        /// <param name="xMoveBy">Amount to move along the x-axis</param>
        /// <param name="yMoveBy">Amount to move along the y-axis</param>
        private void MoveSecondSine(int xMoveBy, int yMoveBy)
        {
            m_BottomDepthPixels = m_BottomDepthPixels + yMoveBy;
            BottomSineAzimuth = BottomSineAzimuth + (int)(xMoveBy / (m_SourceAzimuthResolution / (double)360));

            if (BottomSineAzimuth > 360)
                BottomSineAzimuth -= 360;
            else if (BottomSineAzimuth < 0)
                BottomSineAzimuth += 360;

            m_BottomSine.change(m_BottomDepthPixels, BottomSineAzimuth, BottomSineAmplitude);
        }

        # region Get methods

        /// <summary>
        /// Returns the y position of the first sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetTopYPoint(int xPoint)
        {
            return m_TopSine.getY(xPoint);
        }

        /// <summary>
        /// Returns the y position of the second sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetBottomYPoint(int xPoint)
        {
            return m_BottomSine.getY(xPoint);
        }

        /// <summary>
        /// Returns a string of all the layer's details:
        /// 
        /// Start Y                     - Start Y boundary in pixels
        /// End Y                       - End Y boundary in pixels
        /// First edge depth in pixels  - Depth of first edge in pixels
        /// First edge azimuth          - azimuth of first edge (0-360)
        /// First edge amplitude        - amplitude of first edge in pixels 
        /// Second edge depth in pixels - Depth of second edge in pixels
        /// Second edge azimuth         - azimuth of second edge (0-360)
        /// Second edge amplitude       - amplitude of second edge in pixels 
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
                   m_TopDepthPixels + "," + 
                   TopSineAzimuth + "," + 
                   TopSineAmplitude + "," + 
                   m_BottomDepthPixels + "," + 
                   BottomSineAzimuth + "," + 
                   BottomSineAmplitude + "," + 
                   m_LayerType + "," + 
                   m_Description + "," + 
                   m_Quality + "," + 
                   m_TimeAdded + "," + 
                   m_TimeLastModified + "," + 
                   Group;            
        }

        public override int GetNumOfEdges()
        {
            if(TopSineAmplitude == BottomSineAmplitude &&
               TopSineAzimuth == BottomSineAzimuth &&
               m_TopDepthPixels == m_BottomDepthPixels)
            {
                return 1;
            }

            return 2;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the points of the layers first edge
        /// </summary>
        public override List<Point> GetTopEdgePoints()
        {
            return m_TopSine.getSinePoints();
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the points of the layers second edge
        /// </summary>
        public override List<Point> GetBottomEdgePoints()
        {
            return m_BottomSine.getSinePoints();
        }

        # endregion

        # region Set methods

        public void SetTopEdgeDepth(int topDepth)
        {
            m_TopDepthPixels = topDepth;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetBottomEdgeDepth(int bottomDepthPixels)
        {
            m_BottomDepthPixels = bottomDepthPixels;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetTopSineAmplitude(int topAmplitude)
        {
            TopSineAmplitude = topAmplitude;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetBottomSineAmplitude(int bottomAmplitude)
        {
            BottomSineAmplitude = bottomAmplitude;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetTopSineAzimuth(int topAzimuth)
        {
            TopSineAzimuth = topAzimuth;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        public void SetBottomSineAzimuth(int bottomAzimuth)
        {
            BottomSineAzimuth = bottomAzimuth;

            m_TopSine.calculatePoints();
            m_BottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            m_TimeLastModified = DateTime.Now;
        }

        # endregion
    }
}
