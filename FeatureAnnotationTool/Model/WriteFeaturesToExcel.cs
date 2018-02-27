using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoreholeFeatures;

namespace FeatureAnnotationTool.Model
{
    /// <summary>
    /// Writes features to Excel
    /// </summary>
    class WriteFeaturesToExcel
    {
        private Microsoft.Office.Interop.Excel.Application app = null;
        private Microsoft.Office.Interop.Excel.Workbook workbook = null;
        private Microsoft.Office.Interop.Excel.Worksheet layersWorksheet = null;
        private Microsoft.Office.Interop.Excel.Worksheet clustersWorksheet = null;
        private Microsoft.Office.Interop.Excel.Worksheet inclusionsWorksheet = null;

        private string fileName;

        private List<Layer> layers;
        private List<Cluster> clusters;
        private List<Inclusion> inclusions;

        private string[,] layersCells;
        private string[,] clustersCells;
        private string[,] inclusionsCells;
 
        private List<string> layerPropertiesToInclude;
        private List<string> clusterPropertiesToInclude;
        private List<string> inclusionPropertiesToInclude;

        private int[] layerBrightnesses = null;
        private int[] clusterBrightnesses = null;
        private int[] inclusionBrightnesses = null;

        private int currentWorksheet = 1;

        public WriteFeaturesToExcel(string fileName, List<Layer> layers, List<Cluster> clusters, List<Inclusion> inclusions)
        {
            this.fileName = fileName;
            this.layers = layers;
            this.clusters = clusters;
            this.inclusions = inclusions;

            layersCells = new string[layers.Count+1, 12];
            clustersCells = new string[clusters.Count+1, 9];
            inclusionsCells = new string[inclusions.Count+1, 9];
        }

        public void WriteFile()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            workbook = app.Workbooks.Add();

            if(layers.Count > 0 && layerPropertiesToInclude != null)
                writeLayersWorksheet();

            if (clusters.Count > 0 && clusterPropertiesToInclude != null)
                writeClustersWorksheet();

            if(inclusions.Count > 0 && inclusionPropertiesToInclude != null)
                writeInclusionsWorksheet();

            workbook.SaveAs(fileName);
            workbook.Close();
        }

