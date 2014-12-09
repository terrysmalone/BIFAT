using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdgeDetector
{
    /// <summary>
    /// Calculates Gaussian kernels 
    /// </summary>
    internal class GaussianBlur
    {
       private int realKernelWidth;
        private int kernelWidth;
        private float kernelSigma;

        private float[] kernel, diffKernel;

        #region Properties

        /// <summary>
        /// Rounded kernel width
        /// </summary>
        public int RealKernelWidth
        {
            get
            {
                return realKernelWidth;
            }
        }

        #endregion properties

        #region Constructor

        public GaussianBlur(int kernelWidth, float kernelSigma)
        {
            this.kernelWidth = kernelWidth;
            this.kernelSigma = kernelSigma;

            kernel = new float[kernelWidth];
            diffKernel = new float[kernelWidth];

            for (realKernelWidth = 0; realKernelWidth < kernelWidth; realKernelWidth++)
            {
                float g1 = Gaussian(realKernelWidth, kernelSigma);

                if (g1 <= 0.005 && realKernelWidth >= 2) break;

                float g2 = Gaussian(realKernelWidth - 0.5f, kernelSigma);
                float g3 = Gaussian(realKernelWidth + 0.5f, kernelSigma);

                kernel[realKernelWidth] = (g1 + g2 + g3) / 3f / (2f * (float)Math.PI * kernelSigma * kernelSigma);
                diffKernel[realKernelWidth] = g3 - g2;
            }
        }

        #endregion constructor

        /// <summary>
        /// Works out the value at a given point in the gaussian kernel
        /// </summary>
        /// <param name="position">The position in the kernel</param>
        /// <param name="sigma">The sigma value of the kernel</param>
        /// <returns>The value at the given position</returns>
        private float Gaussian(float position, float sigma)
        {
            float gaussianValue = (float)Math.Exp(-(position * position) / (2f * sigma * sigma));

            return gaussianValue;
        }

        #region Get mothods

        public float[] GetGaussianKernel()
        {
            return kernel;
        }

        public float[] GetGaussianDiffKernel()
        {
            return diffKernel;
        }

        #endregion Get methods
    }
}