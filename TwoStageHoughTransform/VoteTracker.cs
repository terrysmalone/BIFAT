using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoStageHoughTransform
{
    class VoteTracker
    {
        private double[,] edgeVotes;     //Tracks the number of votes each edgeset has contributed to each depth (used for removing votes later)

        private int accumulatorSpaceSize;

        public VoteTracker(int numOfEdges, int accumulatorSpaceSize)
        {
            this.accumulatorSpaceSize = accumulatorSpaceSize;

            edgeVotes = new double[numOfEdges, accumulatorSpaceSize];
        }

        public void AddVotes(int edgeAddedBy, int depthToAddTo, double amountToAdd)
        {
            edgeVotes[edgeAddedBy, depthToAddTo] += amountToAdd;
        }

        # region Get methods

        public double[] GetVotesFromEdge(int edgeToCheck)
        {
            double[] voteCounts = new double[accumulatorSpaceSize];

            int edgePos = edgeToCheck;

            for (int i = 0; i < accumulatorSpaceSize; i++)
            {
                voteCounts[i] = edgeVotes[edgeToCheck, i];
            }

            return voteCounts;
        }

        public double GetVotes(int edgeToCheck, int depthToCheck)
        {
            double voteCount = 0.0;

            voteCount = edgeVotes[edgeToCheck, depthToCheck];

            return voteCount;
        }

        public double[,] GetAllVotes()
        {
            return edgeVotes;
        }

        #endregion
    }
}
