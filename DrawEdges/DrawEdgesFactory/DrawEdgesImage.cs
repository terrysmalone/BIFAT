using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DrawEdges.DrawEdgesFactory
{
    public abstract class DrawEdgesImage
    {
        protected Bitmap drawnImage;
        protected int imageWidth, imageHeight;

        protected Color backgroundColour = Color.Black;
        protected Color edgeColour = Color.White;

        protected bool drawMultiColouredEdges = false;

        protected Color[] multiEdgeColours = new Color[]{Color.White, Color.Red, Color.Yellow, Color.RoyalBlue, Color.LightGreen, Color.DarkMagenta, Color.LightPink, Color.DarkGreen, Color.LightSeaGreen, Color.OrangeRed, Color.Fuchsia};
        protected int currentColor = 0;

        public void drawEdgesImage()
        {
            drawnImage = new Bitmap(imageWidth, imageHeight);

            drawBlankBackground();

            drawEdges();
        }

        public void drawEdgesOverBackgroundImage(Bitmap backgroundImage)
        {
            drawnImage = new Bitmap(backgroundImage.Width, backgroundImage.Height, backgroundImage.PixelFormat);

            drawnImage = (Bitmap)backgroundImage.Clone();

            drawEdges();
        }

        internal abstract void drawEdges();

        protected void drawBlankBackground()
        {
            for (int width = 0; width < imageWidth; width++)
            {
                for (int height = 0; height < imageHeight; height++)
                {
                    drawnImage.SetPixel(width, height, backgroundColour);
                }
            }
        }

        # region Set methods

        /// <summary>
        /// Sets the colour to draw the background when drawing over a plain background
        /// </summary>
        /// <param name="drawOverBackgroundColour">The colour to draw background</param>
        public void setBackgroundColour(Color backgroundColour)
        {
            this.backgroundColour = backgroundColour;
        }

        /// <summary>
        /// Sets the colour to draw the edges when drawing over a plain background
        /// </summary>
        /// <param name="drawOverBackgroundColour">The colour to draw the edges</param>
        public void setEdgeColour(Color edgeColour)
        {
            this.edgeColour = edgeColour;
        }

        public void SetDrawMultiColouredEdges(bool drawMultiColouredEdges)
        {
            this.drawMultiColouredEdges = drawMultiColouredEdges;
        }

        # endregion

        # region Get methods

        /// <summary>
        /// Returns the drawnImage
        /// </summary>
        /// <returns>The edges image</returns>
        public Bitmap getDrawnEdges()
        {
            return drawnImage;
        }

        # endregion
    }

}
