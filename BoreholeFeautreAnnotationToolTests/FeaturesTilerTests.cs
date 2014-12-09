using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageTiler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class FeaturesTilerTest
    {
        private string testRootFolder = AppDomain.CurrentDomain.BaseDirectory;

        private FileTilerSelector fileTilerFactory = new FileTilerSelector("FeaturesFile");
        private FileTiler tiler;

        private int BOREHOLE_HEIGHT = 17917;
        private int BOREHOLE_WIDTH = 720;


        [TestMethod]
        public void TestBoreholeWidth()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            Assert.IsTrue(tiler.BoreholeWidth == BOREHOLE_WIDTH, "Borehole width should be 720.  It is " + tiler.BoreholeWidth);
        }

        [TestMethod]
        public void TestBoreholeHeight()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            Assert.IsTrue(tiler.BoreholeHeight == BOREHOLE_HEIGHT, "Borehole height should be 17917.  It is " + tiler.BoreholeHeight);
        }

        [TestMethod]
        public void TestSectionStartHeight()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.SectionStartHeight == 0, "SectionStartHeight should be 0.  It is " + tiler.SectionStartHeight);

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.SectionStartHeight == 10000, "SectionStartHeight should be 10000.  It is " + tiler.SectionStartHeight);

            tiler.GoToPreviousSection();
            Assert.IsTrue(tiler.SectionStartHeight == 0, "SectionStartHeight should be 0.  It is " + tiler.SectionStartHeight);


            tiler.GoToNextSection();
            Assert.IsTrue(tiler.SectionStartHeight == 10000, "SectionStartHeight should be 10000.  It is " + tiler.SectionStartHeight);
        }

        [TestMethod]
        public void TestSectionEndHeight()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.SectionEndHeight == 9999, "SectionEndHeight should be 9999.  It is " + tiler.SectionEndHeight);

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.SectionEndHeight == BOREHOLE_HEIGHT - 1, "SectionEndHeight should be 17916.  It is " + tiler.SectionEndHeight);

            tiler.GoToPreviousSection();
            Assert.IsTrue(tiler.SectionEndHeight == 9999, "SectionEndHeight should be 9999.  It is " + tiler.SectionEndHeight);

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.SectionEndHeight == BOREHOLE_HEIGHT - 1, "Borehole width should be 17916.  It is " + tiler.SectionEndHeight);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.SectionEndHeight == 9999, "Borehole width should be 9999.  It is " + tiler.SectionEndHeight);

        }

        [TestMethod]
        public void TestGoToFirstSection()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "Current section should be 1.  It is " + tiler.CurrentSectionNumber);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);
        }

        [TestMethod]
        public void TestNext()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

            Assert.IsTrue(tiler.GoToNextSection() == true, "next should return true");
            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "Current section should be 1.  It is " + tiler.CurrentSectionNumber);
            Assert.IsTrue(tiler.GoToNextSection() == false, "next should return false");
            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "Current section should be 1.  It is " + tiler.CurrentSectionNumber);


            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

            Assert.IsTrue(tiler.GoToNextSection() == true, "next should return true");
            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "Current section should be 1.  It is " + tiler.CurrentSectionNumber);

        }

        [TestMethod]
        public void TestGoToPreviousSection()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

            Assert.IsTrue(tiler.GoToPreviousSection() == false, "previous should return false");

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "Current section should be 1.  It is " + tiler.CurrentSectionNumber);

            Assert.IsTrue(tiler.GoToPreviousSection() == true, "previous should return true");
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

            Assert.IsTrue(tiler.GoToPreviousSection() == false, "previous should return false");
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "Current section should be 0.  It is " + tiler.CurrentSectionNumber);

        }

        [TestMethod]
        public void TestCurrentSectionHeight()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Assert.IsTrue(tiler.CurrentSectionHeight == 10000, "CurrentSectionheight should be 10,000.  It is " + tiler.CurrentSectionHeight);

            tiler.GoToNextSection();
            Assert.IsTrue(tiler.CurrentSectionHeight == 7917, "CurrentSectionheight should be 7,917.  It is " + tiler.CurrentSectionHeight);

            tiler.GoToPreviousSection();
            Assert.IsTrue(tiler.CurrentSectionHeight == 10000, "CurrentSectionheight should be 10,000.  It is " + tiler.CurrentSectionHeight);

        }

        [TestMethod]
        public void TestGoToSection()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToSection(1);

            Assert.IsTrue(tiler.CurrentSectionNumber == 1, "CurrentSectionNumber should return 1. It returned " + tiler.CurrentSectionNumber);

            tiler.GoToSection(0);
            Assert.IsTrue(tiler.CurrentSectionNumber == 0, "CurrentSectionNumber should return 0. It returned " + tiler.CurrentSectionNumber);
        }

        [TestMethod]
        public void TestGetCurrentSectionAsBitmap()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            Bitmap currentBitmap = tiler.GetCurrentSectionAsBitmap();
            Bitmap idealBitmap = new Bitmap(testRootFolder + "\\idealImages\\TsanBH1R1c - 1.BMP");

            Assert.IsTrue(compareImages(currentBitmap, idealBitmap) == true, "The two bitmaps should be the same");

            tiler.GoToNextSection();
            currentBitmap = tiler.GetCurrentSectionAsBitmap();
            idealBitmap = new Bitmap(testRootFolder + "\\idealImages\\TsanBH1R1c - 2.BMP");

            Assert.IsTrue(compareImages(currentBitmap, idealBitmap) == true, "The two bitmaps should be the same");
        }

        [TestMethod]
        public void TestGetCurrentSectionAsBytes()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            tiler.GoToFirstSection();
            byte[] currentBytes = tiler.GetCurrentSectionAsBytes();
            byte[] idealBytes = getBitmapAsBytes(new Bitmap(testRootFolder + "\\idealImages\\TsanBH1R1c - 1.BMP"));

            Assert.IsTrue(compareImages(currentBytes, idealBytes) == true, "The two bitmaps should be the same");

            tiler.GoToNextSection();
            currentBytes = tiler.GetCurrentSectionAsBytes();
            idealBytes = getBitmapAsBytes(new Bitmap(testRootFolder + "\\idealImages\\TsanBH1R1c - 2.BMP"));

            Assert.IsTrue(compareImages(currentBytes, idealBytes) == true, "The two bitmaps should be the same");
        }

        [TestMethod]
        public void TestGetWholeBoreholeImage()
        {
            string imageFile = testRootFolder + "\\idealImages\\TsanBH1R1c";
            tiler = fileTilerFactory.setUpTiler(imageFile, 10000);

            //tiler.GoToFirstSection();
            Bitmap currentBitmap = tiler.GetWholeBoreholeImage();
            Bitmap idealBitmap = new Bitmap(testRootFolder + "\\idealImages\\TsanBH1R1c.BMP");

            Assert.IsTrue(compareImages(currentBitmap, idealBitmap) == true, "The two bitmaps should be the same");
        }

        /// <summary>
        /// Compares two bitmaps.  Since comparing every pixel takes too long a 
        /// number of pixels are chosen at random and are compared
        /// </summary>
        /// <param name="firstImage"></param>
        /// <param name="secondImage"></param>
        /// <returns></returns>
        private bool compareImages(Bitmap firstImage, Bitmap secondImage)
        {
            bool flag = true;
            string firstPixel;
            string secondPixel;

            if (firstImage.Width == secondImage.Width && firstImage.Height == secondImage.Height)
            {
                //for (int i = 0; i < firstImage.Width; i++)
                //{
                //    for (int j = 0; j < firstImage.Height; j++)
                //    {
                Random rand = new Random();
                int xPos;
                int yPos;

                for (int i = 0; i < 1000; i++)
                {
                    xPos = rand.Next(firstImage.Width);
                    yPos = rand.Next(firstImage.Height);

                    firstPixel = firstImage.GetPixel(xPos, yPos).ToString();
                    secondPixel = secondImage.GetPixel(xPos, yPos).ToString();

                    //firstPixel = firstImage.GetPixel(i, j).ToString();
                    //secondPixel = secondImage.GetPixel(i, j).ToString();

                    if (firstPixel != secondPixel)
                    {
                        flag = false;
                        break;
                    }
                }
                //    }
                //}

                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Compares two byte arrays.  Since comparing every pixel takes too long a 
        /// number of pixels are chosen at random and are compared
        /// </summary>
        /// <param name="firstImage"></param>
        /// <param name="secondImage"></param>
        /// <returns></returns>
        private bool compareImages(byte[] firstImage, byte[] secondImage)
        {
            bool flag = true;
            string firstPixel;
            string secondPixel;

            if (firstImage.Length == secondImage.Length)
            {
                Random rand = new Random();
                int pos;

                for (int i = 0; i < 1000; i++)
                {
                    pos = rand.Next(firstImage.Length);

                    firstPixel = firstImage[pos].ToString();
                    secondPixel = secondImage[pos].ToString();

                    //firstPixel = firstImage.GetPixel(i, j).ToString();
                    //secondPixel = secondImage.GetPixel(i, j).ToString();
                    if (firstPixel != secondPixel)
                    {
                        flag = false;
                        break;
                    }
                }
                //    }
                //}

                if (flag == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public byte[] getBitmapAsBytes(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            image.Save(ms, ImageFormat.Bmp);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();

            byte[] bmpData = new byte[bmpBytes.Length - 54];

            for (int i = 0; i < bmpData.Length; i++)
            {
                bmpData[i] = bmpBytes[i + 54];
            }

            ms.Close();

            //image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmpData;
        }
    }
}

