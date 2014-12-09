using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace EdgeDetector
{
    /// <summary>
    /// Scales arrays to Bitmap images
    /// </summary>
    public static class ScaleDataToImage
    {
        #region int to bitmap

        public static Bitmap ScaleToDataRange(int[] filteredPixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            int min, max;

            int range = CalculateRange(filteredPixels, out min, out max);

            Debug.Assert(range > 0);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / (double)range)));

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToGivenRange(int[] filteredPixels, int width, int height, int min, int max)
        {
            Bitmap bitmap = new Bitmap(width, height);

            int range = max - min;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / (double)range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToDataRange(int[,] filteredPixels)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            int min, max;

            int range = CalculateRange(filteredPixels, out min, out max);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / (double)range)));

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToGivenRange(int[,] filteredPixels, int min, int max)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            int range = max - min;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / (double)range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        #endregion int to bitmap

        #region float to bitmap

        public static Bitmap ScaleToDataRange(float[] filteredPixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            float min, max;

            float range = CalculateRange(filteredPixels, out min, out max);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / range)));

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToGivenRange(float[] filteredPixels, int width, int height, float min, float max)
        {
            Bitmap bitmap = new Bitmap(width, height);

            float range = max - min;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToDataRange(float[,] filteredPixels)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            float min, max;

            float range = CalculateRange(filteredPixels, out min, out max);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / range)));

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a scaled image based on the given min and max values.
        /// Any values outwith the range will be treated as at that end of the range
        /// </summary>
        /// <param name="filteredPixels"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Bitmap ScaleToGivenRange(float[,] filteredPixels, float min, float max)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            float range = max - min;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        #endregion float to bitmap

        #region double to bitmap

        public static Bitmap ScaleToDataRange(double[] filteredPixels, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            double min, max;

            double range = CalculateRange(filteredPixels, out min, out max);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / range)));

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToGivenRange(double[] filteredPixels, int width, int height, double min, double max)
        {
            Bitmap bitmap = new Bitmap(width, height);

            double range = max - min;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int currentPos = (y * width) + x;

                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[currentPos] - min) * (255 / range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(x, y, Color.FromArgb(scaledValue, scaledValue, scaledValue));
                }
            }

            return bitmap;
        }

        public static Bitmap ScaleToDataRange(double[,] filteredPixels)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            double min, max;

            double range = CalculateRange(filteredPixels, out min, out max);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / range)));

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a scaled image based on the given min and max values.
        /// Any values outwith the range will be treated as at that end of the range
        /// </summary>
        /// <param name="filteredPixels"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static Bitmap ScaleToGivenRange(double[,] filteredPixels, double min, double max)
        {
            int width = filteredPixels.GetLength(0);
            int height = filteredPixels.GetLength(1);

            Bitmap bitmap = new Bitmap(width, height);

            double range = max - min;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int scaledValue = Convert.ToInt32(Math.Round((filteredPixels[i, j] - min) * (255 / range)));

                    if (scaledValue > 255)
                        scaledValue = 255;

                    if (scaledValue < 0)
                        scaledValue = 0;

                    bitmap.SetPixel(i, j, Color.FromArgb(scaledValue, scaledValue, scaledValue));

                }
            }

            return bitmap;
        }

        #endregion double to bitmap

        #region calculate range values

        private static int CalculateRange(int[] filteredPixels, out int min, out int max)
        {
            min = int.MaxValue;
            max = int.MinValue;

            for (int i = 0; i < filteredPixels.Length; i++)
            {
                int currentValue = filteredPixels[i];

                if (currentValue > max)
                    max = currentValue;

                if (currentValue < min)
                    min = currentValue;
            }

            return max - min;
        }

        private static int CalculateRange(int[,] filteredPixels, out int min, out int max)
        {
            min = int.MaxValue;
            max = int.MinValue;

            for (int i = 0; i < filteredPixels.GetLength(0); i++)
            {
                for (int j = 0; j < filteredPixels.GetLength(1); j++)
                {
                    int currentValue = filteredPixels[i, j];

                    if (currentValue > max)
                        max = currentValue;

                    if (currentValue < min)
                        min = currentValue;
                }
            }

            return max - min;
        }

        private static float CalculateRange(float[] filteredPixels, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < filteredPixels.Length; i++)
            {
                float currentValue = filteredPixels[i];

                if (currentValue > max)
                    max = currentValue;

                if (currentValue < min)
                    min = currentValue;
            }

            return max - min;
        }

        private static float CalculateRange(float[,] filteredPixels, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < filteredPixels.GetLength(0); i++)
            {
                for (int j = 0; j < filteredPixels.GetLength(1); j++)
                {
                    float currentValue = filteredPixels[i, j];

                    if (currentValue > max)
                        max = currentValue;

                    if (currentValue < min)
                        min = currentValue;
                }
            }

            return max - min;
        }

        private static double CalculateRange(double[] filteredPixels, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            for (int i = 0; i < filteredPixels.Length; i++)
            {
                double currentValue = filteredPixels[i];

                if (currentValue > max)
                    max = currentValue;

                if (currentValue < min)
                    min = currentValue;
            }

            return max - min;
        }

        private static double CalculateRange(double[,] filteredPixels, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;

            for (int i = 0; i < filteredPixels.GetLength(0); i++)
            {
                for (int j = 0; j < filteredPixels.GetLength(1); j++)
                {
                    double currentValue = filteredPixels[i, j];

                    if (currentValue > max)
                        max = currentValue;

                    if (currentValue < min)
                        min = currentValue;
                }
            }

            return max - min;
        }

        #endregion calculate range values
    }
}

