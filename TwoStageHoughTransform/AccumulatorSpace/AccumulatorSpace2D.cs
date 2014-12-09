using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwoStageHoughTransform.AccumulatorSpace
{
    /// <summary>
    /// Class which implements a 2 dimensional accumulator space
    /// </summary>
    public class AccumulatorSpace2D
    {
        private int dimension1, dimension2;       //Size of the hough space
        private int[,] space;

        private int dim1Max, dim2Max;

        # region Properties

        /// <summary>
        /// The size of the accumulator spaces first parameter
        /// </summary>
        public int Dimension1
        {
            get
            {
                return dimension1;
            }
        }

        /// <summary>
        /// The size of the accumulator spaces second parameter
        /// </summary>
        public int Dimension2
        {
            get
            {
                return dimension2;
            }
        }

        public int Dimension1Max
        {
            get
            {
                return dim1Max;
            }
        }

        public int Dimension2Max
        {
            get
            {
                return dim2Max;
            }
        }

        #endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="size">The size of the accumulator space</param>
        public AccumulatorSpace2D(int dimension1, int dimension2)
        {
            this.dimension1 = dimension1;
            this.dimension2 = dimension2;

            space = new int[dimension1, dimension2];

            Array.Clear(space, 0, dimension1 * dimension2);
        }

        /// <summary>
        /// Increments the specified position in the accumulator space by 1
        /// </summary>
        /// <param name="positionToIncrement">The position in the accumulator space to increment</param>
        public void Increment(int position1ToIncrement, int position2ToIncrement)
        {
            space[position1ToIncrement, position2ToIncrement]++;
        }

        public void IncrementBy(int position1ToIncrement, int position2ToIncrement, int amountToIncrement)
        {
            space[position1ToIncrement, position2ToIncrement] = space[position1ToIncrement, position2ToIncrement] + amountToIncrement;
        }

        /// <summary>
        /// Decrement the specified position in the accumulator space by 1
        /// </summary>
        /// <param name="positionToIncrement">The position in the accumulator space to increment</param>
        public void Decrement(int position1ToDecrement, int position2ToDecrement)
        {
            space[position1ToDecrement, position2ToDecrement]--;
        }

        public void DecrementBy(int position1ToDecrement, int position2ToIncrement, int amountToDecrement)
        {
            space[position1ToDecrement, position2ToIncrement] = space[position1ToDecrement, position2ToIncrement] + amountToDecrement;
        }

        public void CalculateMax()
        {
            int max = 0;

            for (int i = 0; i < dimension1; i++)
            {
                for (int j = 0; j < dimension2; j++)
                {
                    int currentCell = space[i, j];

                    if (currentCell > max)
                    {
                        max = currentCell;

                        dim1Max = i;
                        dim2Max = j;
                    }
                }
            }
        }

        # region Get Methods

        /**
         * Method which returns the value at the specified position in the the
         * accumulator space
         *
         * @param position The position in the accumulator space of the value to return
         * @return space[position] The value at the specified position
         */
        public int GetValue(int position1, int position2)
        {
            return space[position1, position2];
        }

        /**
         * Method which returns the entire accumulator space
         *
         * @return space The entire accumulator space
         */
        public int[,] GetSpace()
        {
            return space;
        }

        # endregion
    }
}
