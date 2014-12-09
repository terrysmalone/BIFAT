using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ImageTiler
{
    /// <summary>
    /// Class which deals with tiling a features file
    /// 
    /// author - Terry Malone (trm8@aber.ac.uk)
    /// </summary>
    public class FeaturesImageTiler : FileTiler
    {
        int stride;

        public FeaturesImageTiler(string fileName, int sectionHeight)
        {
            this.fileName = fileName;
            this.originalHeight = sectionHeight;

            CalculateImageProperties();

            this.originalHeight = sectionHeight;

            if (boreholeWidth % 4 != 0)
                stride = (boreholeWidth + (4 - (719 % 4))) * 3;
            else
                stride = boreholeWidth * 3;

            originalSectionSize = sectionHeight * stride;
            totalSize = stride * boreholeHeight;
        }

        /// <summary>
        /// Calculates the properties of the image
        /// Properties:
        /// imageStartPosition
        /// boreholeWidth
        /// boreholeHeight
        /// </summary>
        protected override void CalculateImageProperties()
        {
            imageStartPosition = 0;
            BinaryReader reader;

            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open);

                try
                {
                    reader = new BinaryReader(stream);

                    boreholeWidth = reader.ReadInt32();
                    boreholeHeight = reader.ReadInt32();

                    imageStartPosition = 8;

                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Exception caugth in BinaryReader: " + e.Message);
                }

                stream.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception caugth in FileStream: " + e.Message);
            }
        }

        protected override void LoadSection()
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                calculateSectionEnds(fs);
            }
        }

        public override void LoadSpecificSection(int startHeight, int endHeight)
        {
            FileStream fs = File.OpenRead(fileName);

            //sectionStart = imageStartPosition + (startHeight * 3 * boreholeWidth);

            //sectionEnd = imageStartPosition + (endHeight * 3 * boreholeWidth);

            //sectionSize = (int)(sectionEnd - sectionStart + 3);

            //currentSectionHeight = sectionSize / boreholeWidth / 3;

            //currentSectionAsBytes = new byte[sectionSize];

            sectionStart = imageStartPosition + (startHeight * stride);

            sectionEnd = imageStartPosition + (endHeight * stride);

            if (sectionEnd > totalSize + imageStartPosition)
            {
                sectionEnd = totalSize + imageStartPosition;
            }

            //sectionSize = (int)(sectionEnd - sectionStart + (720*3));
            sectionSize = (int)(sectionEnd - sectionStart);
           // currentSectionHeight = sectionSize / ((stride/3) / 3);
            currentSectionHeight = sectionSize / stride;
            
            fs.Seek(sectionStart, SeekOrigin.Begin);

            currentSectionAsBytes = new byte[sectionSize];
            fs.Read(currentSectionAsBytes, 0, currentSectionAsBytes.Length);

            currentSectionAsBitmap = new Bitmap(boreholeWidth, currentSectionHeight, PixelFormat.Format24bppRgb);

            BitmapData bmpData = currentSectionAsBitmap.LockBits(new Rectangle(0, 0, currentSectionAsBitmap.Width, currentSectionAsBitmap.Height), ImageLockMode.WriteOnly, currentSectionAsBitmap.PixelFormat);
            bmpData.Stride = stride;

            Marshal.Copy(currentSectionAsBytes, 0, bmpData.Scan0, currentSectionAsBytes.Length);

            currentSectionAsBitmap.UnlockBits(bmpData);

            fs.Close();
        }

        public override Bitmap GetSpecificSectionAsBitmap(int startHeight, int endHeight)
        {
            GC.Collect();

            using (FileStream fs = File.OpenRead(fileName))
            {
                Bitmap sectionAsBitmap;

                endHeight++;

                long start = imageStartPosition + (startHeight * 3 * boreholeWidth);
                long end = imageStartPosition + (endHeight * 3 * boreholeWidth);

                if (end > totalSize + imageStartPosition)
                {
                    end = totalSize + imageStartPosition;
                }

                int size = (int)(end - start);
                int sectionHeight = size / boreholeWidth / 3;
                fs.Seek(start, SeekOrigin.Begin);

                byte[] sectionAsBytes = new byte[size];
                fs.Read(sectionAsBytes, 0, sectionAsBytes.Length);

                sectionAsBitmap = new Bitmap(boreholeWidth, sectionHeight, PixelFormat.Format24bppRgb);

                BitmapData bmpData = sectionAsBitmap.LockBits(new Rectangle(0, 0, sectionAsBitmap.Width, sectionAsBitmap.Height), ImageLockMode.WriteOnly, sectionAsBitmap.PixelFormat);

                Marshal.Copy(sectionAsBytes, 0, bmpData.Scan0, sectionAsBytes.Length);

                sectionAsBitmap.UnlockBits(bmpData);

                return sectionAsBitmap;
            }
        }

        private void calculateSectionEnds(FileStream fs)
        {
            sectionStart = imageStartPosition + (originalSectionSize * (currentSectionNumber));

            sectionEnd = sectionStart + originalSectionSize;

            if (sectionEnd > totalSize + imageStartPosition)
            {
                sectionEnd = totalSize + imageStartPosition;
            }

            sectionSize = (int)(sectionEnd - sectionStart);

            currentSectionHeight = Convert.ToInt32((double)sectionSize / ((double)stride / 3.0) / 3.0);

            sectionStartHeight = ((currentSectionNumber) * originalHeight);

            sectionEndHeight = sectionStartHeight + currentSectionHeight-1;
        }

        public override byte[] GetCurrentSectionAsBytes()
        {
            byte[] byteSection = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                try
                {
                    byteSection = new byte[sectionSize];

                    fs.Seek(sectionStart, SeekOrigin.Begin);

                    byteSection = new byte[sectionSize];
                    fs.Read(byteSection, 0, byteSection.Length);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error with fileStream: " + e.Message);
                }
            }

            return byteSection;
        }

        public override Bitmap GetCurrentSectionAsBitmap()
        {
            Bitmap currentBitmap = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                try
                {
                    byte[] byteSection = new byte[sectionSize];

                    fs.Seek(sectionStart, SeekOrigin.Begin);

                    byteSection = new byte[sectionSize];
                    fs.Read(byteSection, 0, byteSection.Length);

                    currentBitmap = new Bitmap(boreholeWidth, currentSectionHeight, PixelFormat.Format24bppRgb);

                    BitmapData bmpData = currentBitmap.LockBits(new Rectangle(0, 0, currentBitmap.Width, currentBitmap.Height), ImageLockMode.WriteOnly, currentBitmap.PixelFormat);

                    Marshal.Copy(byteSection, 0, bmpData.Scan0, byteSection.Length);

                    currentBitmap.UnlockBits(bmpData);

                    if (byteSection != null)
                        byteSection = null;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error with fileStream: " + e.Message);
                }
            }

            return currentBitmap;
        }

        public void setImageStartPosition(long imageStartPosition)
        {
            this.imageStartPosition = imageStartPosition;
        }

        public override Bitmap GetWholeBoreholeImage()
        {
            FileStream fs = File.OpenRead(fileName);

            fs.Seek(imageStartPosition, SeekOrigin.Begin);

            currentSectionAsBytes = new byte[totalSize];
            fs.Read(currentSectionAsBytes, 0, currentSectionAsBytes.Length);

            Bitmap wholeImage = new Bitmap(boreholeWidth, boreholeHeight, PixelFormat.Format24bppRgb);

            BitmapData bmpData = wholeImage.LockBits(new Rectangle(0, 0, wholeImage.Width, wholeImage.Height), ImageLockMode.WriteOnly, wholeImage.PixelFormat);

            Marshal.Copy(currentSectionAsBytes, 0, bmpData.Scan0, currentSectionAsBytes.Length);

            wholeImage.UnlockBits(bmpData);

            fs.Close();

            return wholeImage;
        }
    }

}
