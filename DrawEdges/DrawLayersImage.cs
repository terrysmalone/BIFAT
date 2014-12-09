using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BoreholeFeatures;
using EdgeFitting;

namespace DrawEdges
{
    /// <summary>
    /// Draws a List of Layers over a given image
    /// </summary>
    public class DrawLayersImage
    {
        private Bitmap imageToDrawOver, drawnImage;
        private List<Layer> layersToDraw;

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
        /// <param name="layersToDraw">The Layers to draw</param>
        public DrawLayersImage(Bitmap imageToDrawOver, List<Layer> layersToDraw)
        {
            this.imageToDrawOver = imageToDrawOver;
            this.layersToDraw = layersToDraw;

            drawnImage = new Bitmap(imageToDrawOver.Width, imageToDrawOver.Height, imageToDrawOver.PixelFormat);

            DrawLayers();
        }

        /// <summary>
        /// Draws the given sines
        /// </summary>
        private void DrawLayers()
        {
            Pen pen = new Pen(Color.LawnGreen);

            Color semiTransparentColor = Color.FromArgb(70, pen.Color.R, pen.Color.G, pen.Color.B);
            Pen fillingPen = new Pen(semiTransparentColor, 1);

            drawnImage = (Bitmap)imageToDrawOver.Clone();

            graphics = Graphics.FromImage(drawnImage);
            Point[] topPoints, bottomPoints;

            for (int i = 0; i < layersToDraw.Count; i++)
            {
                Layer currentLayer = layersToDraw[i];

                //Top sine
                topPoints = currentLayer.GetTopEdgePoints().ToArray();
                graphics.DrawLines(pen, topPoints);

                //Bottom sine
                bottomPoints = currentLayer.GetBottomEdgePoints().ToArray();
                graphics.DrawLines(pen, bottomPoints);

                //DrawFilling
                for (int j = 0; j < topPoints.Length; j++)
                {
                    graphics.DrawLine(fillingPen, topPoints[j], bottomPoints[j]);
                }
            }
        }
    }
}