using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using RGReferencedData;
using RGLogDataSet;
using System.Drawing;
using System.Runtime.InteropServices;
using RGPreProcessedData;

namespace ImageTiler
{
    /// <summary>
    /// A class which deals with tiling a bitmap from an otv file
    /// 
    /// 
    /// </summary>
    public class OTVImageTiler : FileTiler
    {
        private ReferencedChannel optvChannel;
        private LogDataSet logDataSet;

        public OTVImageTiler(string fileName, int sectionHeight)
        {
            this.fileName = fileName;
            this.originalHeight = sectionHeight;

            string workingFolder = System.IO.Path.Combine(Path.GetDirectoryName(fileName), "working");

            if (Directory.Exists(workingFolder))
                Directory.Delete(workingFolder, true);

            System.IO.Directory.CreateDirectory(workingFolder);

            logDataSet = new LogDataSet(workingFolder);

            logDataSet.Create(fileName);
            
            optvChannel = logDataSet.Referenced.GetChannel("OPTV");
            
            CalculateImageProperties();

            this.originalHeight = sectionHeight;

            currentSectionNumber = 0;
            originalSectionSize = sectionHeight * 3 * boreholeWidth;
            totalSize = boreholeWidth * boreholeHeight * 3;
        }

        protected override void CalculateImageProperties()
        {
            boreholeHeight = optvChannel.Session.Bottom - optvChannel.Session.Top;

            boreholeHeight = (int)((double)boreholeHeight / optvChannel.Session.Step);

            boreholeWidth = optvChannel.PreProcessedSource.RawSource.SampleCount;
        }

        protected override void LoadSection()
        {
            calculateSectionEnds();

            if (optvChannel.Mode.LoggingMode.ToString() == "Up")
                currentSectionAsBitmap = ((BitmapChannel)(optvChannel.PreProcessedSource)).GetBitmap((int)sectionStart, (int)sectionEnd);
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

        public void setImageStartPosition(long imageStartPosition)
        {
            this.imageStartPosition = imageStartPosition;
            LoadSection();
        }

        public override byte[] GetCurrentSectionAsBytes()
        {
            return currentSectionAsBytes;
        }

        public override Bitmap GetCurrentSectionAsBitmap()
        {
            return currentSectionAsBitmap;
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
            Bitmap wholeImage = new Bitmap(fileName);

            return wholeImage;
        }

        public void RemoveWorkingFolder()
        {
            try
            {

                string workingFolder = System.IO.Path.Combine(Path.GetDirectoryName(fileName), "working");

                logDataSet.Dispose();
                optvChannel = null;

                if (Directory.Exists(workingFolder))
                    Directory.Delete(workingFolder, true);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error deleting working folder: " + e.Message);
            }
        }

        public void DisposeTiler()
        {
            logDataSet.Referenced.Clear();
            logDataSet.Dispose();
        }
    }
}
