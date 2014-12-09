using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using RGReferencedData;
using RGPreProcessedData;
using System.IO;

namespace ImageTiler
{
    /// <summary>
    /// Class which implements a Tiler from a given ReferencedChannel
    /// </summary>
    public class ChannelTiler : FileTiler
    {
        private ReferencedChannel optvChannel;

        public ChannelTiler(ReferencedChannel optvChannel, int sectionHeight)
        {
            this.optvChannel = optvChannel;
            this.originalHeight = sectionHeight;

            CalculateImageProperties();

            this.originalHeight = sectionHeight;

            currentSectionNumber = 0;
            originalSectionSize = sectionHeight * 3 * boreholeWidth;
            totalSize = boreholeWidth * boreholeHeight * 3;
        }

        /// <summary>
        /// Calculates the height and width of the borehole channel
        /// </summary>
        protected override void CalculateImageProperties()
        {
            boreholeHeight = optvChannel.Session.Bottom - optvChannel.Session.Top;
            boreholeWidth = optvChannel.PreProcessedSource.RawSource.SampleCount;
        }

        /// <summary>
        /// Loads the current section as an image
        /// </summary>
        protected override void LoadSection()
        {
            calculateSectionEnds();

            if (optvChannel.Mode.LoggingMode.ToString() == "Up")
            {
                currentSectionAsBitmap = ((BitmapChannel)(optvChannel.PreProcessedSource)).GetBitmap((int)sectionStart, (int)sectionEnd);
            }
            else
            {
                currentSectionAsBitmap = ((BitmapChannel)(optvChannel.PreProcessedSource)).GetBitmap((int)sectionStart, (int)sectionEnd);
                currentSectionAsBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            MemoryStream ms = new MemoryStream();

            currentSectionAsBitmap.Save(ms, ImageFormat.Bmp);

            byte[] bmpBytes = ms.GetBuffer();

            currentSectionAsBytes = new byte[bmpBytes.Length - 54];

            for (int i = 0; i < currentSectionAsBytes.Length; i++)
            {
                currentSectionAsBytes[i] = bmpBytes[i + 54];
            }

            ms.Close();

            currentSectionAsBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public override void LoadSpecificSection(int startHeight, int endHeight)
        {
            //Only implemented in FeaturesImageTiler
        }

        public override Bitmap GetSpecificSectionAsBitmap(int startHeight, int endHeight)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the heights of the start and end of the current section
        /// </summary>
        private void calculateSectionEnds()
        {
            if (optvChannel.Mode.LoggingMode.ToString() == "Up")
            {
                sectionEnd = boreholeHeight - ((currentSectionNumber) * originalHeight) - 1;

                sectionStart = sectionEnd - originalHeight + 1;

                if (sectionStart < 0)
                {
                    sectionStart = 0;
                }
            }
            else
            {
                sectionStart = currentSectionNumber * originalHeight;

                sectionEnd = sectionStart + originalHeight - 1;

                if (sectionEnd >= boreholeHeight)
                {
                    sectionEnd = boreholeHeight - 1;
                }
            }

            currentSectionHeight = (int)(sectionEnd - sectionStart + 1);

            sectionSize = currentSectionHeight * 3 * boreholeWidth;

            sectionStartHeight = (((currentSectionNumber + 1) * originalHeight) - originalHeight);

            sectionEndHeight = sectionStartHeight + currentSectionHeight - 1;
        }

        private Bitmap convertCurrentSectionToBitmap()
        {
            Bitmap boreholeImage = new Bitmap(boreholeWidth, currentSectionHeight, PixelFormat.Format24bppRgb);

            BitmapData bmpData = boreholeImage.LockBits(new Rectangle(0, 0, boreholeImage.Width, boreholeImage.Height), ImageLockMode.WriteOnly, boreholeImage.PixelFormat);

            Marshal.Copy(currentSectionAsBytes, 0, bmpData.Scan0, currentSectionAsBytes.Length);

            boreholeImage.UnlockBits(bmpData);

            boreholeImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return boreholeImage;
        }

        public override Bitmap GetWholeBoreholeImage()
        {
            //TODO add exception handling for memory exception when image is too large
            //OR remove getWholeBoreholeImage()

            Bitmap wholeImage = new Bitmap(boreholeWidth, boreholeHeight);

            wholeImage = ((BitmapChannel)(optvChannel.PreProcessedSource)).GetBitmap();

            return wholeImage;
        }

        public override Bitmap GetCurrentSectionAsBitmap()
        {
            return currentSectionAsBitmap;
        }

        public void setImageStartPosition(long imageStartPosition)
        {
            this.imageStartPosition = imageStartPosition;
            LoadSection();
        }

        public override byte[] GetCurrentSectionAsBytes()
        {
            return currentSectionAsBytes;
        }
    }

}
