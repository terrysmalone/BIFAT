using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using BoreholeFeatures;
using System.Windows.Forms;

namespace BrightnessAnalysis
{
    /// <summary>
    /// Calculates the average brighness of every line in a borehole image
    /// </summary>
    public class AverageBrightness
    {
        private Bitmap boreholeSection;
        private int boreholeSectionHeight, boreholeSectionWidth;

        private List<int> averageBrightnesses;
        private List<int> averageRBrightnesses;
        private List<int> averageGBrightnesses;
        private List<int> averageBBrightnesses;

        private int sampleRate = 1;
        private int depthResolution = 1;
        private int boreholeStartDepth = 0;
        private int sectionStartDepth = 0;

        private int totalLayerBrightness;
        private int numberOfLayerPoints;

        private int totalPolygonBrightness;
        private int numberOfPolygonPoints;

        private bool includeBackground;
        private List<Layer> layersToInclude;
        private List<Cluster> clustersToInclude;
        private List<Inclusion> inclusionsToInclude;

        private List<Layer> allLayers;
        private List<Cluster> allClusters;
        private List<Inclusion> allInclusions;

        public AverageBrightness()
        {
            averageBrightnesses = new List<int>();

            averageRBrightnesses = new List<int>();
            averageGBrightnesses = new List<int>();
            averageBBrightnesses = new List<int>();

            includeBackground = true;

            layersToInclude = new List<Layer>();
            clustersToInclude = new List<Cluster>();
            inclusionsToInclude = new List<Inclusion>();

            allLayers = new List<Layer>();
            allClusters = new List<Cluster>();
            allInclusions = new List<Inclusion>();
        }

        # region Add to exclude list methods

        public void AddLayersToIncludeList(List<Layer> layersToAdd)
        {
            layersToInclude.AddRange(layersToAdd);
        }

        public void AddLayerToIncludeList(Layer layerToAdd)
        {
            layersToInclude.Add(layerToAdd);
        }

        public void AddClustersToIncludeList(List<Cluster> clustersToAdd)
        {
            clustersToInclude.AddRange(clustersToAdd);
        }

        public void AddClusterToIncludeList(Cluster clusterToAdd)
        {
            clustersToInclude.Add(clusterToAdd);
        }

        public void AddInclusionsToIncludeList(List<Inclusion> inclusionsToAdd)
        {
            inclusionsToInclude.AddRange(inclusionsToAdd);
        }

        public void AddInclusionToIncludeList(Inclusion inclusionToAdd)
        {
            inclusionsToInclude.Add(inclusionToAdd);
        }

        # endregion

        public void ProcessSection(Bitmap boreholeSection, int sectionStart)
        {
            this.boreholeSection = boreholeSection;
            this.sectionStartDepth = sectionStart;

            boreholeSectionWidth = boreholeSection.Width;
            boreholeSectionHeight = boreholeSection.Height;

            CalculateBrightness();
        }

        private void CalculateBrightness()
        {
            double pixelSampleRate = (double)sampleRate / (double)depthResolution;

            int numberOfLinesScanned = (int)((double)boreholeSectionHeight / (double)pixelSampleRate);

            for (int line = 0; line < numberOfLinesScanned; line++)
            {
                int sample = (int)(line * pixelSampleRate);

                //averageBrightnesses.Add(CalculateLineBrightness(sample));
                CalculateLineBrightness(sample);
            }
        }

        private void CalculateLineBrightness(int line)
        {
            int averageBrightness = 0;
            int total = 0;
            int excludePointsInLine = 0;

            int averageR = 0;
            int totalR = 0;
            int averageG = 0;
            int totalG = 0;
            int averageB = 0;
            int totalB = 0;

            for (int column = 0; column < boreholeSectionWidth; column++)
            {
                if (IsPointToBeIncluded(column, line))
                {
                    Color currentPixel = boreholeSection.GetPixel(column, line);

                    int rgb = currentPixel.ToArgb();
                    total += GetBrightnessOfPixel(rgb);

                    totalR += currentPixel.R;
                    totalG += currentPixel.G;
                    totalB += currentPixel.B;
                }
                else
                {
                    excludePointsInLine++;
                }
            }

            if (excludePointsInLine == boreholeSectionWidth)
            {
                averageBrightness = -1;

                averageR = -1;
                averageG = -1;
                averageB = -1;
            }
            else
            {
                averageBrightness = (int)((double)total / ((double)(boreholeSectionWidth - excludePointsInLine)));

                averageR = (int)((double)totalR / ((double)(boreholeSectionWidth - excludePointsInLine)));
                averageG = (int)((double)totalG / ((double)(boreholeSectionWidth - excludePointsInLine)));
                averageB = (int)((double)totalB / ((double)(boreholeSectionWidth - excludePointsInLine)));
            }

            averageBrightnesses.Add(averageBrightness);
            averageRBrightnesses.Add(averageR);
            averageGBrightnesses.Add(averageG);
            averageBBrightnesses.Add(averageB);
        }

