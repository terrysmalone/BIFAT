using System;
using RGReferencedData;

namespace ImageTiler
{
    /// <summary>
    /// Allows the selection of different types of Tilers
    /// </summary>
    public class FileTilerSelector
    {
        protected String type;

        public FileTilerSelector(String type)
        {
            this.type = type;
        }

        public FileTiler setUpTiler(string fileName, int sectionHeight)
        {
            if (type.Equals("Bitmap"))
                return new BitmapTiler(fileName, sectionHeight);
            else if (type.Equals("OTV"))
                return new OTVImageTiler(fileName, sectionHeight);
            else if (type.Equals("FeaturesFile"))
                return new FeaturesImageTiler(fileName, sectionHeight);
            else
                return new FeaturesImageTiler(fileName, sectionHeight);
        }

        public FileTiler setUpTiler(ReferencedChannel optvChannel, int sectionHeight)
        {
            //if (type.Equals("Channel"))
            return new ChannelTiler(optvChannel, sectionHeight);
        }
    }

}
