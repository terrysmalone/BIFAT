using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edges;
using System.Drawing;

namespace EdgeFitting
{
    /// <summary>
    /// A class which checks if any sines/edgeLines overlap with edges (other than the one
    /// they were created from) and if so combines edges and redraws the sines/edgeLines
    /// 
    /// Author Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1
    /// </summary>
    public class Overlap
    {
        private List<Sine> sineWaves;
        private List<Edge> sineEdges;

        private int imageWidth, imageHeight, maxAmplitude;

        private List<Edge> edgesToAdd = new List<Edge>();

        private List<Edge> newEdges = new List<Edge>();
        private List<Sine> newSines = new List<Sine>();

        private List<int> deleteList = new List<int>();

        private int overlapTolerance;      //Defines how many pixels edges are allowed to overlap before they are joined

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="sineWaves">The sinewaves to check for overlap</param>
        /// <param name="sineEdges">The edge sets to check for overlap</param>
        /// <param name="imageWidth">The image width</param>
        /// <param name="imageHeight">The image height</param>
        /// <param name="maxAmplitude">The max amplitude of sines</param>
        public Overlap(List<Sine> sineWaves, List<Edge> sineEdges, int imageWidth, int imageHeight, int maxAmplitude)
        {
            this.sineWaves = sineWaves;
            this.sineEdges = sineEdges;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.maxAmplitude = maxAmplitude;

            overlapTolerance = 10;
        }

        /// <summary>
        /// Checks if any sines and edges overlap
        /// </summary>
        public void checkForOverlap()
        {
            for (int i = 0; i < sineEdges.Count; i++)
            {
                if (!deleteList.Contains(i))
                {
                    for (int j = 0; j < sineWaves.Count; j++)
                    {
                        checkIfSineAndEdgeOverlap(i, j);
                    }
                }
            }

            createNewLists();
        }

        /// <summary>
        /// Checks if given edge and sine overlap
        /// </summary>
        /// <param name="edge">The edge to check</param>
        /// <param name="sine">The sine to check</param>
        private void checkIfSineAndEdgeOverlap(int edge, int sine)
        {
            if (edge != sine && !deleteList.Contains(sine))
            {
                if (overlap(sineEdges[edge], sineWaves[sine]))
                {
                    checkIfEdgesOverLap(edge, sine);
                }
            }
        }

        /// <summary>
        /// Checks if the given sine and edge overlap.  
        /// </summary>
        /// <param name="edge">The edge to compare</param>
        /// <param name="wave">The sine wave to compare</param>
        /// <returns>True if edges over lap, false if not</returns>
        private bool overlap(Edge edge, Sine wave)
        {
            List<Point> edgePoints = edge.Points;
            List<Point> wavePoints = wave.Points;

            for (int i = 0; i < edgePoints.Count; i++)
            {
                if (wavePoints.Contains(edgePoints[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if two edges overlap
        /// </summary>
        /// <param name="edge1">The first edge</param>
        /// <param name="edge2">The second edge</param>
        private void checkIfEdgesOverLap(int edge1, int edge2)
        {
            if (!edgesShareSpace(sineEdges[edge1], sineEdges[edge2]))
            {
                deleteList.Add(edge1);
                deleteList.Add(edge2);

                Edge newEdge = combineEdges(sineEdges[edge1], sineEdges[edge2]);
                edgesToAdd.Add(newEdge);
            }
        }

        /// <summary>
        /// Checks if edge pixels share the same vertical space
        /// </summary>
        /// <param name="edge1">The first edge</param>
        /// <param name="edge2">The second edge</param>
        /// <returns>True if edges share same space, false if not</returns>
        private bool edgesShareSpace(Edge edge1, Edge edge2)
        {
            bool shareSpace = false;

            edge1.CalculateEndPoints();
            edge2.CalculateEndPoints();

            if (edge2.HighestXPoint >= edge1.HighestXPoint)
            {
                if (edge2.LowestXPoint + overlapTolerance < edge1.HighestXPoint)
                    shareSpace = true;
            }
            else
            {
                if (edge2.HighestXPoint - overlapTolerance > edge1.LowestXPoint)
                    shareSpace = true;
            }

            return shareSpace;
        }

        /// <summary>
        /// Combines two edges
        /// </summary>
        /// <param name="firstEdge"></param>
        /// <param name="secondEdge"></param>
        /// <returns></returns>
        private Edge combineEdges(Edge firstEdge, Edge secondEdge)
        {
            Edge newEdge = new Edge(imageWidth);

            newEdge.AddEdge(firstEdge);
            newEdge.AddEdge(secondEdge);

            return newEdge;
        }

        /// <summary>
        /// Creates new sine and Edge lists based on what has already been checked
        /// </summary>
        private void createNewLists()
        {
            removeIfOnDeleteList();

            addNew();
        }

        /// <summary>
        /// Removes sines and edges if they are on delete list
        /// </summary>
        private void removeIfOnDeleteList()
        {
            for (int i = 0; i < sineEdges.Count; i++)
            {
                removeIfOnDeleteList(i);
            }
        }

        private void removeIfOnDeleteList(int edgeToCheck)
        {
            if (!deleteList.Contains(edgeToCheck))
            {
                newEdges.Add(sineEdges[edgeToCheck]);
                newSines.Add(sineWaves[edgeToCheck]);
            }
        }

        private void addNew()
        {
            Sine newSine;
            BestFitSine newFit;

            for (int i = 0; i < edgesToAdd.Count; i++)
            {
                newEdges.Add(edgesToAdd[i]);

                newFit = new BestFitSine(edgesToAdd[i], imageWidth, imageHeight);
                newFit.setMaxAmplitude(maxAmplitude);
                newFit.FindBestFit();
                newSine = newFit.GetSine();
                newSines.Add(newSine);
            }
        }

        public void setOverlapTolerance(int overlapTolerance)
        {
            this.overlapTolerance = overlapTolerance;
        }

        # region get methods

        public List<Sine> getSines()
        {
            return newSines;
        }

        public List<Edge> getEdges()
        {
            return newEdges;
        }

        public int getOverlapTolerance()
        {
            return overlapTolerance;
        }

        public int getImageWidth()
        {
            return imageWidth;
        }

        public int getImageHeight()
        {
            return imageHeight;
        }

        # endregion
    }

}
