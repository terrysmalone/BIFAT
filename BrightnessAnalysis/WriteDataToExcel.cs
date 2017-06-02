using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;

namespace BrightnessAnalysis
{
    /// <summary>
    /// A class which writes depth/brightness data to an excel file
    /// </summary>
    public class WriteDataToExcel
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
        
        private Application app = null;
        private Workbook workbook = null;
        private Worksheet worksheet = null;
        private Worksheet splitWorksheet = null;

        public WriteDataToExcel(string fileName, List<int> data)
        {
            this.fileName = fileName;
            this.data = data;
        }

        public WriteDataToExcel(string fileName, List<int> data, int sampleRate)
        {
            this.fileName = fileName;
            this.data = data;

            this.sampleRate = sampleRate;
        }

        public WriteDataToExcel(string fileName, List<int> data, List<int> rData, List<int> gData,List<int> bData)
        {
            this.fileName = fileName;
            this.data = data;

            this.rData = rData;
            this.gData = gData;
            this.bData = bData;

            addRGBData = true;
        }

        public WriteDataToExcel(string fileName, List<int> data, List<int> rData, List<int> gData, List<int> bData, int sampleRate)
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
            app = new Microsoft.Office.Interop.Excel.Application();
            workbook = app.Workbooks.Add();

            WriteDataToWorksheet();

            workbook.SaveAs(fileName);
            workbook.Close();
        }

        private void WriteDataToWorksheet()
        {
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
            worksheet.Name = "Overall Brightness";

            //addCell(0, 0, "Depth (mm)");
            //addCell(0, 1, "Brightness (0-255)");

            //int row = 1;

            List<Tuple<int, int>> dataToWrite = new List<Tuple<int, int>>();

            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] != -1)
                {
                    dataToWrite.Add(Tuple.Create((startDepth + (i * sampleRate)), data[i]));

                    //row++;
                }
            }

            cellData = new int[dataToWrite.Count, 2];

            for (int i = 0; i < dataToWrite.Count; i++)
            {
                addCell(i, 0, (dataToWrite[i].Item1));

                addCell(i, 1, dataToWrite[i].Item2);
            }

            worksheet.Range["A1"].Value = "Depth (mm)";
            worksheet.Range["B1"].Value = "Brightness (0-255)";

            if (cellData.Length > 0)
            {
                Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[2, 1];
                range = range.get_Resize(cellData.GetLength(0), cellData.GetLength(1));
                // Assign the 2-d array to the Excel Range

                range.set_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault, cellData);
            }

            if (addRGBData == true)
            {
                splitWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[2];
                splitWorksheet.Name = "RGB Brightnesses";

                List<int> depthDataToWrite = new List<int>();
                List<int> rDataToWrite = new List<int>();
                List<int> gDataToWrite = new List<int>();
                List<int> bDataToWrite = new List<int>();

                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i] != -1)
                    {
                        depthDataToWrite.Add((startDepth + (i * sampleRate)));

                        rDataToWrite.Add(rData[i]);
                        gDataToWrite.Add(gData[i]);
                        bDataToWrite.Add(bData[i]);

                        //row++;
                    }
                }

                cellData = new int[depthDataToWrite.Count, 4];

                for (int i = 0; i < depthDataToWrite.Count; i++)
                {
                    addCell(i, 0, (depthDataToWrite[i]));

                    addCell(i, 1, rDataToWrite[i]);
                    addCell(i, 2, gDataToWrite[i]);
                    addCell(i, 3, bDataToWrite[i]);
                }

                splitWorksheet.Range["A1"].Value = "Depth (mm)";
                splitWorksheet.Range["B1"].Value = "R Value (0-255)";
                splitWorksheet.Range["C1"].Value = "G Value (0-255)";
                splitWorksheet.Range["D1"].Value = "B Value (0-255)";


                if (cellData.Length > 0)
                {
                    Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)splitWorksheet.Cells[2, 1];
                    range = range.get_Resize(cellData.GetLength(0), cellData.GetLength(1));
                    // Assign the 2-d array to the Excel Range

                    range.set_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault, cellData);
                }
            }
        }

        private void addCell(int row, int col, int data)
        {
            cellData[row, col] = data;
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
