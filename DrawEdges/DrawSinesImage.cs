using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EdgeFitting;

namespace DrawEdges
{
    /// <summary>
    /// Class which draws a List of sines over a given image
    /// 
    /// Author - Terry Malone (trm8@aber.ac.uk)
    /// Version 1.1 Refactored
    /// </summary>
    public class DrawSinesImage
    {
        private Bitmap imageToDrawOver, drawnImage;
        private List<Sine> sinesToDraw;

        private Graphics graphics;

        public Bitmap DrawnImage
        {
            get
            {
                return drawnImage;
            }
        }

        /// <summary>
        /// Constructor method
        /// </summary>
        /// <param name="imageToDrawOver">The image to draw the sine waves on</param>
        /// <param name="sinesToDraw">The sine waves to draw</param>
        public DrawSinesImage(Bitmap imageToDrawOver, List<Sine> sinesToDraw)
        {
            this.imageToDrawOver = imageToDrawOver;
            this.sinesToDraw = sinesToDraw;

            drawnImage = new Bitmap(imageToDrawOver.Width, imageToDrawOver.Height, imageToDrawOver.PixelFormat);

            drawSines();
        }

        /// <summary>
        /// Draws the given sines
        /// </summary>
        private void drawSines()
        {
            Pen pen = new Pen(Color.Red, 3);

            drawnImage = (Bitmap)imageToDrawOver.Clone();

            graphics = Graphics.FromImage(drawnImage);
            Point[] points;

            for (int i = 0; i < sinesToDraw.Count; i++)
            {
                points = sinesToDraw[i].Points.ToArray();
                graphics.DrawLines(pen, points);

                graphics.DrawRectangle(new Pen(Color.Red, 1), new Rectangle(new Point(points[points.Length-1].X, points[points.Length-1].Y-1), new Size(1, 3)));
            }
        }
    }
}
