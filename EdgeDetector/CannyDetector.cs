using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace EdgeDetector
{
    /// <summary>
    /// Performs a Canny edge detector on image data in the form of a byte array (three bytes make up 1 pixel( 
    /// or an int array 
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// </summary>
    public class CannyDetector
    {
        # region parameters

        private int kernelWidth;
        private float kernelSigma;
        private float lowThreshold, highThreshold;

        # endregion

        private int stackCounter;

        private static float MAGNITUDE_SCALE = 100000F;

        private int[] pixelData;
        private int imageWidth, imageHeight, imageSize;

        private float[] xConv, yConv;
        private float horizontalWeight = 1.0f;
        private float verticalWeight = 1.0f;

        private float[] xGradient, yGradient;
        private int[] magnitude;

        private int initX, maxX, initY, maxY;
        private float[] kernel, diffKernel;
        private int realKernelWidth;        //The radius of the kernel where values do not exceed the cutoff of 0.005

        private int[] thresholded;
        private bool[] edgeData;

        private bool wrapHorizontally = false;
        private bool wrapVertically = false;

        private int index, indexW, indexNW, indexSW, indexE, indexNE, indexSE, indexN, indexS;

        # region properties

        public float LowThreshold
        {
            get { return lowThreshold; }
            set { lowThreshold = value; }
        }

        public float HighThreshold
        {
            get { return highThreshold; }
            set { highThreshold = value; }
        }

        public int GaussianWidth
        {
            get
            { return kernelWidth; }
            set { kernelWidth = value; }
        }

        public float GaussianSigma
        {
            get { return kernelSigma; }
            set { kernelSigma = value; }
        }

        public float HorizontalWeight
        {
            get { return horizontalWeight; }
            set
            {
                if (value >= 0.0 && value < 1.0)
                    horizontalWeight = value;
            }
        }

        public float VerticalWeight
        {
            get
            {
                return verticalWeight;
            }
            set
            {
                if (value >= 0.0 && value < 1.0)
                    verticalWeight = value;
            }
        }
        public bool WrapHorizontally
        {
            get { return wrapHorizontally; }
            set { wrapHorizontally = value; }
        }

        public bool WrapVertically
        {
            get { return wrapVertically; }
            set { wrapVertically = value; }
        }


        # endregion

        # region Initialisation methods

        /// <summary>
        /// Constructor method which sets up the edge detector and initialises the values
        /// </summary>
        /// <param name="rawIntData">An integer array representing the brightness at each pixel</param>
        /// <param name="imageWidth">The width of the original image in pixels</param>
        /// <param name="imageHeight">The height of the original image in pixels</param>
        public CannyDetector(int[] rawIntData, int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            imageSize = imageWidth * imageHeight;

            SetDefaults();

            pixelData = new int[imageSize];
            //SetUpData();

            ConvertDataFromRGB(rawIntData);
        }

        public CannyDetector(Bitmap sourceImage)
        {
            this.imageWidth = sourceImage.Width;
            this.imageHeight = sourceImage.Height;

            imageSize = imageWidth * imageHeight;

            SetDefaults();

            pixelData = new int[imageSize];
            //SetUpData();

            int[] rawIntData = BitmapConverter.BitmapToRGBArray(sourceImage);

            ConvertDataFromRGB(rawIntData);
        }

        /// <summary>
        /// Constructor method which sets up the edge detector and initialises the values
        /// </summary>
        /// <param name="rawByteData">A byte array representing the r,g,b values of the original image</param>
        /// <param name="imageWidth">The width of the original image in pixels</param>
        /// <param name="imageHeight">The height of the original image in pixels</param>
        public CannyDetector(byte[] rawByteData, int imageWidth, int imageHeight)
        {
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            imageSize = imageWidth * imageHeight;

            SetDefaults();

            pixelData = new int[imageSize];
            //SetUpData();

            ConvertDataFromBytes(rawByteData);
        }

        /// <summary>
        /// Sets up the default parameter values
        /// </summary>
        private void SetDefaults()
        {
            //Default values
            kernelWidth = 3;
            kernelSigma = 1.0f;
            lowThreshold = 0.5f;
            highThreshold = 1.0f;
        }

        /// <summary>
        /// Sets initialises all data arrays
        /// </summary>
        private void SetUpData()
        {
            //Set up arrays            
            xConv = new float[imageSize];
            yConv = new float[imageSize];
            xGradient = new float[imageSize];
            yGradient = new float[imageSize];
            magnitude = new int[imageSize];
            thresholded = new int[imageSize];
            edgeData = new bool[imageSize];
        }

        /// <summary>
        /// Method which converts the raw byte r,g,b data to brightness values
        /// </summary>
        /// <param name="rawByteData">The byte data to convert</param>
        private void ConvertDataFromBytes(byte[] rawByteData)
        {
            int offset = 0;

            for (int i = 0; i < imageSize; i++)
            {
                int b = rawByteData[offset++] & 0xff;
                int g = rawByteData[offset++] & 0xff;
                int r = rawByteData[offset++] & 0xff;

                pixelData[i] = CalculateBrightness(r, g, b);
            }
        }

        /// <summary>
        /// Converts the int r,g,b data to brightness values
        /// </summary>
        /// <param name="rawIntData">The int data to convert</param>
        private void ConvertDataFromRGB(int[] rawIntData)
        {
            for (int i = 0; i < imageSize; i++)
            {
                int currentPixel = rawIntData[i];

                int r = (currentPixel & 0xff0000) >> 16;
                int g = (currentPixel & 0xff00) >> 8;
                int b = currentPixel & 0xff;

                pixelData[i] = CalculateBrightness(r, g, b);
            }
        }

        /// <summary>
        /// Returns the brightness value of a given RGB
        /// </summary>
        /// <param name="r">The red value of the pixel</param>
        /// <param name="g">The green value of the pixel</param>
        /// <param name="b">The blue value of the pixel</param>
        /// <returns>The brightness value associated with the given RGB values</returns>
        private int CalculateBrightness(float r, float g, float b)
        {

            int brightness = (int)Math.Floor(0.334f * r + 0.333f * g + 0.333f * b);

            return brightness;
        }

        # endregion

        /// <summary>
        /// Method which calls all the methods needed to perform the edge detection
        /// </summary>
        public void DetectEdges()
        {
            SetUpData();
            //ClearMemory();

            GaussianBlur();
            CalculateBorderValues();
            performConvolution();
            CalculateGradients();   //Calculates the gradient values in the image
            PerformSuppression();   //Perform non-maxima suppression

            int low = (int)Math.Floor(lowThreshold * MAGNITUDE_SCALE);
            int high = (int)Math.Floor(highThreshold * MAGNITUDE_SCALE);
            PerformHysteresis(low, high);
            BinarizeEdges();
        }

        /// <summary>
        /// Method which creates the gaussian blur
        /// </summary>
        private void GaussianBlur()
        {
            GaussianBlur gaussianBlur = new GaussianBlur(kernelWidth, kernelSigma);
            //Create 2 1D convolution masks
            kernel = gaussianBlur.GetGaussianKernel();
            diffKernel = gaussianBlur.GetGaussianDiffKernel();

            realKernelWidth = gaussianBlur.RealKernelWidth;
        }

        private void CalculateBorderValues()
        {
            if (wrapHorizontally)
            {
                initX = 0;
                maxX = imageWidth - 1;
            }
            else
            {
                initX = realKernelWidth;
                maxX = imageWidth - (realKernelWidth);
            }

            if (wrapVertically)
            {
                initY = 0;
                maxY = imageWidth * (imageHeight - 1);
            }
            else
            {
                initY = imageWidth * (realKernelWidth);
                maxY = imageWidth * (imageHeight - (realKernelWidth));
            }
        }

        /// <summary>
        /// Method which performs convolution on the data
        /// </summary>
        private void performConvolution()
        {
            for (int x = initX; x <= maxX; x++) // for (int x = initX; x < maxX; x++)
            {
                for (int y = initY; y <= maxY; y += imageWidth)
                {
                    CalculateConvolutionValue(x, y);
                }
            }
        }

        /// <summary>
        /// Convolves image with the gaussian blur
        /// </summary>
        /// <param name="x">The x position to convolve</param>
        /// <param name="y">The y position to convolve</param>
        private void CalculateConvolutionValue(int x, int y)
        {
            int index = x + y;
            float sumX = pixelData[index] * kernel[0];
            float sumY = sumX;

            int yOffset = imageWidth;

            for (int xOffset = 1; xOffset < realKernelWidth; xOffset++)
            {
                //Vertical wrapping
                if (y - yOffset < 0)
                    sumY += kernel[xOffset] * (pixelData[index - yOffset + maxY] + pixelData[index + yOffset]);
                else if (y + yOffset > maxY)
                    sumY += kernel[xOffset] * (pixelData[index - yOffset] + pixelData[index + yOffset - maxY]);
                else
                    sumY += kernel[xOffset] * (pixelData[index - yOffset] + pixelData[index + yOffset]);

                //Horizontal wrapping
                if (x - xOffset < 0)
                {
                    //if (wrapHorizontally)
                    sumX += kernel[xOffset] * (pixelData[index - xOffset + imageWidth] + pixelData[index + xOffset]);
                }
                else if (x + xOffset >= imageWidth)
                {
                    //if (wrapHorizontally)
                    sumX += kernel[xOffset] * (pixelData[index - xOffset] + pixelData[index + xOffset - imageWidth]);
                }
                else
                {
                    sumX += kernel[xOffset] * (pixelData[index - xOffset] + pixelData[index + xOffset]);
                }

                yOffset += imageWidth;
            }

            yConv[index] = sumY;
            xConv[index] = sumX;
        }

        # region Calculate Gradient

        /// <summary>
        /// Method which calculates the x and y gradients of all points in the image
        /// </summary>
        private void CalculateGradients()
        {
            for (int x = initX; x <= maxX; x++)
            {
                for (int y = initY; y <= maxY; y += imageWidth)
                {
                    if (verticalWeight > 0.0f)
                        CalculateYGradient(x, y);

                    if (horizontalWeight > 0.0f)
                        CalculateXGradient(x, y);
                }
            }
        }

        private void CalculateYGradient(int x, int y)
        {
            float sum = 0.0f;
            int index = x + y;
            int yOffset = imageWidth;

            for (int i = 1; i < realKernelWidth; i++)
            {
                if (y - yOffset < 0)
                    sum += diffKernel[i] * (xConv[index - yOffset + maxY] - xConv[index + yOffset]);
                else if (y + yOffset > maxY)
                    sum += diffKernel[i] * (xConv[index - yOffset] - xConv[index + yOffset - maxY]);
                else
                    sum += diffKernel[i] * (xConv[index - yOffset] - xConv[index + yOffset]);

                yOffset += imageWidth;
            }

            yGradient[index] = sum * verticalWeight;
        }

        private void CalculateXGradient(int x, int y)
        {
            float sum = 0f;
            int index = x + y;

            for (int i = 1; i < realKernelWidth; i++)
            {
                if (x - i < 0)
                {
                    //if (wrapHorizontally)
                    sum += diffKernel[i] * (yConv[index - i + imageWidth] - yConv[index + i]);
                }
                else if (x + i >= imageWidth)
                {
                    //if (wrapHorizontally)
                    sum += diffKernel[i] * (yConv[index - i] - yConv[index + i - imageWidth]);
                }
                else
                {
                    sum += diffKernel[i] * (yConv[index - i] - yConv[index + i]);
                }
            }

            xGradient[index] = sum * horizontalWeight;
        }

        # endregion

        /// <summary>
        /// Performs non-maximal suppression on the image data.  This
        /// gives us the magnitude of all points while thinning the lines
        /// </summary>
        private void PerformSuppression()
        {
#warning look into setting init and max x when edge wrapping is not set

            //initX = 0;
            //initY = imageWidth * realKernelWidth;
            //maxY = imageWidth * (imageHeight - realKernelWidth);

            for (int x = initX; x <= maxX; x++) // for (int x = initX; x < maxX; x++)
            {
                for (int y = initY; y <= maxY; y += imageWidth)
                {
                    CalculateMagnitude(x, y);
                }
            }
        }

        private void CalculateMagnitude(int x, int y)
        {
            CalculateIndexValues(x, y);

            float xGrad = xGradient[index];
            float yGrad = yGradient[index];

            float gradMag = Hypot(xGrad, yGrad);

            float nMag = Hypot(xGradient[indexN], yGradient[indexN]);
            float sMag = Hypot(xGradient[indexS], yGradient[indexS]);
            float wMag = Hypot(xGradient[indexW], yGradient[indexW]);
            float eMag = Hypot(xGradient[indexE], yGradient[indexE]);
            float neMag = Hypot(xGradient[indexNE], yGradient[indexNE]);
            float seMag = Hypot(xGradient[indexSE], yGradient[indexSE]);
            float swMag = Hypot(xGradient[indexSW], yGradient[indexSW]);
            float nwMag = Hypot(xGradient[indexNW], yGradient[indexNW]);
            float tmp;

            if (xGrad * yGrad <= (float)0)
            {
                if (Math.Abs(xGrad) >= Math.Abs(yGrad))
                {
                    if ((tmp = Math.Abs(xGrad * gradMag)) >= Math.Abs(yGrad * neMag - (xGrad + yGrad) * eMag) && tmp > Math.Abs(yGrad * swMag - (xGrad + yGrad) * wMag))
                        magnitude[index] = (int)(MAGNITUDE_SCALE * gradMag);
                    else
                        magnitude[index] = 0;
                }
                else
                {
                    if ((tmp = Math.Abs(yGrad * gradMag)) >= Math.Abs(xGrad * neMag - (yGrad + xGrad) * nMag) && tmp > Math.Abs(xGrad * swMag - (yGrad + xGrad) * sMag))
                        magnitude[index] = (int)(MAGNITUDE_SCALE * gradMag);
                    else
                        magnitude[index] = 0;
                }
            }
            else
            {
                if (Math.Abs(xGrad) >= Math.Abs(yGrad))
                {
                    if ((tmp = Math.Abs(xGrad * gradMag)) >= Math.Abs(yGrad * seMag + (xGrad - yGrad) * eMag) && tmp > Math.Abs(yGrad * nwMag + (xGrad - yGrad) * wMag))
                        magnitude[index] = (int)(MAGNITUDE_SCALE * gradMag);
                    else
                        magnitude[index] = 0;
                }
                else
                {
                    if ((tmp = Math.Abs(yGrad * gradMag)) >= Math.Abs(xGrad * seMag + (yGrad - xGrad) * sMag) && tmp > Math.Abs(xGrad * nwMag + (yGrad - xGrad) * nMag))
                        magnitude[index] = (int)(MAGNITUDE_SCALE * gradMag);
                    else
                        magnitude[index] = 0;
                }
            }
        }

        private void CalculateIndexValues(int x, int y)
        {
            index = x + y;

            int xWest, xEast;
            int yNorth, ySouth;

            if (x - 1 < 0)
            {
                xWest = imageWidth - 1;
                xEast = x + 1;
            }
            else if (x + 1 >= imageWidth)
            {
                xWest = x - 1;
                xEast = 0;
            }
            else
            {
                xWest = x - 1;
                xEast = x + 1;
            }


            if (y - imageWidth < 0)
            {
                yNorth = maxY;
                ySouth = y + imageWidth;
            }
            else if (y + imageWidth > maxY)
            {
                yNorth = y - imageWidth;
                ySouth = initY;
            }
            else
            {
                yNorth = y - imageWidth;
                ySouth = y + imageWidth;
            }

            indexN = x + yNorth;
            indexNE = yNorth + xEast;
            indexE = y + xEast;
            indexSE = ySouth + xEast;
            indexS = x + ySouth;
            indexSW = ySouth + xWest;
            indexW = xWest + y;
            indexNW = yNorth + xWest;
        }

        /// <summary>
        /// Method which returns the hypotenuse of two given parameters
        /// i.e. square root of (x squared + y squared)
        /// </summary>
        /// <param name="x">The first value to calculate</param>
        /// <param name="y">The second value to calculate</param>
        /// <returns>hypotenuse of x and y</returns>
        private float Hypot(float x, float y)
        {
            float hypotenuse = (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

            return hypotenuse;
        }

        /// <summary>
        /// Method which performs hysteresis on the pixelData
        /// </summary>
        private void PerformHysteresis(int low, int high)
        {
            //Array.Clear(pixelData, 0, pixelData.Length);

            int offset = 0;

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    //If the magnitude is more than or equal to the high threshold follow the edge
                    if (thresholded[offset] == 0 && magnitude[offset] >= high)
                    {
                        stackCounter = 0;

                        Follow(x, y, offset, low);
                    }

                    offset++;
                }
            }
        }

        /// <summary>
        /// Method which connects points from a high threshold point until there are
        ///no connecting point above low threshold
        ///
        /// </summary>
        /// <param name="xCurrent">The x position of the beginning edge point</param>
        /// <param name="yCurrent">The y position of the beginning edge point</param>
        /// <param name="indexValue">The index value of the point</param>
        /// <param name="threshold">The low threshold value</param>
        private void Follow(int xCurrent, int yCurrent, int indexValue, int threshold)
        {
            stackCounter++;

            if (stackCounter > imageWidth)
                return;

            int xPrevious, xNext;
            int yPrevious, yNext;

            if (xCurrent == 0)
                xPrevious = xCurrent;
            else
                xPrevious = xCurrent - 1;

            if (xCurrent == imageWidth - 1)
                xNext = xCurrent;
            else
                xNext = xCurrent + 1;

            if (yCurrent == 0)
                yPrevious = yCurrent;
            else
                yPrevious = yCurrent - 1;

            if (yCurrent == imageHeight - 1)
                yNext = yCurrent;
            else
                yNext = yCurrent + 1;

            thresholded[indexValue] = magnitude[indexValue];

            for (int x = xPrevious; x <= xNext; x++)
            {
                for (int y = yPrevious; y <= yNext; y++)
                {
                    int i2 = x + y * imageWidth;

                    if ((y != yCurrent || x != xCurrent) && thresholded[i2] == 0 && magnitude[i2] >= threshold)
                    {
                        Follow(x, y, i2, threshold);

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Thresholds the edge points so that they appear as either
        /// black or white
        /// </summary>
        private void BinarizeEdges()
        {
            for (int i = imageWidth; i < imageSize - imageWidth; i++)
            {
                if (thresholded[i] > 0)
                    edgeData[i] = true;
                else
                    edgeData[i] = false;
            }
        }

        private void ClearMemory()
        {
            xConv = null;
            yConv = null;
            xGradient = null;
            yGradient = null;
            magnitude = null;
            thresholded = null;

            GC.Collect();
        }

        # region Get methods

        /// <summary>
        /// Returns an array of floats giving the images convolution in the x direction
        /// </summary>
        /// <returns></returns>
        public float[] GetXConvolutionData()
        {
            return xConv;
        }

        public Bitmap GetXConvolutionBitmap()
        {
            Bitmap convolutionImage = ScaleDataToImage.ScaleToDataRange(xConv, imageWidth, imageHeight);

            return convolutionImage;
        }

        /// <summary>
        /// Returns an array of floats giving the images convolution in the y direction
        /// </summary>
        /// <returns></returns>
        public float[] GetYConvolutionData()
        {
            return yConv;
        }

        public Bitmap GetYConvolutionBitmap()
        {
            Bitmap convolutionImage = ScaleDataToImage.ScaleToDataRange(yConv, imageWidth, imageHeight);

            return convolutionImage;
        }

        /// <summary>
        /// Returns an array of floats giving the images gradients in the x direction
        /// </summary>
        /// <returns></returns>
        public float[] GetXGradientData()
        {
            return xGradient;
        }

        public Bitmap GetXGradientBitmap()
        {
            Bitmap gradientImage = ScaleDataToImage.ScaleToDataRange(xGradient, imageWidth, imageHeight);

            return gradientImage;
        }

        /// <summary>
        /// Returns an array of floats giving the images gradients in the y direction
        /// </summary>
        /// <returns></returns>
        public float[] GetYGradientData()
        {
            return yGradient;
        }

        public Bitmap GetYGradientBitmap()
        {
            Bitmap gradientImage = ScaleDataToImage.ScaleToDataRange(yGradient, imageWidth, imageHeight);

            return gradientImage;
        }

        /// <summary>
        /// Returns an int array of the images magnitude after non maximal suppression
        /// </summary>
        /// <returns></returns>
        public int[] GetMagnitudeData()
        {
            return magnitude;
        }

        public Bitmap GetMagnitudeBitmap()
        {
            Bitmap gradientImage = ScaleDataToImage.ScaleToDataRange(magnitude, imageWidth, imageHeight);

            return gradientImage;
        }

        /// <summary>
        /// Returns an int array of the images magnitude values after hysteresis
        /// </summary>
        /// <returns></returns>
        public int[] GetThresholdedData()
        {
            return thresholded;
        }

        public Bitmap GetThresholdedBitmap()
        {
            Bitmap thresholdedImage = ScaleDataToImage.ScaleToDataRange(thresholded, imageWidth, imageHeight);

            return thresholdedImage;
        }

        /// <summary>
        /// Method which returns the edge data
        ///
        /// </summary>
        /// <returns>A one-dimensional array where eack pixel is representing by a bool value.
        /// The found edges are true and non found edges are black</returns>
        public bool[] GetEdgeData()
        {
            return edgeData;
        }

        /// <summary>
        /// Returns a Bitmap with the edges appearing white and the background black
        /// </summary>
        /// <returns>Bitmap image with the found edges drawn</returns>
        public Bitmap GetEdgesBitmap()
        {
            Bitmap edgesImage = new Bitmap(imageWidth, imageHeight);

            int offset = 0;

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {

                    //int currentPos = (y * imageWidth) + x;

                    if (edgeData[offset])
                    {
                        edgesImage.SetPixel(x, y, Color.White);
                    }
                    else
                        edgesImage.SetPixel(x, y, Color.Black);

                    offset++;
                }
            }

            return edgesImage;
        }

        # endregion

    }
}

