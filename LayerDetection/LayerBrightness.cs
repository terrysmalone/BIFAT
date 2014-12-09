using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EdgeFitting;

namespace LayerDetection
{
    /// <summary>
    /// Class which calculates the average brightness between two sinusoids
    /// in an image
    /// </summary>
    class LayerBrightness
    {
        private Graphics graphics;
        private Bitmap originalImage;
        private int imageWidth, imageHeight;

        private int inLayerBrightness, surroundingBrightness, averageImageBrightness;
        private int lastCount;

        # region Constructors

        /// <summary>
        /// Constuctor method
        /// </summary>
        /// <param name="originalImage">The original image which is to be checked</param>
        public LayerBrightness(Bitmap originalImage)
        {
            this.originalImage = originalImage;
            imageWidth = originalImage.Width;
            imageHeight = originalImage.Height;

            graphics = Graphics.FromImage(originalImage);
        }

        # endregion

        /// <summary>
        /// Method which calculates the average brightness of the entire image (0 = black 255 = white)
        /// </summary>
        private void CalculateImageBrightness()
        {
            int totalImageBrightness = 0;
            Color pixelColor;

            int widthSpacing = 5;
            int heightSpacing = 5;

            for (int x = 0; x < imageWidth; x += widthSpacing)
            {
                for (int y = 0; y < imageHeight; y += heightSpacing)
                {
                    pixelColor = originalImage.GetPixel(x, y);

                    int b = pixelColor.B & 0xff;
                    int g = pixelColor.G & 0xff;
                    int r = pixelColor.R & 0xff;

                    totalImageBrightness += CalculateBrightness(r, g, b);
                }
            }

            double widthResolution = imageWidth / (double)widthSpacing;
            double heightResolution = imageHeight / (double)heightSpacing;

            averageImageBrightness = (int)Math.Floor((float)totalImageBrightness / ((float)widthResolution * (float)heightResolution));
        }

        /**
         * Method which returns the brightness value of a given RGB
         * 
         * @param r The red value of the pixel
         * @param g The green value of the pixel
         * @param b The blue value of the pixel
         * 
         * @return The brightness value associated with the given RGB values
         */
        private int CalculateBrightness(float r, float g, float b)
        {
            int pixelBrightness = (int)Math.Floor(0.334f * r + 0.333f * g + 0.333f * b);

            return pixelBrightness;
        }

        /// <summary>
        /// Calculates and returns the average brighness between two Sines
        /// </summary>
        /// <param name="sine1">The first sine</param>
        /// <param name="sine2">The second sine</param>
        /// <returns></returns>
        public int GetBrightness(Sine sine1, Sine sine2)
        {
            inLayerBrightness = 0;
            
            List<Point> sine1Points = new List<Point>();
            List<Point> sine2Points = new List<Point>();

            sine1Points = sine1.Points;
            sine2Points = sine2.Points;

            inLayerBrightness = CheckBrightness(sine1Points, sine2Points);
            
            return inLayerBrightness;
        }

        /// <summary>
        /// Calculates and returns the average brighness between two Sines
        /// </summary>
        /// <param name="sine1">The first sine</param>
        /// <param name="sine2">The second sine</param>
        /// <returns></returns>
        public int GetBrightness(EdgeLine line1, EdgeLine line2)
        {
            inLayerBrightness = 0;

            List<Point> line1Points = new List<Point>();
            List<Point> line2Points = new List<Point>();

            line1Points = line1.Points;
            line2Points = line2.Points;

            inLayerBrightness = CheckBrightness(line1Points, line2Points);

            return inLayerBrightness;
        }

        private int CheckBrightness(List<Point> edge1Points, List<Point> edge2Points)
        {
            int totalBrightness = 0;
            int averageBrightness = 0;

            int count = 0;
            Color pixelColor;

            for (int x = 0; x < edge1Points.Count; x++)
            {
                int minY = Math.Min(edge1Points[x].Y, edge2Points[x].Y);
                int maxY = Math.Max(edge1Points[x].Y, edge2Points[x].Y);

                for (int y = minY; y <= maxY; y++)
                {
                    if (y < originalImage.Height && y >= 0)
                    {
                        pixelColor = originalImage.GetPixel(x, y);

                        int b = pixelColor.B & 0xff;
                        int g = pixelColor.G & 0xff;
                        int r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);

                        count++;
                    }
                }
            }

            averageBrightness = (int)Math.Floor((float)totalBrightness / (float)count);

            lastCount = count;

            return averageBrightness;
        }

        public bool IsDarker(Sine sine1, Sine sine2)
        {
            bool isDarker = false;

            int startPoint = (sine1.Depth - sine1.Amplitude) - 10;
            int endPoint = (sine1.Depth + sine1.Amplitude) + 10;

            int totalBrightness = 0;
            surroundingBrightness = 0;
            inLayerBrightness = 0;
            int count = 0;
            Color pixelColor;

            List<Point> sine1Points = new List<Point>();
            List<Point> sine2Points = new List<Point>();

            sine1Points = sine1.Points;
            sine2Points = sine2.Points;

            //Calculate brightness surrounding layer
            for (int x = 0; x < sine1Points.Count; x++)
            {
                for (int y = startPoint; y <= sine1Points[x].Y; y++)    //Before layer
                {
                    if (y > 0 && y < originalImage.Height)
                    {
                        pixelColor = originalImage.GetPixel(x, y);

                        int b = pixelColor.B & 0xff;
                        int g = pixelColor.G & 0xff;
                        int r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);
                    }

                    count++;
                }

                for (int y = sine2Points[x].Y; y <= endPoint; y++)    //After layer
                {
                    if (y > 0 && y < originalImage.Height)
                    {
                        pixelColor = originalImage.GetPixel(x, y);

                        int b = pixelColor.B & 0xff;
                        int g = pixelColor.G & 0xff;
                        int r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);
                    }

                    count++;
                }
            }

            surroundingBrightness = (int)Math.Floor((float)totalBrightness / (float)count);

            inLayerBrightness = GetBrightness(sine1, sine2);

            if (surroundingBrightness > inLayerBrightness)
            {
                isDarker = true;
            }
            else
            {
                isDarker = false;
            }

            return isDarker;
        }

        /// <summary>
        /// Returns the average brightness of the whole image
        /// </summary>
        /// <returns>The average brightness of the image</returns>
        public int GetImageBrightness()
        {
            CalculateImageBrightness();

            return averageImageBrightness;
        }

        /// <summary>
        /// Returns the number of pixels in the last layer that was checked
        /// </summary>
        /// <returns></returns>
        public int GetLastLayerCount()
        {
            return lastCount;
        }
    }
}
