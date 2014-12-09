using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edges;

namespace EdgeFitting
{
    /// <summary>
    /// a class which fits Sines or Lines to a group of given edge sets
    /// 
    /// Author Terry malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class EdgeFit
    {
        private string imageType = "Borehole";

        private List<Edge> edges = new List<Edge>();

        private List<Sine> sines = new List<Sine>();
        private List<EdgeLine> edgeLines = new List<EdgeLine>();
        
        private int sourceImageWidth, sourceImageHeight;
        private int maxAmplitude = 30;

        # region Properties

        public int MaxAmplitude
        {
            get
            {
                return maxAmplitude;
            }
            set
            {
                maxAmplitude = value;
            }
        }

        /// <summary>
        /// Returns the EdgeLines that have been fit
        /// </summary>
        public List<EdgeLine> FitLines
        {
            get
            {
                return edgeLines;
            }
        }

        /// <summary>
        /// Returns the sinusoids that have been fit
        /// </summary>
        public List<Sine> FitSines
        {
            get
            {
                return sines;
            }
        }

        public string ImageType
        {
            get
            {
                return imageType;
            }
            set
            {
                imageType = value;
            }
        }

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="edges">The edges to fit sinusoids to</param>
        /// <param name="sourceImageWidth">The width of the image the edges are from</param>
        /// <param name="sourceImageHeight">The height of the image the edges are from</param>
        public EdgeFit(List<Edge> edges, int sourceImageWidth, int sourceImageHeight)
        {
            this.edges = edges;
            this.sourceImageWidth = sourceImageWidth;
            this.sourceImageHeight = sourceImageHeight;
        }

        /// <summary>
        /// Fits sines or EdgeLines to the edges
        /// </summary>
        public void FitEdges()
        {
            for (int i = 0; i < edges.Count; i++)
            {
                if (imageType == "Borehole")
                    FitSineToEdge(edges[i]);
                else
                    FitEdgeLineToEdge(edges[i]);
            }

            if (imageType == "Borehole")
                joinOverlaps();
        }

        /// <summary>
        /// Fits a sinusoid to the given edge
        /// </summary>
        /// <param name="edge">The edge to fit a sinusoid to</param>
        private void FitSineToEdge(Edge edge)
        {
            BestFitSine bestFitSine;

            bestFitSine = new BestFitSine(edge, sourceImageWidth, sourceImageHeight);
            bestFitSine.setMaxAmplitude(maxAmplitude);
            bestFitSine.FindBestFit();

            sines.Add(bestFitSine.GetSine());
        }

        private void FitEdgeLineToEdge(Edge edge)
        {
            BestFitEdgeLine bestFitEdgeLine = new BestFitEdgeLine(edge, sourceImageWidth, sourceImageHeight);
            bestFitEdgeLine.FindBestFit();

            edgeLines.Add(bestFitEdgeLine.GetEdgeLine());
        }

        /// <summary>
        /// Checks if any sines or edges overlap and if so removes them
        /// </summary>
        private void joinOverlaps()
        {
            Overlap overlap = new Overlap(sines, edges, sourceImageWidth, sourceImageHeight, maxAmplitude);
            overlap.checkForOverlap();
            sines.Clear();
            sines = overlap.getSines();
        }        
    }
}
