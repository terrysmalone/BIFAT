using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AutomaticFeatureDetection
{
    /// <summary>
    /// Converts a bitmap image to an RGB int[] array
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1 Refactored
    /// </summary>
    public static class BitmapConverter
    {
        private const int PixelWidth = 3;
        private const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;

        /// <summary>
        /// Returns an int[] array representing the RGB values of alkl pixels in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>The int[] array of RGB values</returns>
        public static int[] GetRgbFromBitmap(Bitmap image)
        {
            const int startX = 0;
            const int startY = 0;
            const int offset = 0;

            var scansize = image.Width;

            var rgbArray = new int[image.Width * image.Height];

            var data = image.LockBits(new Rectangle(startX, startY, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat);

            try
            {
                var pixelData = new Byte[data.Stride];

                for (var row = 0; row < data.Height; row++)
                {
                    Marshal.Copy(data.Scan0 + (row * data.Stride), pixelData, 0, data.Stride);

                    for (var pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        rgbArray[offset + (row * scansize) + pixeloffset] =
                            (pixelData[pixeloffset * PixelWidth + 2] << 16) +   // R 
                            (pixelData[pixeloffset * PixelWidth + 1] << 8) +    // G
                            pixelData[pixeloffset * PixelWidth];                // B
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }

            return rgbArray;
        }
    }
}