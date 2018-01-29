using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using BoreholeFeatures;

namespace DrawEdges
{
    /// <summary>
    /// Draws a Bitmap where layers appera as Black and non-layers as white
    /// </summary>
    public class DrawBinaryLayersImage
    {
        private Bitmap drawnImage;

        private List<Layer> layers;
        private int imageWidth;
        private int imageHeight;

        # region properties

        public Bitmap DrawnImage
        {
            get
            {
                return drawnImage;
            }
        }

        #endregion

        public DrawBinaryLayersImage(List<Layer> layers, int imageWidth, int imageHeight)
        {
            this.layers = layers;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;

            drawnImage = new Bitmap(imageWidth, imageHeight, PixelFormat.Format24bppRgb);

            for (int y = 0; y < imageHeight; y++)
            {
                for (int x = 0; x < imageWidth; x++)
                {
                    drawnImage.SetPixel(x, y, Color.White);
                }
            }
            DrawBinaryImage();
        }

        private void DrawBinaryImage()
        {
            Point[] topPoints, bottomPoints;
            
            for (int i = 0; i < layers.Count; i++)
            {
                Layer currentLayer = layers[i];

                //Top sine
                topPoints = currentLayer.GetTopEdgePoints().ToArray();
                
                //Bottom sine
                bottomPoints = currentLayer.GetBottomEdgePoints().ToArray();
                
                //DrawFilling
                for (int col = 0; col < topPoints.Length; col++)
                {
                    int xPoint = topPoints[col].X;

                    for (int row=topPoints[col].Y; row <= bottomPoints[col].Y; row++)
                    {
                        int yPoint = row;

                        drawnImage.SetPixel(xPoint, yPoint, Color.Black);
                    }
                }
            }
        }
    }
}
