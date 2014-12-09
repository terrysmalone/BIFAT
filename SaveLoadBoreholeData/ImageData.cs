using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ImageTiler;

namespace SaveLoadBoreholeData
{
    /// <summary>
    /// Writes Bitmap data to a file
    ///
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.0
    /// </summary>
    public class ImageData
    {
        private FileTilerSelector fileTilerFactory;
        private FileTiler tiler;

        private string sourceFile, destinationFile;
        private string projectLocation;
        private string fileType;

        public ImageData(string projectLocation, string fileType)
        {
            this.fileType = fileType;
            this.projectLocation = projectLocation;

            string[] files = Directory.GetFiles(projectLocation + "\\source");

            sourceFile = files[0];
            destinationFile = projectLocation + "\\data\\imageData";
        }

        public void Write()
        {
            fileTilerFactory = new FileTilerSelector(fileType);
            tiler = fileTilerFactory.setUpTiler(sourceFile, 10000);

            tiler.GoToFirstSection();

            using (FileStream stream = new FileStream(destinationFile, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(tiler.BoreholeWidth);
                    writer.Write(tiler.BoreholeHeight);

                    do
                    {
                        writer.Write(tiler.GetCurrentSectionAsBytes());

                    } while (tiler.GoToNextSection());
                }
            }

            if (fileType == "Bitmap")
            {
                if (Directory.Exists(projectLocation + "\\source"))
                    Directory.Delete(projectLocation + "\\source", true);
            }
        }
    }
}
