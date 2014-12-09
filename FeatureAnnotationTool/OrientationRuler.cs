using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FeatureAnnotationTool
{
    public class OrientationRuler
    {
        private int rulerWidth, imageWidth;

        private int orientationRulerWidth, orientationRulerHeight;
        private int start, end;

        //private int rotation; //0=N, 90=E, 180=S, 270=W

        private Bitmap orientationRulerImage;
        private Graphics orientationRulerGraphics;

        private Font rulerFont;
        private SolidBrush rulerBrush;


        public OrientationRuler(int rulerWidth, int imageWidth, int rotation)
        {
            this.rulerWidth = rulerWidth;   //Not orientation ruler
            this.imageWidth = imageWidth;

            InitialiseRulerGraphics();

            start = rulerWidth;
            end = start + imageWidth;

            double oneDegree = (double)imageWidth / 360.0;

            double smallStep = oneDegree * 10;

            for (int i = 1; i < 36; i++)
            {
                //int position = (int)Math.Round(((double)imageWidth / 36.0) * (double)i);

                int position = (int)Math.Round(smallStep * (double)i);
                DrawLine(position + start);
            }
            
            double largeStep = oneDegree * 90;

            int currentPosition = start;

            string[] letters = new string[5];

            if(rotation == 0)
                letters = new string[]{"N","E","S","W","N"};
            else if (rotation == 90)
                letters = new string[] { "E", "S", "W", "N", "E" };
            else if (rotation == 180)
                letters = new string[] {"S", "W", "N", "E","S" };
            else if (rotation == 270)
                letters = new string[] {"W", "N", "E", "S", "W"};



            DrawLargeLine(currentPosition, letters[0]);
            currentPosition += (int)largeStep;
            DrawLargeLine(currentPosition, letters[1]);
            currentPosition += (int)largeStep;
            DrawLargeLine(currentPosition, letters[2]);
            currentPosition += (int)largeStep;
            DrawLargeLine(currentPosition, letters[3]);
            currentPosition += (int)largeStep;
            DrawLargeLine(currentPosition, letters[4]);
            
        }

        private void DrawLine(int position)
        {
            for (int i = orientationRulerHeight - 6; i < orientationRulerHeight; i++)
                orientationRulerImage.SetPixel(position, i, Color.Black);
        }

        private void DrawLargeLine(int position, string letter)
        {
            for (int i = orientationRulerHeight - 10; i < orientationRulerHeight; i++)
                orientationRulerImage.SetPixel(position, i, Color.Black);

            orientationRulerGraphics.DrawString(letter, rulerFont, rulerBrush, position - 4, orientationRulerHeight-30);
        }

        private void InitialiseRulerGraphics()
        {
            orientationRulerWidth = rulerWidth + imageWidth + 19;       //19 is the width of the scrollbar

            orientationRulerHeight = 46;

            orientationRulerImage = new Bitmap(orientationRulerWidth, orientationRulerHeight);

            orientationRulerGraphics = Graphics.FromImage(orientationRulerImage);

            rulerFont = SystemFonts.MessageBoxFont;
            rulerBrush = new SolidBrush(Color.Black);
        }

        public Bitmap GetOrientationRulerImage()
        {
            return orientationRulerImage;
        }
    }
}
