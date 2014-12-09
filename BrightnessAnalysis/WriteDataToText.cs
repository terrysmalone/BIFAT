using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BrightnessAnalysis
{
    /// <summary>
    /// A class which writes depth/brightness data to a txt
    /// </summary>
    public class WriteDataToText
    {
        private string fileName;
        private List<int> data;
        private int sampleRate = 1;
        private int startDepth = 0;
        private int depthResolution;

        private int[,] cellData;

        public bool addRGBData = false;
        private List<int> rData;
        private List<int> gData;
        private List<int> bData;

        public WriteDataToText(string fileName, List<int> data)
        {
            this.fileName = fileName;
            this.data = data;
        }

        public WriteDataToText(string fileName, List<int> data, int sampleRate)
        {
            this.fileName = fileName;
            this.data = data;
            
            this.sampleRate = sampleRate;
        }

        public WriteDataToText(string fileName, List<int> data, List<int> rData, List<int> gData, List<int> bData)
        {
            this.fileName = fileName;
            this.data = data;

            this.rData = rData;
            this.gData = gData;
            this.bData = bData;

            addRGBData = true;
        }

        public WriteDataToText(string fileName, List<int> data, List<int> rData, List<int> gData,List<int> bData, int sampleRate)
        {
            this.fileName = fileName;
            this.data = data;

            this.rData = rData;
            this.gData = gData;
            this.bData = bData;

            this.sampleRate = sampleRate;
            
            addRGBData = true;
        }

        public void write()
        {
            try
            {
                StreamWriter file = new StreamWriter(fileName);

                if (addRGBData == false)
                {
                    file.WriteLine("#Depth(mm) Brightness(0-255)");

                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] != -1)
                        {
                            file.WriteLine((startDepth + (i * sampleRate)) + " " + data[i]);
                        }
                    }
                }
                else
                {
                    file.WriteLine("#Depth(mm) Brightness(0-255) R Value(0-255) G Value(0-255) B Value(0-255)");

                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] != -1)
                        {
                            file.WriteLine((startDepth + (i * sampleRate)) + " " + data[i] + " " + rData[i]  + " " + gData[i]  + " " + bData[i]);
                        }
                    }
                }

                file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error writing text file: " + e.Message);
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public void setDepthResolution(int depthResolution)
        {
            this.depthResolution = depthResolution;
        }

        public void setStartDepth(int startDepth)
        {
            this.startDepth = startDepth;
        }
    }

}
