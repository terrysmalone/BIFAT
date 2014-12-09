using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageTiler
{
    public abstract class FileTiler
    {
        protected long imageStartPosition;
        protected string fileName;

        protected int boreholeWidth, boreholeHeight;

        protected int currentSectionNumber = 0;

        protected int currentSectionHeight, originalHeight;
        protected int sectionSize, originalSectionSize;

        protected long sectionStart, sectionEnd;

        protected byte[] currentSectionAsBytes;
        protected Bitmap currentSectionAsBitmap;
        protected int sectionStartHeight;
        protected int sectionEndHeight;
        protected int totalSize;

        #region Properties

        public int CurrentSectionNumber
        {
            get { return currentSectionNumber; }
        }

        public int BoreholeHeight
        {
            get { return boreholeHeight; }
        }

        public int BoreholeWidth
        {
            get { return boreholeWidth; }
        }

        /// <summary>
        /// The start height of the current section
        /// </summary>
        public int SectionStartHeight
        {
            get { return sectionStartHeight; }
        }

        /// <summary>
        /// The end height of the current section
        /// </summary>
        public int SectionEndHeight
        {
            get { return sectionEndHeight; }
        }

        /// <summary>
        /// The end height of the current section
        /// </summary>
        public int CurrentSectionHeight
        {
            get { return currentSectionHeight; }
        }

        public int DefaultSectionHeight
        {
            get { return originalHeight; }
        }

        # endregion

        protected abstract void CalculateImageProperties();

        protected abstract void LoadSection();

        public abstract Bitmap GetSpecificSectionAsBitmap(int startHeight, int endHeight);

        public abstract void LoadSpecificSection(int startheight, int endHeight);

        /// <summary>
        /// Sets the first section as the current one
        /// </summary>
        public void GoToFirstSection()
        {
            currentSectionNumber = 0;
            LoadSection();
        }

        /// <summary>
        /// Sets the last section as the current one
        /// </summary>
        public void GoToLastSection()
        {
            int lastSection = -1;
            bool lastFound = false;

            while (lastFound == false)
            {
                lastSection++;

                if ((lastSection + 1) * originalSectionSize >= totalSize)
                    lastFound = true;
            }

            currentSectionNumber = lastSection;

            LoadSection();
        }

        /// <summary>
        /// Sets a specific section as the current one
        /// </summary>
        /// <param name="currentSection">The section to set as the current section</param>
        public void GoToSection(int currentSection)
        {
            this.currentSectionNumber = currentSection;
            LoadSection();
        }

        /// <summary>
        /// Sets the previous section to the current section if there is one.
        /// </summary>
        /// <returns>True if there is a previous section, false if not</returns>
        public bool GoToPreviousSection()
        {
            if (currentSectionNumber == 0)
                return false;
            else
            {
                currentSectionNumber--;
                LoadSection();
                return true;
            }
        }

        /// <summary>
        /// Sets the next section to the current section if there is one.
        /// </summary>
        /// <returns>True if there is a next section, false if not</returns>
        public bool GoToNextSection()
        {
            //MessageBox.Show("currentSectionNumber: " + currentSectionNumber + "\noriginalSectionSize: " + originalSectionSize + "\ntotalSize: " + totalSize);
            if ((currentSectionNumber + 1) * originalSectionSize >= totalSize)
            {
                //MessageBox.Show("Returned false");
                return false;
            }
            else
            {
                //MessageBox.Show("Returned true");
                currentSectionNumber++;
                LoadSection();
                return true;
            }
        }

        # region get methods

        public bool GetIsPreviousSection()
        {
            if (currentSectionNumber == 0)
                return false;
            else
                return true;
        }

        public bool GetIsNextSection()
        {
            if ((currentSectionNumber + 1) * originalSectionSize >= totalSize)
                return false;
            else
                return true;
        }
          
        /// <summary>
        /// Returns the current section as a bitmap
        /// </summary>
        /// <returns>The current section</returns>
        public abstract Bitmap GetCurrentSectionAsBitmap();

        /// <summary>
        /// Returns the current section as a byte array
        /// </summary>
        /// <returns>The current section</returns>
        public abstract byte[] GetCurrentSectionAsBytes();

        public abstract Bitmap GetWholeBoreholeImage();

        # endregion
    }
}
