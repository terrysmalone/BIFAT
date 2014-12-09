using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace FeatureAnnotationTool
{
    public class Ruler
    {
        private int rulerStartDepth, rulerEndDepth, rulerHeight;
        private int depthResolution;

        private Graphics rulerGraphics;
        private Bitmap rulerImage;
        private int rulerWidth = 60;

        private Font rulerFont;
        private SolidBrush rulerBrush;

        public Ruler(int rulerStartDepth, int rulerEndDepth, int depthResolution)
        {
            this.rulerStartDepth = rulerStartDepth;
            this.rulerEndDepth = rulerEndDepth;
            this.depthResolution = depthResolution;

            InitialiseRulerGraphics();

            //for (int rulerPos = 0; rulerPos < rulerEndDepth-rulerStartDepth; rulerPos++)
            for (int rulerPos = 0; rulerPos < rulerHeight; rulerPos++)
            {
                //if ((rulerStartDepth + (rulerPos * depthResolution)) % 1000 == 0)
                if ((rulerStartDepth + (rulerPos * depthResolution)) % 1000 >= 0 && (rulerStartDepth + (rulerPos * depthResolution)) % 1000 < depthResolution)
                {                    
                    drawRulerMetreLine(rulerPos);
                }
                //else if ((rulerStartDepth + (rulerPos * depthResolution)) % 100 == 0)
                else if ((rulerStartDepth + (rulerPos * depthResolution)) % 100 >= 0 && (rulerStartDepth + (rulerPos * depthResolution)) % 100 < depthResolution)
                {
                    drawTenCentimetreLine(rulerPos);

                }
                //else if ((rulerStartDepth + (rulerPos * depthResolution)) % 10 == 0)
                else if ((rulerStartDepth + (rulerPos * depthResolution)) % 10 >= 0 && (rulerStartDepth + (rulerPos * depthResolution)) % 10 < depthResolution)
                {
                    drawRulerCentimetreLine(rulerPos);
                }
            }
        }

        private void InitialiseRulerGraphics()
        {            
            rulerHeight = ((rulerEndDepth - rulerStartDepth) / depthResolution)+1;
            //rulerHeight = (rulerEndDepth - rulerStartDepth) + 1;

            rulerImage = new Bitmap(rulerWidth, rulerHeight, PixelFormat.Format24bppRgb);
            //rulerImage = new Bitmap(rulerWidth, rulerHeight);
            
            rulerGraphics = Graphics.FromImage(rulerImage);

            //set background color
            rulerGraphics.Clear(System.Drawing.Color.White);

            rulerFont = SystemFonts.MessageBoxFont;
            rulerBrush = new SolidBrush(Color.Black);
        }

        /// <summary>
        /// Draws the ruler metre line
        /// </summary>
        /// <param name="rulerPos">The position along the ruler</param>
        private void drawRulerMetreLine(int rulerPos)
        {
            for (int xPos = rulerWidth - 20; xPos < rulerWidth; xPos++)
                rulerImage.SetPixel(xPos, rulerPos, Color.Black);

            float value = rulerStartDepth + (rulerPos * depthResolution);
            float difference = (rulerStartDepth + (rulerPos * depthResolution)) % 1000;

            //string text = System.Convert.ToString((float)((rulerStartDepth + (rulerPos * depthResolution)) / 1000.0f));
            string text = System.Convert.ToString((value - difference) / 1000.0f);
            rulerGraphics.DrawString(text, rulerFont, rulerBrush, 5, rulerPos - 8);
        }

        /// <summary>
        /// Draws the ten centimetre lines
        /// </summary>
        /// <param name="rulerPos">The position along the ruler</param>
        private void drawTenCentimetreLine(int rulerPos)
        {
            for (int xPos = rulerWidth - 10; xPos < rulerWidth; xPos++)
                rulerImage.SetPixel(xPos, rulerPos, Color.Black);

            float value = rulerStartDepth + (rulerPos * depthResolution);
            float difference = (rulerStartDepth + (rulerPos * depthResolution)) % 100;

            //string text = System.Convert.ToString((float)((rulerStartDepth + (rulerPos * depthResolution)) / 1000.0f));
            string text = System.Convert.ToString((value - difference) / 1000.0f);
            rulerGraphics.DrawString(text, rulerFont, rulerBrush, 5, rulerPos - 8);
        }

        /// <summary>
        /// Draws the centimetre lines
        /// </summary>
        /// <param name="rulerPos">The position along the ruler</param>
        private void drawRulerCentimetreLine(int rulerPos)
        {
            for (int xPos = rulerWidth - 5; xPos < rulerWidth; xPos++)
                rulerImage.SetPixel(xPos, rulerPos, Color.Black);
        }

        public Bitmap GetRulerImage()
        {
            return rulerImage;
        }

        public int GetRulerHeight()
        {
            return rulerHeight;
        }

        public int GetRulerWidth()
        {
            return rulerWidth;
        }
    }
}
