using System;
using System.Collections.Generic;
using System.Drawing;
using EdgeFitting;

namespace LayerDetection
{
    /// <summary>
    /// Class which calculates the average brightness between two sinusoids
    /// in an image
    /// </summary>
    internal class LayerBrightness
    {
        private readonly Bitmap m_originalImage;
        private readonly int m_imageWidth;
        private readonly int m_imageHeight;

        private int m_inLayerBrightness;
        private int m_surroundingBrightness;
        private int m_averageImageBrightness;

        private int m_lastCount;

        # region Constructors

        /// <summary>
        /// Constuctor method
        /// </summary>
        /// <param name="originalImage">The original image which is to be checked</param>
        public LayerBrightness(Bitmap originalImage)
        {
            m_originalImage = originalImage;
            m_imageWidth = originalImage.Width;
            m_imageHeight = originalImage.Height;
        }

        # endregion

        /// <summary>
        /// Method which calculates the average brightness of the entire image (0 = black 255 = white)
        /// </summary>
        private void CalculateImageBrightness()
        {
            var totalImageBrightness = 0;

            const int widthSpacing = 5;
            const int heightSpacing = 5;

            for (var x = 0; x < m_imageWidth; x += widthSpacing)
            {
                for (var y = 0; y < m_imageHeight; y += heightSpacing)
                {
                    var pixelColor = m_originalImage.GetPixel(x, y);

                    var b = pixelColor.B & 0xff;
                    var g = pixelColor.G & 0xff;
                    var r = pixelColor.R & 0xff;

                    totalImageBrightness += CalculateBrightness(r, g, b);
                }
            }

            var widthResolution = m_imageWidth / (double)widthSpacing;
            var heightResolution = m_imageHeight / (double)heightSpacing;

            m_averageImageBrightness = (int)Math.Floor(totalImageBrightness
                                                       / ((float)widthResolution * (float)heightResolution));
        }

        // Returns the brightness value of a given RGB
        private static int CalculateBrightness(float r, float g, float b)
        {
            return (int)Math.Floor(0.334f * r + 0.333f * g + 0.333f * b);
        }

        //Calculates and returns the average brighness between two Sines
        public int GetBrightness(Sine sine1, Sine sine2)
        {
            m_inLayerBrightness = 0;

            var sine1Points = sine1.Points;
            var sine2Points = sine2.Points;

            m_inLayerBrightness = CheckBrightness(sine1Points, sine2Points);
            
            return m_inLayerBrightness;
        }

        // Calculates and returns the average brighness between two Sines
        public int GetBrightness(EdgeLine line1, EdgeLine line2)
        {
            m_inLayerBrightness = 0;

            var line1Points = line1.Points;
            var line2Points = line2.Points;

            m_inLayerBrightness = CheckBrightness(line1Points, line2Points);

            return m_inLayerBrightness;
        }

        private int CheckBrightness(IList<Point> edge1Points, IList<Point> edge2Points)
        {
            var totalBrightness = 0;

            var count = 0;

            for (var x = 0; x < edge1Points.Count; x++)
            {
                var minY = Math.Min(edge1Points[x].Y, edge2Points[x].Y);
                var maxY = Math.Max(edge1Points[x].Y, edge2Points[x].Y);

                for (var y = minY; y <= maxY; y++)
                {
                    if (y < m_originalImage.Height && y >= 0)
                    {
                        var pixelColor = m_originalImage.GetPixel(x, y);

                        var b = pixelColor.B & 0xff;
                        var g = pixelColor.G & 0xff;
                        var r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);

                        count++;
                    }
                }
            }

            var averageBrightness = (int)Math.Floor(totalBrightness / (float)count);

            m_lastCount = count;

            return averageBrightness;
        }

        public bool IsDarker(Sine sine1, Sine sine2)
        {
            const int detectionSize = 10;

            var totalBrightness = 0;
            m_surroundingBrightness = 0;
            m_inLayerBrightness = 0;
            var count = 0;

            var sine1Points = sine1.Points;
            var sine2Points = sine2.Points;

            //Calculate brightness surrounding layer
            for (var x = 0; x < sine1Points.Count; x++)
            {
                Color pixelColor;

                var sine1Point = sine1Points[x];

                for (var y = sine1Point.Y-detectionSize; y <= sine1Point.Y; y++)    //Before layer
                {
                    if (y > 0 && y < m_originalImage.Height)
                    {
                        pixelColor = m_originalImage.GetPixel(x, y);

                        var b = pixelColor.B & 0xff;
                        var g = pixelColor.G & 0xff;
                        var r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);
                    }

                    count++;
                }

                var sine2Point = sine2Points[x];


                for (var y = sine2Point.Y; y <= sine2Point.Y + detectionSize; y++)    //After layer
                {
                    if (y > 0 && y < m_originalImage.Height)
                    {
                        pixelColor = m_originalImage.GetPixel(x, y);

                        var b = pixelColor.B & 0xff;
                        var g = pixelColor.G & 0xff;
                        var r = pixelColor.R & 0xff;

                        totalBrightness += CalculateBrightness(r, g, b);
                    }

                    count++;
                }
            }

            m_surroundingBrightness = (int)Math.Floor(totalBrightness / (float)count);

            m_inLayerBrightness = GetBrightness(sine1, sine2);

            var isDarker = m_surroundingBrightness > m_inLayerBrightness;

            return isDarker;
        }

        /// <summary>
        /// Returns the average brightness of the whole image
        /// </summary>
        /// <returns>The average brightness of the image</returns>
        public int GetImageBrightness()
        {
            CalculateImageBrightness();

            return m_averageImageBrightness;
        }

        /// <summary>
        /// Returns the number of pixels in the last layer that was checked
        /// </summary>
        /// <returns></returns>
        public int GetLastLayerCount()
        {
            return m_lastCount;
        }
    }
}
