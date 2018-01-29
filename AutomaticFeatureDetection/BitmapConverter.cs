using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace AutomaticFeatureDetection
{
    /// <summary>
    /// Converts a bitmap image to a one dimensional RGB int[] array
    /// 
    /// Author - Terry Malone (terrysmalone@hotmail.com)
    /// </summary>
    public static class BitmapConverter
    {
        private const int PixelWidth = 3;
        private const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;

        /// <summary>
        /// Returns an int[] array representing the RGB values of all pixels in the image
        /// </summary>
        /// <param name="image">The image</param>
        /// <returns>The int[] array of RGB values</returns>
        public static int[] GetRgbFromBitmap(Bitmap image)
        {
            var scanSize = image.Width;

            var rgbArray = new int[image.Width * image.Height];

            var data = image.LockBits(new Rectangle( 0, 0, image.Width, image.Height), 
                                      ImageLockMode.ReadOnly, PixelFormat);

            try
            {
                var pixelData = new byte[data.Stride];

                for (var row = 0; row < data.Height; row++)
                {
                    Marshal.Copy(data.Scan0 + (row * data.Stride), pixelData, 0, data.Stride);

                    for (var pixelOffset = 0; pixelOffset < data.Width; pixelOffset++)
                    {
                        rgbArray[(row * scanSize) + pixelOffset] =
                            (pixelData[pixelOffset * PixelWidth + 2] << 16) +   // R 
                            (pixelData[pixelOffset * PixelWidth + 1] << 8) +    // G
                             pixelData[pixelOffset * PixelWidth];               // B
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