using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LargeBitmapFileCreator
{
    /// <summary>
    /// Writes large bitmap files from given data arrays. 
    /// ***Chunks must be given in reverse order***
    /// </summary>
    public class LargeBMPFileCreator
    {
        string filePath;
        string path;
        Int32 width, height;
        Int32 size;
        Int16 reserved = 0;

        private int currentDataPosition;

        private int sectionHeight = 0;
        private int currentSection = 0;

        private int totalSections;
        private int heightThisSection = 0;
        private int lastSectionHeight;

        private int currentSectionHeight;

        public LargeBMPFileCreator(string filePath, int width, int height)
        {
            this.filePath = filePath;
            this.width = width;
            this.height = height;

            currentDataPosition = 54;

            sectionHeight = height;

            WriteFileHeaders();
        }

        public LargeBMPFileCreator(string filePath, int width, int height, int sectionHeight)
        {
            this.filePath = filePath;
            this.width = width;
            this.height = height;

            this.sectionHeight = sectionHeight;
            
            currentDataPosition = 54;

            WriteFileHeaders();
        }

        /// <summary>
        /// Writes the headers for a bitmap that is being split into multiple image files
        /// </summary>
        private void WriteFileHeaders()
        {
            //Calculate how many files are to be written and write all their headers

            if (sectionHeight >= height)            //Only one file is being created
            {
                size = ((int)((double)width * (double)height * (double)3) + 54);
                path = filePath;
                currentSectionHeight = sectionHeight;
                WriteFileHeader();

                totalSections = 1;
                currentSection = 1;

                lastSectionHeight = sectionHeight;
                
            }
            else
            {
                int currentHeight = 0;
                int count = 0;

                while (currentHeight < height)
                {
                    count++;

                    currentHeight += sectionHeight;

                    if (currentHeight < height) //Not last section
                    {
                        size = ((int)((double)width * (double)sectionHeight * (double)3) + 54);
                        path = filePath.Substring(0, filePath.Length-4) + "_" + count.ToString() + ".bmp";
                        
                        currentSectionHeight = sectionHeight;
                        
                        WriteFileHeader();
                    }
                    else                        //last section                
                    {
                        size = ((int)((double)width * (double)(sectionHeight - (currentHeight-height)) * (double)3) + 54);
                        path = filePath.Substring(0, filePath.Length - 4) + "_" + count.ToString() + ".bmp";
                        lastSectionHeight = height % sectionHeight;

                        if (lastSectionHeight == 0)
                            lastSectionHeight = sectionHeight;

                        currentSectionHeight = lastSectionHeight;

                        WriteFileHeader();
                    }
                }

                totalSections = count;
                currentSection = count;                
            }
        }

        /// <summary>
        /// Writes the header for a bitmap 
        /// </summary>
        public void WriteFileHeader()
        {
            BinaryWriter writer;

            try
            {
                FileStream stream = new FileStream(path, FileMode.Create);

                char b = 'B';
                char m = 'M';
                Int32 offset = 54;
                Int32 bitmapInfoHeader = 40;
                Int16 planes = 1;
                Int32 bitCount = 24;
                Int16 compression = 0;
                Int32 sizeOfImageData = size - offset;
                Int32 ppm = 3780;
                Int32 colours = 0;

                try
                {
                    writer = new BinaryWriter(stream);

                    writer.Write(b);
                    writer.Write(m);
                    writer.Write(size);
                    writer.Write(reserved);
                    writer.Write(reserved);
                    writer.Write(offset);

                    writer.Write(bitmapInfoHeader);
                    writer.Write(width);
                    writer.Write(currentSectionHeight);
                    writer.Write(planes);
                    writer.Write(bitCount);
                    writer.Write(compression);
                    writer.Write(sizeOfImageData);
                    writer.Write(ppm);
                    writer.Write(ppm);
                    writer.Write(colours);
                    writer.Write(colours);

                    writer.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caugth in BinaryWriter: " + e.Message);
                    Console.ReadLine();
                }

                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caugth in FileStream: " + e.Message);
                Console.ReadLine();
            }
        }

        public void ReadFileheader()
        {
            BinaryReader reader;

            try
            {
                FileStream stream = new FileStream(path, FileMode.Open);

                try
                {
                    reader = new BinaryReader(stream);

                    Console.WriteLine("Type: " + reader.ReadChar() + reader.ReadChar() + " - position: " + stream.Position);
                    Console.WriteLine("Size: " + reader.ReadInt32() + " - position: " + stream.Position);
                    Console.WriteLine("Reserved 1: " + reader.ReadInt16() + " - position: " + stream.Position);
                    Console.WriteLine("Reserved 2: " + reader.ReadInt16() + " - position: " + stream.Position);
                    Console.WriteLine("Offset (where image data starts): " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("BitmapInfoHeade: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("Width: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("Height: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("Planes: " + reader.ReadInt16() + " - position: " + stream.Position);

                    Console.WriteLine("Bit Count: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("Compression: " + reader.ReadInt16() + " - position: " + stream.Position);

                    Console.WriteLine("Size Image: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("XPelsPerMeter: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("YPelsPerMeter: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.WriteLine("Colours Used: " + reader.ReadInt32() + " - position: " + stream.Position);
                    Console.WriteLine("Colours Important: " + reader.ReadInt32() + " - position: " + stream.Position);

                    Console.ReadLine();
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caugth in BinaryReader: " + e.Message);
                    Console.ReadLine();
                }

                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caugth in FileStream: " + e.Message);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Adds the given array of byte data onto the end of the data in the bitmap file
        /// NOTE: Since bitmap data is written backwards the byte chunks must be sent in chunks starting from the end of the image
        /// (this will make writing the file much faster). The chunks are still in normal order so will have to be reversed.
        /// </summary>
        /// <param name="imageData"></param>
        public void AddByteData(byte[] imageData)
        {
            //Check current section
            CheckSection();

            heightThisSection += (int)((double)(imageData.Length / 3) / (double)width);

            string fileToAddTo = "";

            if (sectionHeight >= height)
            {
                fileToAddTo = filePath;
            }
            else
            {
                fileToAddTo = filePath.Substring(0, filePath.Length - 4) + "_" + currentSection.ToString() + ".bmp";
            }            

            try
            {
                FileStream stream = new FileStream(fileToAddTo, FileMode.Open);

                try
                {
                    BinaryWriter writer = new BinaryWriter(stream);

                    writer.Seek(currentDataPosition, SeekOrigin.Begin);

                    byte[] reversedData = ReverseData(imageData);

                    writer.Write(reversedData);

                    if (reversedData != null)
                        reversedData = null;

                    currentDataPosition += imageData.Length;

                    writer.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caugth in BinaryWriter: " + e.Message);
                    Console.ReadLine();
                }

                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caugth in FileStream: " + e.Message);
                Console.ReadLine();
            }

            if (imageData != null)
                imageData = null;
        }

        private void CheckSection()
        {
            if(currentSection == totalSections) //on last section
            {
                if(heightThisSection >= lastSectionHeight) //Next section
                {
                    currentSection--;
                    currentDataPosition = 54;
                    heightThisSection = 0;
                }
            }
            else
            {
                if(heightThisSection >=  sectionHeight) //Next section
                {
                    currentSection--;
                    currentDataPosition = 54;
                    heightThisSection = 0;
                }
            }

            
        }

        private byte[] ReverseData(byte[] data)
        {
            byte[] bitmapData = null;

            using (Bitmap imageToReverse = new Bitmap(width, data.Length / width / 3, PixelFormat.Format24bppRgb))
            {
                BitmapData bmpData = imageToReverse.LockBits(new Rectangle(0, 0, imageToReverse.Width, imageToReverse.Height), ImageLockMode.WriteOnly, imageToReverse.PixelFormat);

                Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

                imageToReverse.UnlockBits(bmpData);

                using (MemoryStream ms = new MemoryStream())
                {
                    imageToReverse.Save(ms, ImageFormat.Bmp);
                    bitmapData = ms.ToArray();
                }
            }

            byte[] reversedData = new byte[data.Length];

            for (int i = 0; i < reversedData.Length; i++)
            {
                reversedData[i] = bitmapData[i + 54];
            }

            if (bitmapData != null)
                bitmapData = null;

            if (data != null)
                data = null;

            return reversedData;
        }

        # region Get methods

        public int GetBMPDataSize()
        {
            throw new NotImplementedException();
        }

        public bool GetIsFileComplete()
        {
            BinaryReader reader;

            int size = 0, streamSize = 0;

            try
            {
                FileStream stream = new FileStream(filePath, FileMode.Open);

                streamSize = (int)stream.Length;

                try
                {
                    reader = new BinaryReader(stream);

                    reader.ReadChar();
                    reader.ReadChar();

                    size = reader.ReadInt32();

                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caugth in BinaryReader: " + e.Message);
                    Console.ReadLine();
                }

                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caugth in FileStream: " + e.Message);
                Console.ReadLine();
            }

            if (size == streamSize)
                return true;
            else
                return false;
        }

        public int GetImageWidth()
        {
            return width;
        }

        public int GetImageHeight()
        {
            return height;
        }

        # endregion
    }

}