        # region Is point within feature methods

        /// <summary>
        /// Returns whether the given coordinate falls within one of the features to exclude
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <returns>True if point is within an exclude feature</returns>
        private bool IsPointToBeIncluded(int xPos, int yPos)
        {
            //bool pointIsInIncludeFeature = false;

            if (includeBackground == true)
            {
                if (IsPointWithinBackground(xPos, yPos))
                {
                    return true;
                }
            }

            if (IsPointWithinIncludedLayer(xPos, yPos) || IsPointWithinIncludedCluster(xPos, yPos) || IsPointWithinIncludedInclusion(xPos, yPos))
            {
                return true;
            }

            return false;
        }

        private bool IsPointWithinBackground(int xPos, int yPos)
        {
            //Check all layers
            for (int i = 0; i < allLayers.Count; i++)
            {
                Layer currentLayer = allLayers[i];

                if (yPos > currentLayer.GetTopYPoint(xPos) - sectionStartDepth && yPos < currentLayer.GetBottomYPoint(xPos) - sectionStartDepth)
                    return false;
            }

            //Check all clusters
            for (int i = 0; i < allClusters.Count; i++)
            {
                Cluster currentCluster = allClusters[i];

                if (isPointWithinPolygon(xPos, yPos, currentCluster.Points))
                    return false;
            }

            //Check all inclusions
            for (int i = 0; i < allInclusions.Count; i++)
            {
                Inclusion currentInclusion = allInclusions[i];

                if (isPointWithinPolygon(xPos, yPos, currentInclusion.Points))
                    return false;
            }

            return true;
        }

        private bool IsPointWithinIncludedLayer(int xPos, int yPos)
        {
            for (int i = 0; i < layersToInclude.Count; i++)
            {
                Layer currentLayer = layersToInclude[i];

                if (yPos > currentLayer.GetTopYPoint(xPos) - sectionStartDepth && yPos < currentLayer.GetBottomYPoint(xPos) - sectionStartDepth)
                    return true;
            }

            return false;
        }

        private bool IsPointWithinIncludedCluster(int xPos, int yPos)
        {
            for (int i = 0; i < clustersToInclude.Count; i++)
            {
                Cluster currentCluster = clustersToInclude[i];

                if (isPointWithinPolygon(xPos, yPos, currentCluster.Points))
                    return true;
            }

            return false;
        }

        private bool IsPointWithinIncludedInclusion(int xPos, int yPos)
        {
            for (int i = 0; i < inclusionsToInclude.Count; i++)
            {
                Inclusion currentInclusion = inclusionsToInclude[i];

                if (isPointWithinPolygon(xPos, yPos, currentInclusion.Points))
                    return true;
            }

            return false;
        }

