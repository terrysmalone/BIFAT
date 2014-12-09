using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ImageTiler
{
    /// <summary>
    /// Deals with tiling a Bitmap borehole image
    /// </summary>
    public class BitmapTiler : FileTiler
    {
        int stride;        
        private int offset;

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="fileName">The image file</param>
        /// <param name="originalSectionHeight">The height of each section</param>
        public BitmapTiler(string fileName, int originalSectionHeight)
        {
            this.fileName = fileName;

            this.originalHeight = originalSectionHeight;

            CalculateImageProperties();

            if (boreholeWidth % 4 != 0)
                stride = (boreholeWidth + (4 - (719 % 4))) * 3;
            else
                stride = boreholeWidth * 3;

            currentSectionNumber = 0;
            originalSectionSize = originalSectionHeight * stride;
            int sectionSize = stride * boreholeHeight;

            //totalSize = Math.Min(originalSectionSize, sectionSize);
            totalSize = boreholeWidth * boreholeHeight * 3;
        }

        /// <summary>
        /// Loads the current section into the byte[] array currentSectionAsBytes
        /// </summary>
        protected override void LoadSection()
        {
            FileStream fs = File.OpenRead(fileName);
            
            calculateSectionEnds(fs);

            loadImageDataFromStream(fs);

            convertCurrentSectionToBitmap();
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
        /// Converts currentSectionAsBytes array to a bitmap
        /// </summary>
        private void convertCurrentSectionToBitmap()
        {
            currentSectionAsBitmap = new Bitmap(boreholeWidth, currentSectionHeight, PixelFormat.Format24bppRgb);

            
            BitmapData bmpData = currentSectionAsBitmap.LockBits(new Rectangle(0, 0, currentSectionAsBitmap.Width, currentSectionAsBitmap.Height), ImageLockMode.WriteOnly, currentSectionAsBitmap.PixelFormat);
            bmpData.Stride = stride;
            Marshal.Copy(currentSectionAsBytes, 0, bmpData.Scan0, currentSectionAsBytes.Length);

            currentSectionAsBitmap.UnlockBits(bmpData);

            currentSectionAsBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //currentSectionAsBitmap.Save("TestBitmap.bmp");
        }

        /// <summary>
        /// Calculates the image dimensions
        /// </summary>
        protected override void CalculateImageProperties()
        {
            BinaryReader reader;

            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open);

                try
                {
                    reader = new BinaryReader(stream);

                    reader.ReadChar();
                    reader.ReadChar();
                    reader.ReadInt32();
                    reader.ReadInt16();
                    reader.ReadInt16();
                    offset = reader.ReadInt32();
                    reader.ReadInt32();

                    boreholeWidth = reader.ReadInt32();
                    boreholeHeight = reader.ReadInt32();

                    reader.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not read bmp file: " + e.Message, "Project creation error");
                }
           

                stream.Close();
             }
            catch (Exception e)
            {
                MessageBox.Show("Could not read bmp file: " + e.Message, "Project creation error");
            }
        }

        private void loadImageDataFromStream(FileStream fs)
        {
            fs.Seek(sectionStart, SeekOrigin.Begin);

            //Should be 1937520
            currentSectionAsBytes = new byte[sectionSize];
            fs.Read(currentSectionAsBytes, 0, currentSectionAsBytes.Length);

            fs.Close();
        }

        /// <summary>
        /// Calcukates the sectionStart and sectionEnd values of the current section
        /// </summary>
        /// <param name="fs">The FileStream containing the current bitmap file</param>
        private void calculateSectionEnds(FileStream fs)
        {
            sectionStart = fs.Length - (originalSectionSize * (currentSectionNumber + 1));

            if (sectionStart < offset)
            {
                sectionStart = offset;
            }

            sectionEnd = fs.Length - (currentSectionNumber * originalSectionSize);

            sectionSize = (int)(sectionEnd - sectionStart);

            currentSectionHeight = Convert.ToInt32((double)sectionSize / ((double)stride/3.0) / 3.0);

            sectionStartHeight = (((currentSectionNumber + 1) * originalHeight) - originalHeight);
            sectionEndHeight = sectionStartHeight + currentSectionHeight-1;
        }



        /// <summary>
        /// Returns the current section as a byte array
        /// </summary>
        /// <returns>The current section</returns>
        public override byte[] GetCurrentSectionAsBytes()
        {
            Rectangle rect = new Rectangle(0, 0, currentSectionAsBitmap.Width, currentSectionAsBitmap.Height);

            //currentSectionAsBitmap.Save("currentSection.bmp");
            System.Drawing.Imaging.BitmapData bmpData = currentSectionAsBitmap.LockBits(
                rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int length = bmpData.Stride * bmpData.Height;

            byte[] bytes = new byte[length];

            Marshal.Copy(bmpData.Scan0, bytes, 0, length);
            currentSectionAsBitmap.UnlockBits(bmpData);

            //byte[] bytes = new byte[length];

            // Copy bitmap to byte[]
            //Marshal.Copy(bmpData.Scan0, bytes, 0, length);
            //currentSectionAsBitmap.UnlockBits(bmpData);

            //Remove padding
            //byte[] bytes = new byte[(bmpData.Width * (bmpData.Height) * 3)];
            //for (int y = 0; y < bmpData.Height; ++y)
            //{
            //    IntPtr mem = (IntPtr)(((long)bmpData.Scan0 + y * bmpData.Stride));
            //    Marshal.Copy(mem, bytes, y * bmpData.Width * 3, bmpData.Width * 3);                
            //}

            //currentSectionAsBitmap.UnlockBits(bmpData);

            //currentSectionAsBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            //MemoryStream ms = new MemoryStream();
            //// Save to memory using the Jpeg format
            //currentSectionAsBitmap.Save(ms, ImageFormat.Bmp);

            //// read to end
            //byte[] bmpBytes = ms.GetBuffer();

            //byte[] bmpData = new byte[bmpBytes.Length - 54];

            //for (int i = 0; i < bmpData.Length; i++)
            //{
            //    bmpData[i] = bmpBytes[i + 54];
            //}

            //ms.Close();

            //currentSectionAsBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bytes;
        }

        public override Bitmap GetCurrentSectionAsBitmap()
        {
            return currentSectionAsBitmap;
        }

        public override Bitmap GetWholeBoreholeImage()
        {
            Bitmap wholeImage = new Bitmap(fileName);

            return wholeImage;
        }
    }
}
