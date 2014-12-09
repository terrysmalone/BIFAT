using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEdges;
using EdgeFitting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoreholeFeautreAnnotationToolTests
{
    [TestClass]
    public class DrawSinesTests
    {
        private int BLANK = 0;
        private int SINE = -65536;

        [TestMethod]
        public void TestDrawSines()
        {
            Bitmap originalBitmap = new Bitmap(360, 100);
            List<Sine> sinesToDraw = new List<Sine>();

            Sine sine1 = new Sine(10, 50, 5, 360);
            sinesToDraw.Add(sine1);

            Sine sine2 = new Sine(46, 100, 4, 360);
            sinesToDraw.Add(sine2);



            DrawSinesImage drawSines = new DrawSinesImage(originalBitmap, sinesToDraw);
            Bitmap afterImage = drawSines.DrawnImage;
            
            for (int i = 0; i < sine1.Points.Count; i++)
            {
                int x = i;
                int y = sine1.GetY(x);

                Assert.IsTrue(afterImage.GetPixel(x, y - 3).ToArgb() == BLANK, "Pixel " + x + ", " + (y - 3) + " should be 0.  It is " + afterImage.GetPixel(x, y + -3).ToArgb());
                Assert.IsTrue(afterImage.GetPixel(x, y).ToArgb() == SINE, "Pixel " + x + ", " + y + " should be " + SINE + ".  It is " + afterImage.GetPixel(x, y).ToArgb());
                Assert.IsTrue(afterImage.GetPixel(x, y + 3).ToArgb() == BLANK, "Pixel " + x + ", " + (y + 3) + " should be 0.  It is " + afterImage.GetPixel(x, y + 3).ToArgb());
            }

            for (int i = 0; i < sine2.Points.Count; i++)
            {
                int x = i;
                int y = sine2.GetY(x);

                Assert.IsTrue(afterImage.GetPixel(x, y - 3).ToArgb() == BLANK, "Pixel " + x + ", " + (y - 3) + " should be 0.  It is " + afterImage.GetPixel(x, y + -3).ToArgb());
                Assert.IsTrue(afterImage.GetPixel(x, y).ToArgb() == SINE, "Pixel " + x + ", " + y + " should be " + SINE + ".  It is " + afterImage.GetPixel(x, y).ToArgb());
                Assert.IsTrue(afterImage.GetPixel(x, y + 3).ToArgb() == BLANK, "Pixel " + x + ", " + (y + 3) + " should be 0.  It is " + afterImage.GetPixel(x, y + 3).ToArgb());
            }
        }
    }
}
