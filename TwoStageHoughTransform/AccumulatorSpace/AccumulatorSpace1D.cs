using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoStageHoughTransform.AccumulatorSpace
{
    /// <summary>
    /// Class which implements a 1 dimensional accumulator space
    /// </summary>
    class AccumulatorSpace1D
    {
        private int size;
        private double[] space;

        private int sizeToCheckEachWay = 2;

        #region Properties

        public int Size
        {
            get
            {
                return size;
            }
        }

        public int NeighboursToCheck
        {
            get
            {
                return sizeToCheckEachWay;
            }
            set
            {
                sizeToCheckEachWay = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="size">The size of the accumulator space</param>
        public AccumulatorSpace1D(int size)
        {
            this.size = size;

            space = new double[size];

            Array.Clear(space, 0, size);
        }

        #endregion

        /// <summary>
        /// Increments the specified position in the accumulator space by 1
        /// </summary>
        /// <param name="positionToIncrement">The position in the accumulator space to increment</param>
        public void Increment(int positionToIncrement)
        {
            space[positionToIncrement]++;
        }

        public void IncrementBy(int positionToIncrement, double amountToIncrement)
        {
            space[positionToIncrement] = space[positionToIncrement] + amountToIncrement;
        }

        /// <summary>
        /// Decrement the specified position in the accumulator space by 1
        /// </summary>
        /// <param name="positionToIncrement">The position in the accumulator space to increment</param>
        public void Decrement(int positionToDecrement)
        {
            if (space[positionToDecrement] >= 0)
                space[positionToDecrement]--;
        }

        public void DecrementBy(int positionToDecrement, double amountToDecrement)
        {
            space[positionToDecrement] = space[positionToDecrement] - amountToDecrement;

            if (space[positionToDecrement] < 0)
                space[positionToDecrement] = 0;
        }

        public int GetTopPeak(int peakThreshold)
        {
            double highest = 0;
            int highestPosition = 0;

            //Find highest point
            for (int i = 0; i < space.Length; i++)
            {
                double total = 0;

                total += space[i];

                for (int check = 1; check <= sizeToCheckEachWay; check++)
                {
                    //Check before
                    if (i - check >= 0)
                        total += space[i - check];

                    //Check after
                    if (i + check < space.Length)
                        total += space[i + check];
                }

                if (total > highest)
                {
                    highestPosition = i;

                    highest = total;
                }
            }

            if (highest >= peakThreshold)
            {
                return highestPosition;
            }
            else
                return space.Length;    //Returns not possible value

        }

        # region Get Methods

        /// <summary>
        /// Returns the position and value of the top accumulated values
        /// </summary>
        /// <param name="numberToReturn">The number of values to return</param>
        /// <returns>The most accumulated as a two dimensional array, giving both its position and value</returns>
        public int[,] GetMostAccumulated(int numberToReturn)
        {
            int[,] mostAccumulated = new int[2, numberToReturn];

            int[] tempSpace = (int[])space.Clone();

            int highest = 0;
            int highestPosition = 0;

            for (int i = 0; i < numberToReturn; i++)
            {
                highest = 0;

                for (int j = 0; j < tempSpace.Length; j++)
                {
                    if (tempSpace[j] > highest)
                    {

                        highest = tempSpace[j];
                        highestPosition = j;
                    }
                }

                mostAccumulated[0, i] = highestPosition;
                mostAccumulated[1, i] = highest;

                //Remove the peak before highest value
                int nextposition = highestPosition - 1;
                //int previousValue = tempSpace[highestPosition];

                while (nextposition >= 0 && tempSpace[nextposition] > 0)
                {
                    tempSpace[nextposition] = 0;
                    nextposition--;
                }

                //Remove the peak before highest value
                nextposition = highestPosition + 1;

                while (nextposition < size && tempSpace[nextposition] > 0)
                {
                    tempSpace[nextposition] = 0;
                    nextposition++;
                }

                tempSpace[highestPosition] = 0;
            }

            return mostAccumulated;
        }

        /**
         * Method which returns the value at the specified position in the the
         * accumulator space
         *
         * @param position The position in the accumulator space of the value to return
         * @return space[position] The value at the specified position
         */
        public double getValue(int position)
        {
            return space[position];
        }

        /**
         * Method which returns the entire accumulator space
         *
         * @return space The entire accumulator space
         */
        public double[] GetSpace()
        {
            return space;
        }

        # endregion
    }
}
