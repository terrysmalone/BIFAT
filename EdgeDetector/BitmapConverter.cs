using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EdgeDetector
{
    public static class BitmapConverter
    {
        /// <summary>
        /// Converts a bitmap to an RGB array where every cell represents one pixel
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <returns></returns>
        public static int[] BitmapToRGBArray(Bitmap sourceImage)
        {
#warning find better fix
            if (sourceImage.PixelFormat == PixelFormat.Format48bppRgb)
            {
                Bitmap copy = new Bitmap(sourceImage);

                sourceImage = new Bitmap(copy.Width, copy.Height, PixelFormat.Format24bppRgb);

                using (Graphics gr = Graphics.FromImage(sourceImage))
                {
                    gr.DrawImage(copy, new Rectangle(0, 0, sourceImage.Width, sourceImage.Height));
                }
            }

            int width = sourceImage.Width;
            int height = sourceImage.Height;
            BitmapData data = sourceImage.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            int[] rgbArray = new int[width * height];


            int pixelWidth = 3;
            int rOffset = 2;
            int gOffset = 1;
            int bOffset = 0;

            if (sourceImage.PixelFormat == PixelFormat.Format24bppRgb)
            {
                pixelWidth = 3;
                rOffset = 2;
                gOffset = 1;
                bOffset = 0;
            }
            else if (sourceImage.PixelFormat == PixelFormat.Format32bppRgb)
            {
                pixelWidth = 4;
                rOffset = 2;
                gOffset = 1;
                bOffset = 0;
            }
            else if (sourceImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                pixelWidth = 4;
                rOffset = 2;
                gOffset = 1;
                bOffset = 0;
            }
            else if (sourceImage.PixelFormat == PixelFormat.Format48bppRgb)
            {
                //Should never get here since 48bpp images are converted to 24bpp

                // pixelWidth = 3;
                // rOffset = 2;
                // gOffset = 1;
                //  bOffset = 0;
            }

            try
            {
                byte[] pixelData = new Byte[data.Stride];

                for (int row = 0; row < data.Height; row++)
                {
                    Int64 scanPos = (Int64)data.Scan0;
                    IntPtr ptrData = new IntPtr(scanPos + (row * data.Stride));
                    Marshal.Copy(ptrData, pixelData, 0, data.Stride);

                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        rgbArray[0 + (row * width) + pixeloffset] =
                            (pixelData[pixeloffset * pixelWidth + rOffset] << 16) +   // R 
                            (pixelData[pixeloffset * pixelWidth + gOffset] << 8) +    // G
                            pixelData[pixeloffset * pixelWidth + bOffset];                // B
                    }
                }
            }
            finally
            {
                sourceImage.UnlockBits(data);
            }

            return rgbArray;
        }

        /// <summary>
        /// Converts a bitmap to an RGB array where every cell represents one pixel
        /// </summary>
        /// <param name="sourceArray"></param>
        public static Bitmap RGBArrayToBitmap(int[] sourceArray, int imageWidth, int imageHeight)
        {
            Debug.Assert(sourceArray.Length == imageWidth * imageHeight);

            Bitmap image = new Bitmap(imageWidth, imageHeight);

            int offset = 0;

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    int pixelValue = sourceArray[offset];

                    int r = (pixelValue & 0xff0000) >> 16;
                    int g = (pixelValue & 0xff00) >> 8;
                    int b = pixelValue & 0xff;

                    image.SetPixel(x, y, Color.FromArgb(r, g, b));

                    offset++;
                }
            }

            return image;
        }
    }
}