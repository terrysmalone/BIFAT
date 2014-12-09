using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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
        private const int PIXEL_WIDTH = 3;
        private const PixelFormat PIXEL_FORMAT = PixelFormat.Format24bppRgb;

        /// <summary>
        /// Returns an int[] array representing the RGB values of alkl pixels in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>The int[] array of RGB values</returns>
        public static int[] getRGBFromBitmap(Bitmap image)
        {
            int startX = 0;
            int startY = 0;
            int offset = 0;

            int scansize = image.Width;

            int[] rgbArray = new int[image.Width * image.Height];

            BitmapData data = image.LockBits(new Rectangle(startX, startY, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, PIXEL_FORMAT);

            try
            {
                byte[] pixelData = new Byte[data.Stride];

                for (int row = 0; row < data.Height; row++)
                {
                    Marshal.Copy(data.Scan0 + (row * data.Stride), pixelData, 0, data.Stride);

                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        rgbArray[offset + (row * scansize) + pixeloffset] =
                            (pixelData[pixeloffset * PIXEL_WIDTH + 2] << 16) +   // R 
                            (pixelData[pixeloffset * PIXEL_WIDTH + 1] << 8) +    // G
                            pixelData[pixeloffset * PIXEL_WIDTH];                // B
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