        private bool isPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
        {
            yPoint += sectionStartDepth;
            
            Point p1, p2;

            bool inside = false;

            if (polygonPoints.Count < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(polygonPoints[polygonPoints.Count - 1].X, polygonPoints[polygonPoints.Count - 1].Y);

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Point newPoint = new Point(polygonPoints[i].X, polygonPoints[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < xPoint) == (xPoint <= oldPoint.X) && ((long)yPoint - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(xPoint - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        # endregion

        /// <summary>
        /// Calculates the brightness of a pixel from its given int value
        /// </summary>
        /// <param name="currentPixel"></param>
        /// <returns>The brightness of the pixel ranging from 0-255</returns>
        private int GetBrightnessOfPixel(int currentPixel)
        {
            int brightness;

            int r = (currentPixel & 0xff0000) >> 16;
            int g = (currentPixel & 0xff00) >> 8;
            int b = currentPixel & 0xff;

            brightness = CalculateBrightness(r, g, b);
              
            return brightness;
        }

        /**
         * Returns the brightness value of a given RGB
         * 
         * @param r The red value of the pixel
         * @param g The green value of the pixel
         * @param b The blue value of the pixel
         * 
         * @return The brightness value associated with the given RGB values
         */
        private int CalculateBrightness(float r, float g, float b)
        {
            int brightness = (int)Math.Floor(0.334f * r + 0.333f * g + 0.333f * b);

            return brightness;
        }

        /// <summary>
        /// Writes the average brightnesses to an excel file
        /// </summary>
        /// <param name="fileName">The file to write to</param>
        public void WriteToExcel(string fileName)
        {
            WriteDataToExcel write = new WriteDataToExcel(fileName, averageBrightnesses, averageRBrightnesses, averageGBrightnesses, averageBBrightnesses, sampleRate);
            write.setDepthResolution(depthResolution);
            write.setStartDepth(boreholeStartDepth);
            write.write();
        }

        public void WriteToTxt(string fileName)
        {
            WriteDataToText write = new WriteDataToText(fileName, averageBrightnesses, averageRBrightnesses, averageGBrightnesses, averageBBrightnesses, sampleRate);
            write.setDepthResolution(depthResolution);
            write.setStartDepth(boreholeStartDepth);
            write.write();
        }

        public void WriteToExcel()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "*.xlsx";
            saveFileDialog.Filter = "Excel File|*.xlsx";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WriteDataToExcel write = new WriteDataToExcel(saveFileDialog.FileName, averageBrightnesses, sampleRate);
                write.setDepthResolution(depthResolution);
                write.setStartDepth(boreholeStartDepth);
                write.write();
            }
        }

        # region Get methods

        public int GetBrightnessOfLine(int lineNumber)
        {
            if (lineNumber >= 0 && lineNumber < boreholeSectionHeight)
                return averageBrightnesses[lineNumber];
            else return 0;
        }

        public List<int> GetAllAverageBrightnesses()
        {
            return averageBrightnesses;
        }

        public int GetBoreholeHeight()
        {
            return boreholeSectionHeight;
        }

        public int GetBoreholeWidth()
        {
            return boreholeSectionWidth;
        }

        # region Feature Brightness methods

        /// <summary>
        /// Returns the brightness of a given layer
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public int GetBrightnessOfLayer(Layer layer)
        {
            int layerBrightness = 0;
            totalLayerBrightness = 0;
            numberOfLayerPoints = 0;
            //int totalBrightness = 0;
            //int numberOfLayerPoints = 0;

            int layerStart = layer.StartDepth;
            int layerEnd = layer.EndDepth;

            int boreholeSectionHeight = boreholeSection.Height;
            int heightOfSectionInMM = boreholeSectionHeight * depthResolution;

            if (layerStart < (sectionStartDepth + heightOfSectionInMM) && layerEnd > sectionStartDepth)     //Layer is within section
            {
                List<Point> topPoints = layer.GetTopEdgePoints();
                List<Point> bottomPoints = layer.GetBottomEdgePoints();

                for (int xPos = topPoints[0].X; xPos <= topPoints[topPoints.Count - 1].X; xPos++)
                {
                    for (int yPos = topPoints[xPos].Y; yPos <= bottomPoints[xPos].Y; yPos++)
                    {
                        int yPosMM = (yPos * depthResolution) + boreholeStartDepth;

                        if (yPosMM < (sectionStartDepth + heightOfSectionInMM) && yPosMM > sectionStartDepth)
                        {
                            int y = yPos - (int)(((double)sectionStartDepth - (double)boreholeStartDepth) / (double)depthResolution) - 1;

                            int rgb = boreholeSection.GetPixel(xPos, y).ToArgb();
                            totalLayerBrightness += GetBrightnessOfPixel(rgb);

                            numberOfLayerPoints++;
                        }
                    }
                }
            }

            layerBrightness = (int)((double)totalLayerBrightness / (double)numberOfLayerPoints);

            return layerBrightness;
        }

        public int GetTotalLayerBrightness()
        {
            return totalLayerBrightness;
        }

        public int GetNumberOfLayerPointsUsed()
        {
            return numberOfLayerPoints;
        }

        # endregion

        # region Cluster/Inclusion methods

        public int GetBrightnessOfCluster(Cluster cluster)
        {
            List<Point> polygonPoints = cluster.Points;

            int clusterBrightness = GetBrightnessOfPolygon(polygonPoints, cluster.StartDepth, cluster.EndDepth);

            return clusterBrightness;
        }

        public int GetBrightnessOfInclusion(Inclusion inclusion)
        {
            List<Point> polygonPoints = inclusion.Points;

            int inclusionBrightness = GetBrightnessOfPolygon(polygonPoints, inclusion.StartDepth, inclusion.EndDepth);

            return inclusionBrightness;
        }

        private int GetBrightnessOfPolygon(List<Point> polygonPoints, int polygonStartMM, int polygonEndMM)
        {
            int polygonBrightness = 0;

            totalPolygonBrightness = 0;
            numberOfPolygonPoints = 0;

            int boreholeSectionHeight = boreholeSection.Height;
            int heightOfSectionInMM = boreholeSectionHeight * depthResolution;

            if (polygonStartMM < (sectionStartDepth + heightOfSectionInMM) && polygonEndMM > sectionStartDepth)     //Layer is within section
            {
                for (int xPos = 0; xPos < boreholeSection.Width; xPos++)
                {
                    for (int yPos = 0; yPos < boreholeSection.Height; yPos++)
                    {
                        //int yPosMM = (yPos * depthResolution) + boreholeStartDepth + sectionStartDepth;

                        int y = yPos + (int)(((double)sectionStartDepth - (double)boreholeStartDepth) / (double)depthResolution);

                        if (IsPointWithinPolygon(xPos, y, polygonPoints))                        
                        {
                            //int y = yPos - (int)(((double)sectionStartDepth - (double)boreholeStartDepth) / (double)depthResolution) - 1;

                            int rgb = boreholeSection.GetPixel(xPos, yPos).ToArgb();

                            int pixelBrightness = GetBrightnessOfPixel(rgb);
                            
                            totalPolygonBrightness += pixelBrightness;

                            numberOfPolygonPoints++;
                        }
                    }
                }
            }

            polygonBrightness = (int)((double)totalPolygonBrightness / (double)numberOfPolygonPoints);

            return polygonBrightness;
        }

        /// <summary>
        /// Checks if a given point is within the bounds of a given polygon
        /// </summary>
        /// <param name="xPoint">The x position of the point to check</param>
        /// <param name="yPoint">The y position of the point to check</param>
        /// <param name="polygonPoints">A List of all the corner Points of the polgon</param>
        /// <returns></returns>
        private bool IsPointWithinPolygon(int xPoint, int yPoint, List<Point> polygonPoints)
        {
            Point p1, p2;

            bool inside = false;

            if (polygonPoints.Count < 3)
            {
                return inside;
            }

            Point oldPoint = new Point(polygonPoints[polygonPoints.Count - 1].X, polygonPoints[polygonPoints.Count - 1].Y);

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Point newPoint = new Point(polygonPoints[i].X, polygonPoints[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < xPoint) == (xPoint <= oldPoint.X) && ((long)yPoint - (long)p1.Y) * (long)(p2.X - p1.X) < ((long)p2.Y - (long)p1.Y) * (long)(xPoint - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        public int GetTotalFeatureBrightness()
        {
            return totalPolygonBrightness;
        }

        public int GetNumberOfFeaturePointsUsed()
        {
            return numberOfPolygonPoints;
        }

        # endregion

        # endregion

        # region Set methods

        /// <summary>
        /// Sets the gap between lines
        /// </summary>
        /// <param name="sampleRate"></param>
        public void SetSamplingRate(int sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        /// <summary>
        /// Sets the depth resolution in the form of mm per pixels
        /// </summary>
        /// <param name="depthResolution"></param>
        public void SetDepthResolution(int depthResolution)
        {
            this.depthResolution = depthResolution;
        }

        /// <summary>
        /// Sets the start depth of the borehole image
        /// </summary>
        /// <param name="startDepth"></param>
        public void SetStartDepth(int startDepth)
        {
            this.boreholeStartDepth = startDepth;
        }

        public void SetSectionStartDepth(int sectionStartDepth)
        {
            this.sectionStartDepth = sectionStartDepth;
        }

        public void SetCurrentSectionImage(Bitmap currentSectionImage)
        {
            this.boreholeSection = currentSectionImage;
        }

        public void SetIncludeBackground(bool includeBackground)
        {
            this.includeBackground = includeBackground;
        }

        public void SetAllLayers(List<Layer> allLayers)
        {
            this.allLayers = allLayers;
        }

        public void SetAllClusters(List<Cluster> allClusters)
        {
            this.allClusters = allClusters;
        }

        public void SetAllInclusions(List<Inclusion> allInclusions)
        {
            this.allInclusions = allInclusions;
        }

        # endregion
    }
}
