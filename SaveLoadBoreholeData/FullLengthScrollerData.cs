using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageTiler;
using System.IO;
using System.Drawing;

namespace SaveLoadBoreholeData
{
    /// <summary>
    /// Writes image data for the full length scroller from the saved image data
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public class FullLengthScrollerData
    {
        private FileTilerSelector fileTilerFactory;
        private FileTiler tiler;

        private string imageDataLocation;
        private string destinationFile;

        private int targetHeight, targetWidth;      
        
        public FullLengthScrollerData(string projectLocation)
        {
            imageDataLocation = projectLocation + "\\data";

            destinationFile = imageDataLocation + "\\scrollImageData";

        }

        public void Write()
        {
            fileTilerFactory = new FileTilerSelector("FeaturesFile");
            tiler = fileTilerFactory.setUpTiler(imageDataLocation + "\\imageData", 10000);

            tiler.GoToFirstSection();

            targetWidth = 40;
            targetHeight = 1200;

            Bitmap fullScrollPreviewImage = new Bitmap(targetWidth, targetHeight);

            int counter = 0;

            
            int boreholeHeight = tiler.BoreholeHeight;


            int smallSectionCurrentPos = 0;
            int currentTop;

            do
            {
                Bitmap sectionImage = tiler.GetCurrentSectionAsBitmap();

                //int smallSectionCurrentPos = (Int32)((double)tiler.SectionStartHeight * ((double)targetHeight / (double)tiler.BoreholeHeight));

                int smallSecHeight = Convert.ToInt32((double)tiler.CurrentSectionHeight * ((double)targetHeight / (double)tiler.BoreholeHeight));

                if (smallSecHeight < 1)
                    smallSecHeight = 1;

                Bitmap smallSectionImage = (Bitmap)sectionImage.GetThumbnailImage(targetWidth, smallSecHeight, null, IntPtr.Zero);

                Graphics g = Graphics.FromImage(fullScrollPreviewImage);

                g.DrawImage(smallSectionImage, 0, smallSectionCurrentPos);

                smallSectionCurrentPos += smallSecHeight;
                //smallSectionImage.Save("sec" + counter + ".bmp");
                counter++;

            } while (tiler.GoToNextSection());

            //Trim rounded excess
            int excess = targetHeight - smallSectionCurrentPos;

            if (excess > 0)
            {
                Console.WriteLine(excess);
                Rectangle srcRect = Rectangle.FromLTRB(0, 0, targetWidth, targetHeight - excess);
                Bitmap trimmedFullScrollPreviewImage = new Bitmap(srcRect.Width, srcRect.Height);
                Rectangle destRect = new Rectangle(0, 0, srcRect.Width, srcRect.Height);
                using (Graphics graphics = Graphics.FromImage(trimmedFullScrollPreviewImage))
                {
                    graphics.DrawImage(fullScrollPreviewImage, destRect, srcRect, GraphicsUnit.Pixel);
                }

                trimmedFullScrollPreviewImage.Save(destinationFile);
            }
            else
            {
                fullScrollPreviewImage.Save(destinationFile);
            }
        }

        public Bitmap GetFullPreviewImage()
        {
            return new Bitmap(destinationFile);
        }
    }
}
