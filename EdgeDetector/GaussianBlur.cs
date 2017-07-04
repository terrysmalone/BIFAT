using System;

namespace EdgeDetector
{
    /// <summary>
    /// Calculates Gaussian kernels 
    /// </summary>
    internal class GaussianBlur
    {
        private readonly float[] m_Kernel;
        private readonly float[] m_DiffKernel;
        
        /// <summary>
        /// Rounded kernel width
        /// </summary>
        public int RealKernelWidth { get; }
        
        #region Constructor

        public GaussianBlur(int kernelWidth, float kernelSigma)
        {
            m_Kernel = new float[kernelWidth];
            m_DiffKernel = new float[kernelWidth];

            for (RealKernelWidth = 0; RealKernelWidth < kernelWidth; RealKernelWidth++)
            {
                var g1 = Gaussian(RealKernelWidth, kernelSigma);

                if (g1 <= 0.005 && RealKernelWidth >= 2) break;

                var g2 = Gaussian(RealKernelWidth - 0.5f, kernelSigma);
                var g3 = Gaussian(RealKernelWidth + 0.5f, kernelSigma);

                m_Kernel[RealKernelWidth] = (g1 + g2 + g3) 
                                            / 3f 
                                            / (2f * (float)Math.PI * kernelSigma * kernelSigma);

                m_DiffKernel[RealKernelWidth] = g3 - g2;
            }
        }

        #endregion constructor

        /// <summary>
        /// Works out the value at a given point in the gaussian kernel
        /// </summary>
        /// <param name="position">The position in the kernel</param>
        /// <param name="sigma">The sigma value of the kernel</param>
        /// <returns>The value at the given position</returns>
        private static float Gaussian(float position, float sigma)
        {
            var gaussianValue = (float)Math.Exp(-(position * position) / (2f * sigma * sigma));

            return gaussianValue;
        }

        #region Get mothods

        public float[] GetGaussianKernel()
        {
            return m_Kernel;
        }

        public float[] GetGaussianDiffKernel()
        {
            return m_DiffKernel;
        }

        #endregion Get methods
    }
}