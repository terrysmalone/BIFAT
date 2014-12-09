using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FeatureAnnotationTool
{
    /// <summary>
    /// Rotates image through 360 degrees of the borehole to allow for wrapping
    /// </summary>
    class ImageRotation
    {
        private Bitmap originalImage;
        private Bitmap rotatedImage;

        private int imageWidth, imageHeight;

        public ImageRotation(Bitmap originalImage)
        {
            this.originalImage = originalImage;
            imageWidth = originalImage.Width;
            imageHeight = originalImage.Height;

            rotatedImage = new Bitmap(originalImage.Width, originalImage.Height, originalImage.PixelFormat);
        }

        public void RotateImageBy(int rotation)
        {
            int imageSplitPosition = (int)((double)rotation * ((double)imageWidth / (double)360));
           
            Rectangle leftRect = new Rectangle(0,0,imageSplitPosition, imageHeight);
            Rectangle rightRect = new Rectangle(imageSplitPosition,0,imageWidth-imageSplitPosition, imageHeight);

            System.Drawing.Imaging.PixelFormat format = originalImage.PixelFormat;

            Bitmap leftSection = originalImage.Clone(leftRect, format);
            Bitmap rightSection = originalImage.Clone(rightRect, format);

            Graphics g = Graphics.FromImage(rotatedImage);

            g.DrawImage(rightSection, 0, 0);
            g.DrawImage(leftSection, rightSection.Width, 0);
        }

        public Bitmap GetRotatedImage()
        {
            return rotatedImage;
        }
    }
}
