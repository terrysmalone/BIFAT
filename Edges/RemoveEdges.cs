using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Edges
{
    /// <summary>
    /// A class which removes any edges which are shorter than a given value from an
    /// ArrayList of Edges
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// version 1.1
    /// </summary>
    public class RemoveEdges
    {
        private int minimumLength;
        private List<Edge> edgesBefore;
        private List<Edge> edgesAfter = new List<Edge>();

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="edges">The List of edges</param>
        /// <param name="minimumLength">The threshold which edges smaller than are removed</param>
        public RemoveEdges(List<Edge> edges, int minimumLength)
        {
            this.edgesBefore = edges;
            this.minimumLength = minimumLength;
        }

        /// <summary>
        /// Removes the shorter edges from the ArrayList of Edges
        /// </summary>
        public void removeEdges()
        {
            //edgesAfter.RemoveAll(delegate(Edge currentEdge) { return currentEdge.EdgeLength < minimumLength; });
            for (int edge = 0; edge < edgesBefore.Count; edge++)
            {
                if (edgesBefore[edge].EdgeLength > minimumLength)
                    edgesAfter.Add(edgesBefore[edge]);
            }
        }

        # region set methods

        /// <summary>
        /// Sets the minimum length (All edges smaller than this will be removed
        /// </summary>
        /// <param name="minimumLength">The minimum length to be considered an edge</param>
        public void setMinimumLength(int minimumLength)
        {
            this.minimumLength = minimumLength;
        }

        # endregion

        # region get methods

        /// <summary>
        /// Returns the minimum length of which to consider an edge
        /// </summary>
        /// <returns>The minimum length</returns>
        public int getMinimumLength()
        {
            return minimumLength;
        }

        /// <summary>
        /// Returns the number of edges
        /// </summary>
        /// <returns>The number of edges</returns>
        public List<Edge> getEdges()
        {
            return edgesAfter;
        }

        # endregion
    }

}