        private void writeLayersWorksheet()
        {
            layersWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[currentWorksheet];
            currentWorksheet++;
            layersWorksheet.Name = "Layers";
            
            int row = 0;

            //Write column headers
            int currentColumn = 0;

            if (layerPropertiesToInclude.Contains("Start depth (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Start Depth (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("End depth (mm)"))
            {
                addCell(layersCells, row, currentColumn, "End Depth (mm)");
                currentColumn++;
            }

            # region Borehole only properties

            if (layerPropertiesToInclude.Contains("Top sine depth (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Top sine depth (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Top sine azimuth"))
            {
                addCell(layersCells, row, currentColumn, "Top sine azimuth");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Top sine amplitude (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Top sine amplitude (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Bottom sine depth (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Bottom Sine Depth (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Bottom sine azimuth"))
            {
                addCell(layersCells, row, currentColumn, "Bottom sine azimuth");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Bottom sine amplitude (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Bottom sine amplitude (mm)");
                currentColumn++;
            }

            # endregion

            # region Core only properties

            if (layerPropertiesToInclude.Contains("Top edge intercept (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Top edge intercept (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Top edge slope"))
            {
                addCell(layersCells, row, currentColumn, "Top edge slope");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Bottom edge intercept (mm)"))
            {
                addCell(layersCells, row, currentColumn, "Bottom edge intercept (mm)");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Bottom edge slope"))
            {
                addCell(layersCells, row, currentColumn, "Bottom edge slope");
                currentColumn++;
            }

            # endregion

            if (layerPropertiesToInclude.Contains("Group"))
            {
                addCell(layersCells, row, currentColumn, "Group");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Layer type"))
            {
                addCell(layersCells, row, currentColumn, "Layer Type");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Layer description"))
            {
                addCell(layersCells, row, currentColumn, "Layer Description");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Layer quality"))
            {
                addCell(layersCells, row, currentColumn, "Layer Quality");
                currentColumn++;
            }

            if (layerPropertiesToInclude.Contains("Mean layer brightness"))
            {
                addCell(layersCells, row, currentColumn, "Mean layer brightness");
                currentColumn++;
            }

            //Write data
            for (int i = 0; i < layers.Count; i++)
            {
                row++;
                int testnum = layers[i].TopEdgeDepthMm;

                currentColumn = 0;

                if (layerPropertiesToInclude.Contains("Start depth (mm)"))
                {
                    addCell(layersCells, row, currentColumn, System.Convert.ToString((layers[i].StartDepth)));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("End depth (mm)"))
                {
                    addCell(layersCells, row, currentColumn, System.Convert.ToString((layers[i].EndDepth)));
                    currentColumn++;
                }

                # region Borehole only properties

                if (layerPropertiesToInclude.Contains("Top sine depth (mm)"))
                {
                    addCell(layersCells, row, currentColumn, System.Convert.ToString(layers[i].TopEdgeDepthMm));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Top sine azimuth"))
                {
                    addCell(layersCells, 
                            row, 
                            currentColumn, 
                            Convert.ToString(((BoreholeLayer)layers[i]).TopSineAzimuth));

                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Top sine amplitude (mm)"))
                {
                    addCell(layersCells, 
                            row, 
                            currentColumn, 
                            Convert.ToString(((BoreholeLayer)layers[i]).TopSineAmplitudeInMm));

                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Bottom sine depth (mm)"))
                {
                    addCell(layersCells, 
                            row, 
                            currentColumn, 
                            Convert.ToString(layers[i].BottomEdgeDepthMm));

                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Bottom sine azimuth"))
                {
                    addCell(layersCells, 
                            row, 
                            currentColumn, 
                            Convert.ToString(((BoreholeLayer)layers[i]).BottomSineAzimuth));

                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Bottom sine amplitude (mm)"))
                {
                    addCell(layersCells, 
                            row, 
                            currentColumn, 
                            Convert.ToString(((BoreholeLayer)layers[i]).BottomSineAmplitudeInMm));

                    currentColumn++;
                }

                # endregion

                # region Core only properties

                if (layerPropertiesToInclude.Contains("Top edge intercept (mm)"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(((CoreLayer)layers[i]).TopEdgeInterceptMm));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Top edge slope"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(((CoreLayer)layers[i]).TopEdgeSlope));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Bottom edge intercept (mm)"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(((CoreLayer)layers[i]).BottomEdgeInterceptMm));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Bottom edge slope"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(((CoreLayer)layers[i]).BottomEdgeSlope));
                    currentColumn++;
                }

                # endregion

                if (layerPropertiesToInclude.Contains("Group"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(layers[i].Group));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Layer type"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(layers[i].LayerType));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Layer description"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(layers[i].Description));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Layer quality"))
                {
                    addCell(layersCells, row, currentColumn, Convert.ToString(layers[i].Quality));
                    currentColumn++;
                }

                if (layerPropertiesToInclude.Contains("Mean layer brightness"))
                {
                    addCell(layersCells, row, currentColumn, layerBrightnesses[i].ToString());
                    currentColumn++;
                }
            }

            writeToWorksheet(layersWorksheet, layersCells);
        }
        
        public void writeClustersWorksheet()
        {
            clustersWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[currentWorksheet];
            currentWorksheet++;
            clustersWorksheet.Name = "Clusters";

            int row = 0;

            //Write column headers
            int currentColumn = 0;

            if (clusterPropertiesToInclude.Contains("Start depth (mm)"))
            {
                addCell(clustersCells, row, currentColumn, "Start Depth (mm)");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("End depth (mm)"))
            {
                addCell(clustersCells, row, currentColumn, "End Depth (mm)");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Top Y boundary"))
            {
                addCell(clustersCells, row, currentColumn, "Top Y boundary");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Bottom Y boundary"))
            {
                addCell(clustersCells, row, currentColumn, "Bottom Y boundary");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Left X boundary"))
            {
                addCell(clustersCells, row, currentColumn, "Left X boundary");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Right X boundary"))
            {
                addCell(clustersCells, row, currentColumn, "Right X boundary");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Points"))
            {
                addCell(clustersCells, row, currentColumn, "Points");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Cluster type"))
            {
                addCell(clustersCells, row, currentColumn, "Cluster type");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Cluster description"))
            {
                addCell(clustersCells, row, currentColumn, "Cluster description");
                currentColumn++;
            }

            if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
            {
                addCell(clustersCells, row, currentColumn, "Mean cluster brightness");
                currentColumn++;
            }

            for (int i = 0; i < clusters.Count; i++)
            {
                row++;

                currentColumn = 0;

                if (clusterPropertiesToInclude.Contains("Start depth (mm)"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].StartDepth));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("End depth (mm)"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].EndDepth));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Top Y boundary"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].TopYBoundary));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Bottom Y boundary"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].BottomYBoundary));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Left X boundary"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].LeftXBoundary));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Right X boundary"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].RightXBoundary));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Points"))
                {
                    addCell(clustersCells, row, currentColumn, clusters[i].PointsString);
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Cluster type"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].ClusterType));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Cluster description"))
                {
                    addCell(clustersCells, row, currentColumn, System.Convert.ToString(clusters[i].Description));
                    currentColumn++;
                }

                if (clusterPropertiesToInclude.Contains("Mean cluster brightness"))
                {
                    addCell(clustersCells, row, currentColumn, clusterBrightnesses[i].ToString());
                    currentColumn++;
                }
            }

            writeToWorksheet(clustersWorksheet, clustersCells);
        }

        public void writeInclusionsWorksheet()
        {
            inclusionsWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[currentWorksheet];
            currentWorksheet++;
            inclusionsWorksheet.Name = "Inclusions";

            int row = 0;

            //Write column headers
            int currentColumn = 0;

            if (inclusionPropertiesToInclude.Contains("Start depth (mm)"))
            {
                addCell(inclusionsCells, row, currentColumn, "Start Depth (mm)");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("End depth (mm)"))
            {
                addCell(inclusionsCells, row, currentColumn, "End Depth (mm)");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Top Y boundary"))
            {
                addCell(inclusionsCells, row, currentColumn, "Top Y boundary");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Bottom Y boundary"))
            {
                addCell(inclusionsCells, row, currentColumn, "Bottom Y boundary");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("left X boundary"))
            {
                addCell(inclusionsCells, row, currentColumn, "left X boundary");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Right X boundary"))
            {
                addCell(inclusionsCells, row, currentColumn, "Right X boundary");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Points"))
            {
                addCell(inclusionsCells, row, currentColumn, "Points");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Inclusion type"))
            {
                addCell(inclusionsCells, row, currentColumn, "Inclusion type");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Inclusion description"))
            {
                addCell(inclusionsCells, row, currentColumn, "Inclusion description");
                currentColumn++;
            }

            if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
            {
                addCell(inclusionsCells, row, currentColumn, "Mean inclusion brightness");
                currentColumn++;
            }

            for (int i = 0; i < inclusions.Count; i++)
            {
                row++;

                currentColumn = 0;

                if (inclusionPropertiesToInclude.Contains("Start depth (mm)"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].StartDepth));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("End depth (mm)"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].EndDepth));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Top Y boundary"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].TopYBoundary));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Bottom Y boundary"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].BottomYBoundary));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("left X boundary"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].LeftXBoundary));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Right X boundary"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].RightXBoundary));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Points"))
                {
                    addCell(inclusionsCells, row, currentColumn, inclusions[i].PointsString);
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Cluster type"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].InclusionType));
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Cluster description"))
                {
                    addCell(inclusionsCells, row, currentColumn, System.Convert.ToString(inclusions[i].Description)); 
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Mean inclusions brightness"))
                {
                    addCell(inclusionsCells, row, currentColumn, "Mean inclusion brightness");
                    currentColumn++;
                }

                if (inclusionPropertiesToInclude.Contains("Mean inclusion brightness"))
                {
                    addCell(inclusionsCells, row, currentColumn, inclusionBrightnesses[i].ToString());
                    currentColumn++;
                }
            }

            writeToWorksheet(inclusionsWorksheet, inclusionsCells);
        }

        /// <summary>
        /// Adds a string to the specified worksheet and cell
        /// </summary>
        /// <param name="worksheet">The worksheet to add the string to</param>
        /// <param name="row">The row to add the string to</param>
        /// <param name="col">The column to add the string to</param>
        /// <param name="data">The data to add</param>
        private void addCell(string[,] cellData, int row, int col, string data /*string cell1, string cell2, string format*/)
        {
            cellData[row, col] = data;
            //worksheet.Cells[row, col] = data;
        }

        private void writeToWorksheet(Microsoft.Office.Interop.Excel.Worksheet worksheet, string[,] cellData)
        {
            int rowCount = cellData.GetLength(0);
            int columnCount = cellData.GetLength(1);

            // Get an Excel Range of the same dimensions
            Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, 1];
            range = range.get_Resize(rowCount, columnCount);
            // Assign the 2-d array to the Excel Range
            range.set_Value(Microsoft.Office.Interop.Excel.XlRangeValueDataType.xlRangeValueDefault, cellData);
        }

        internal void AddLayerPropertiesToinclude(List<string> layerPropertiesToInclude)
        {
            if (layerPropertiesToInclude != null)
            {
                this.layerPropertiesToInclude = layerPropertiesToInclude;

                layersCells = new string[layers.Count + 1, layerPropertiesToInclude.Count];
            }
        }

        internal void AddClusterPropertiesToinclude(List<string> clusterPropertiesToInclude)
        {
            if (clusterPropertiesToInclude != null)
            {
                this.clusterPropertiesToInclude = clusterPropertiesToInclude;

                clustersCells = new string[clusters.Count + 1, clusterPropertiesToInclude.Count];
            }
        }

        internal void AddInclusionPropertiesToinclude(List<string> inclusionPropertiesToInclude)
        {
            if (inclusionPropertiesToInclude != null)
            {
                this.inclusionPropertiesToInclude = inclusionPropertiesToInclude;

                inclusionsCells = new string[inclusions.Count + 1, inclusionPropertiesToInclude.Count];
            }
        }

        internal void SetLayerBrightnesses(int[] layerBrightnesses)
        {
            this.layerBrightnesses = layerBrightnesses;
        }

        internal void SetClusterBrightnesses(int[] clusterBrightnesses)
        {
            this.clusterBrightnesses = clusterBrightnesses;
        }

        internal void SetInclusionBrightnesses(int[] inclusionBrightnesses)
        {
            this.inclusionBrightnesses = inclusionBrightnesses;
        }
    }
}
