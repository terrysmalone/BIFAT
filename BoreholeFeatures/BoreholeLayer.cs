using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace BoreholeFeatures
{
    [DefaultPropertyAttribute("Description")]
    public sealed class BoreholeLayer : Layer
    {
        # region property grid methods

        [CategoryAttribute("\tBottom Edge"), DescriptionAttribute("The amplitude of the layer's bottom edge")]
        [DisplayName("Amplitude (mm)")]
        public int SecondSineAmplitude
        {
            get
            {
                return BottomSineAmplitudeInMM;
            }

        }

        [CategoryAttribute("\tBottom Edge"), DescriptionAttribute("The azimuth of the layer's bottom edge")]
        [DisplayName("Azimuth")]
        public int SecondSineAzimuth
        {
            get
            {
                return bottomAzimuth;
            }

        }

        [CategoryAttribute("\t\tTop Edge"), DescriptionAttribute("The depth of the layer's top edge")]
        [DisplayName("Depth (mm)")]
        public int FirstSineDepth
        {
            get
            {
                return TopEdgeDepthMM;
                //return (int)((double)sourceStartDepth + (double)((double)topDepthPixels * (double)depthResolution));
                //return (int)(((double)topDepthPixels - (double)sourceStartDepth) * (double)depthResolution) + sourceStartDepth;
            }

        }

        [CategoryAttribute("\t\tTop Edge"), DescriptionAttribute("The amplitude of the layer's top edge")]
        [DisplayName("Amplitude (mm)")]
        public int FirstSineAmplitude
        {
            get
            {
                //return (int)((double)topAmplitude * (double)depthResolution);
                return TopSineAmplitudeInMM;
            }

        }

        [CategoryAttribute("\t\tTop Edge"), DescriptionAttribute("The azimuth of the layer's top edge")]
        [DisplayName("Azimuth")]
        public int FirstSineAzimuth
        {
            get
            {
                return topAzimuth;
            }

        }

        [CategoryAttribute("\tBottom Edge"), DescriptionAttribute("The depth of the layer's bottom edge")]
        [DisplayName("Depth (mm)")]
        public int SecondSineDepth
        {
            get
            {
                return BottomEdgeDepthMM;
                //return (int)((double)sourceStartDepth + (double)((double)bottomDepthPixels * (double)depthResolution));
                //return (int)(((double)bottomDepthPixels - (double)sourceStartDepth) * (double)depthResolution) + sourceStartDepth;
            }

        }

        # endregion

        private SineWave topSine;
        private SineWave bottomSine;
        
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
        public BoreholeLayer(int firstDepth, int firstAmplitude, int firstAzimuth, int secondDepth, int secondAmplitude, int secondAzimuth, int sourceAzimuthResolution, int depthResolution)
        {
            this.topDepthPixels = firstDepth;
            this.topAmplitude = firstAmplitude;
            this.topAzimuth = firstAzimuth;
            this.bottomDepthPixels = secondDepth;
            this.bottomAmplitude = secondAmplitude;
            this.bottomAzimuth = secondAzimuth;
            this.sourceAzimuthResolution = sourceAzimuthResolution;
            this.depthResolution = depthResolution;

            topSine = new SineWave(firstDepth, firstAzimuth, firstAmplitude, sourceAzimuthResolution);
            bottomSine = new SineWave(secondDepth, secondAzimuth, secondAmplitude, sourceAzimuthResolution);
            
            timeAdded = DateTime.Now;
            timeLastModified = DateTime.Now;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Changes the amplitude of top (lowest y value) sine of the layer by a specified amount
        /// </summary>
        /// <param name="changeAmplitudeBy">The amount to change the amplitude by</param>
        public override void ChangeTopAmplitudeBy(int changeAmplitudeBy)
        {
            topAmplitude = topAmplitude + changeAmplitudeBy;

            if (topAmplitude < 0)
                topAmplitude = 0;

            topSine.change(topDepthPixels, topAzimuth, topAmplitude);
            
            timeLastModified = DateTime.Now;

            CalculateStartY();
            CalculateEndY();
        }

        /// <summary>
        /// Changes the amplitude of bottom (highest y value) sine of the layer by a specified amount
        /// </summary>
        /// <param name="changeAmplitudeBy">The amount to change the amplitude by</param>
        public override void ChangeBottomAmplitudeBy(int changeAmplitudeBy)
        {
            bottomAmplitude = bottomAmplitude + changeAmplitudeBy;

            if (bottomAmplitude < 0)
                bottomAmplitude = 0;

            bottomSine.change(bottomDepthPixels, bottomAzimuth, bottomAmplitude);
            
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
        public override void MoveEdge(int sineToMove, int xMoveBy, int yMoveBy)
        {
            if (sineToMove == FIRST_EDGE)
            {
                //Will not let the layer move below the second edge
                if (topDepthPixels + yMoveBy <= bottomDepthPixels)
                {
                    moveFirstSine(xMoveBy, yMoveBy);
                }
            }
            else if (sineToMove == SECOND_EDGE)
            {
                //Will not let it move above the first edge
                if (bottomDepthPixels + yMoveBy >= topDepthPixels)
                {
                    moveSecondSine(xMoveBy, yMoveBy);
                }
            }
            else if (sineToMove == BOTH_EDGES)
            {
                moveFirstSine(xMoveBy, yMoveBy);
                moveSecondSine(xMoveBy, yMoveBy);
            }

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        /// <summary>
        /// Moves the layers first sine by the specified amount
        /// </summary>
        /// <param name="xMoveBy">Amount to move along the x-axis</param>
        /// <param name="yMoveBy">Amount to move along the y-axis</param>
        private void moveFirstSine(int xMoveBy, int yMoveBy)
        {
            topDepthPixels = topDepthPixels + yMoveBy;
            topAzimuth = topAzimuth + (int)((double)xMoveBy / ((double)sourceAzimuthResolution / 360.0));

            if (topAzimuth > 360)
                topAzimuth -= 360;
            else if (topAzimuth < 0)
                topAzimuth += 360;

            topSine.change(topDepthPixels, topAzimuth, topAmplitude);
        }

        /// <summary>
        /// Moves the layers second sine by the specified amount
        /// </summary>
        /// <param name="xMoveBy">Amount to move along the x-axis</param>
        /// <param name="yMoveBy">Amount to move along the y-axis</param>
        private void moveSecondSine(int xMoveBy, int yMoveBy)
        {
            bottomDepthPixels = bottomDepthPixels + yMoveBy;
            bottomAzimuth = bottomAzimuth + (int)((double)xMoveBy / ((double)sourceAzimuthResolution / (double)360));

            if (bottomAzimuth > 360)
                bottomAzimuth -= 360;
            else if (bottomAzimuth < 0)
                bottomAzimuth += 360;

            bottomSine.change(bottomDepthPixels, bottomAzimuth, bottomAmplitude);
        }

        # region Get methods

        /// <summary>
        /// Returns the y position of the first sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int GetTopYPoint(int xPoint)
        {
            return topSine.getY(xPoint);
        }

        /// <summary>
        /// Returns the y position of the second sine at the given x point
        /// </summary>
        /// <param name="xPoint"></param>
        /// <returns></returns>
        public override int getBottomYPoint(int xPoint)
        {
            return bottomSine.getY(xPoint);
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
            WriteLayerType();

            details = layerStartY + "," + layerEndY + "," + topDepthPixels + "," + topAzimuth + "," + topAmplitude + "," + bottomDepthPixels + "," + bottomAzimuth + "," + bottomAmplitude + "," + layerType + "," + description + "," + quality + "," + timeAdded + "," + timeLastModified + "," + Group;

            return details;
        }

        public override int GetNumOfEdges()
        {
            if (topAmplitude == bottomAmplitude && topAzimuth == bottomAzimuth && topDepthPixels == bottomDepthPixels)
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
            return topSine.getSinePoints();
        }

        /// <summary>
        /// Returns the points of the layers second edge
        /// </summary>
        /// <returns>A List of the points along the second edge</returns>
        public override List<Point> GetBottomEdgePoints()
        {
            return bottomSine.getSinePoints();
        }

        # endregion

        # region Set methods

        public override void SetTopEdgeDepth(int topDepthPixels)
        {
            this.topDepthPixels = topDepthPixels;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetBottomEdgeDepth(int bottomDepthPixels)
        {
            this.bottomDepthPixels = bottomDepthPixels;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetTopSineAmplitude(int topAmplitude)
        {
            this.topAmplitude = topAmplitude;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetBottomSineAmplitude(int bottomAmplitude)
        {
            this.bottomAmplitude = bottomAmplitude;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetTopSineAzimuth(int topAzimuth)
        {
            this.topAzimuth = topAzimuth;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetBottomSineAzimuth(int bottomAzimuth)
        {
            this.bottomAzimuth = bottomAzimuth;

            topSine.calculatePoints();
            bottomSine.calculatePoints();

            CalculateStartY();
            CalculateEndY();

            timeLastModified = DateTime.Now;
        }

        public override void SetTopEdgeSlope(double firstEdgeSlope)
        {
            //Not implemented in borehole
            throw new NotImplementedException();
        }

        public override void SetTopEdgeIntercept(int firstEdgeIntercept)
        {
            //Not implemented in borehole
            throw new NotImplementedException();
        }

        public override void SetBottomEdgeSlope(double secondEdgeSlope)
        {
            //Not implemented in borehole
            throw new NotImplementedException();
        }

        public override void SetBottomEdgeIntercept(int secondEdgeIntercept)
        {
            //Not implemented in borehole
            throw new NotImplementedException();
        }

        # endregion
    }
}
