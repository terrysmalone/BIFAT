using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using EdgeFitting;
using BoreholeFeatures;
using System.IO;

namespace LayerDetection
{
    /// <summary>
    /// Checks between detected sines to determine whether they belong to a layer
    /// </summary>
    public class DetectLayers
    {
        private List<Sine> foundSines;
        private List<EdgeLine> foundLines;

        private List<Layer> detectedLayers;
        private Bitmap image;
        private int depthResolution;

        private int layerBrightnessSensitivity = 1;

        private LayerTypeSelector layerTypeSelector;

        private int checkHeight = 20;

        LayerBrightness layerBrightness;
        int min, max, range;
        List<int> brightnesses, areas;

        private string imageType;

        # region Properties

        public List<Layer> DetectedLayers
        {
            get
            {
                return detectedLayers;
            }
        }

        public int LayerBrightnessSensitivity
        {
            get
            {
                return layerBrightnessSensitivity;
            }
            set
            {
                layerBrightnessSensitivity = value;
            }
        }

        # endregion

        # region Constructors

        public DetectLayers(List<Sine> foundSines, Bitmap image, int depthResolution)
        {
            imageType = "Borehole";
            layerTypeSelector = new LayerTypeSelector(imageType);

            this.foundSines = foundSines;
            this.image = image;
            this.depthResolution = depthResolution;            
        }

        public DetectLayers(List<EdgeLine> foundLines, Bitmap image, int depthResolution)
        {
            imageType = "Core";
            layerTypeSelector = new LayerTypeSelector(imageType);

            this.foundLines = foundLines;
            this.image = image;
            this.depthResolution = depthResolution;            
        }

        public void Run()
        {
            layerBrightness = new LayerBrightness(image);

            detectedLayers = new List<Layer>();

            if (imageType.Equals("Borehole"))
            {
                if (foundSines.Count > 0)
                {
                    if (foundSines.Count > 1)
                        CheckAroundSineLayer();
                    else
                    {
                        var onlySine = foundSines[0];
                        var layer = layerTypeSelector.setUpLayer(onlySine.Depth, 
                                                                 onlySine.Amplitude, 
                                                                 onlySine.Azimuth, 
                                                                 onlySine.Depth, 
                                                                 onlySine.Amplitude, 
                                                                 onlySine.Azimuth, 
                                                                 image.Width, 
                                                                 depthResolution);
                        layer.Quality = onlySine.Quality;

                        detectedLayers.Add(layer);
                    }
                }
            }
            else
            {
                if (foundLines.Count > 0)
                {
                    if (foundLines.Count > 1)
                        CheckAroundLineLayer();
                    else
                    {
                        EdgeLine onlyLine = foundLines[0];

                        Layer layer = layerTypeSelector.setUpLayer(onlyLine.Slope, onlyLine.Intercept, onlyLine.Slope, onlyLine.Intercept, image.Width, depthResolution);
                        layer.Quality = onlyLine.Quality;

                        detectedLayers.Add(layer);
                    }
                }
            }

            detectedLayers.Sort(new LayerDepthComparer());
        }

        # endregion

        # region Sine methods

        /// <summary>
        /// Checks the brightness outwith the potential layer
        /// </summary>
        private void CheckAroundSineLayer()
        {
            AddBoundarySines();

            CalculatePossibleSineBrightnesses();

            RemoveSolitarySines();

            RemoveBoundarySines();

            FindSineLayers();            
        }         

        /// <summary>
        /// Adds the top and bottom edges of the image as boundary sines
        /// </summary>
        private void AddBoundarySines()
        {
            Sine topBoundarySine, bottomBoundarySine;

            topBoundarySine = new Sine(0, 0, 0, image.Width);
            bottomBoundarySine = new Sine(image.Height - 1, 0, 0, image.Width);

            foundSines.Insert(0, topBoundarySine);
            foundSines.Insert(foundSines.Count, bottomBoundarySine);
        }

        /// <summary>
        /// Calculate the brightnesses of possible layers between all consecutive edges
        /// </summary>
        /// <returns></returns>
        private void CalculatePossibleSineBrightnesses()
        {
            brightnesses = new List<int>();
            areas = new List<int>();

            min = 255;
            max = 0;

            for (int i = 0; i < foundSines.Count - 1; i++)
            {
                Sine top = foundSines[i];
                Sine bottom = foundSines[i + 1];

                int brightness = layerBrightness.GetBrightness(top, bottom);
                brightnesses.Add(brightness);

                int area = layerBrightness.GetLastLayerCount();
                areas.Add(area);

                if (brightness < min)
                    min = brightness;
                else if (brightness > max)
                    max = brightness;
            }

            range = max - min;
        }

        /// <summary>
        /// Remove sines that are not on layer edges (i.e. they are surrounded by similar brightnesses)
        /// </summary>
        private void RemoveSolitarySines()
        {
            for (int i = brightnesses.Count - 2; i >= 0; i--)       //Go backwards so items can be removed from foundSines
            {
                if (AreBrightnessesClose(brightnesses[i], brightnesses[i + 1], range))
                {
                    Sine sineToAdd = foundSines[i + 1];

                    AddSine(sineToAdd);

                    RemoveSineBetween(i, i + 1);                    
                }
            }
        }

        /// <summary>
        /// Adds a Sine to the detected layers
        /// </summary>
        /// <param name="sineToAdd"></param>
        private void AddSine(Sine sineToAdd)
        {
            int layerDepth = sineToAdd.Depth;
            int layerAmplitude = sineToAdd.Amplitude;
            int layerAzimuth = sineToAdd.Azimuth;

            Layer layerToAdd = layerTypeSelector.setUpLayer(layerDepth, layerAmplitude, layerAzimuth, layerDepth, layerAmplitude, layerAzimuth, image.Width, depthResolution);
            layerToAdd.Quality = sineToAdd.Quality;

            detectedLayers.Add(layerToAdd);
        }

        /// <summary>
        /// Removes data of two positions from areas and brightness lists and replaces with new combined data
        /// </summary>
        /// <param name="i"></param>
        /// <param name="p"></param>
        private void RemoveSineBetween(int first, int second)
        {
            foundSines.RemoveAt(second);

            int combinedArea = areas[first] + areas[second];

            int combinedBrightness = ((brightnesses[first] * areas[first]) + (brightnesses[second] * areas[second])) / combinedArea;

            brightnesses.RemoveAt(second);
            brightnesses.RemoveAt(first);

            areas.RemoveAt(second);
            areas.RemoveAt(first);

            brightnesses.Insert(first, combinedBrightness);
            areas.Insert(first, combinedArea);
        }

        public void RemoveBoundarySines()
        {
            foundSines.RemoveAt(0);
            foundSines.RemoveAt(foundSines.Count - 1);
        }

        private void FindSineLayers()
        {
            Sine topSine, bottomSine;

            //Work out which of the remaining edges belong together as Layers
            for (int i = 1; i < brightnesses.Count - 1; i++)
            {
                if (brightnesses[i] < ((brightnesses[i + 1] + brightnesses[i - 1]) / 2))
                {
                    //Combine sines - -1 and i
                    topSine = foundSines[i - 1];
                    bottomSine = foundSines[i];

                    Layer layerToAdd = layerTypeSelector.setUpLayer(topSine.Depth, topSine.Amplitude, topSine.Azimuth, bottomSine.Depth, bottomSine.Amplitude, bottomSine.Azimuth, image.Width, depthResolution);
                    layerToAdd.Quality = topSine.Quality;

                    detectedLayers.Add(layerToAdd);
                    i++;
                }
                else
                {
                    topSine = foundSines[i - 1];

                    Layer layerToAdd = layerTypeSelector.setUpLayer(topSine.Depth, topSine.Amplitude, topSine.Azimuth, topSine.Depth, topSine.Amplitude, topSine.Azimuth, image.Width, depthResolution);
                    layerToAdd.Quality = topSine.Quality;

                    detectedLayers.Add(layerToAdd); 
                }

                if (i == brightnesses.Count - 2)        //Last sine is not a part of a layer so must be added 
                {
                    topSine = foundSines[foundSines.Count - 1];

                    Layer lastLayer = layerTypeSelector.setUpLayer(topSine.Depth, topSine.Amplitude, topSine.Azimuth, topSine.Depth, topSine.Amplitude, topSine.Azimuth, image.Width, depthResolution);
                    lastLayer.Quality = topSine.Quality;

                    detectedLayers.Add(lastLayer);
                }               
            }
        }

        # endregion

        # region EdgeLine methods

        /// <summary>
        /// Checks the brightness outwith the potential layer
        /// </summary>
        private void CheckAroundLineLayer()
        {
            AddBoundaryLines();

            CalculatePossibleLineBrightnesses();

            RemoveSolitaryLines();

            RemoveBoundaryLines();

            FindLineLayers();   
        }

        private void AddBoundaryLines()
        {
            EdgeLine topBoundaryLine, bottomBoundaryLine;

            topBoundaryLine = new EdgeLine(0, 0, image.Width);
            bottomBoundaryLine = new EdgeLine(0, image.Height - 1, image.Width);

            foundLines.Insert(0, topBoundaryLine);
            foundLines.Insert(foundLines.Count, bottomBoundaryLine);
        }

        private void CalculatePossibleLineBrightnesses()
        {
            brightnesses = new List<int>();
            areas = new List<int>();

            min = 255;
            max = 0;

            for (int i = 0; i < foundLines.Count - 1; i++)
            {
                EdgeLine top = foundLines[i];
                EdgeLine bottom = foundLines[i + 1];

                int brightness = layerBrightness.GetBrightness(top, bottom);
                brightnesses.Add(brightness);

                int area = layerBrightness.GetLastLayerCount();
                areas.Add(area);

                if (brightness < min)
                    min = brightness;
                else if (brightness > max)
                    max = brightness;
            }

            range = max - min;

        }
                
        private void RemoveSolitaryLines()
        {
            for (int i = brightnesses.Count - 2; i >= 0; i--)       //Go backwards so items can be removed from foundLines
            {
                if (AreBrightnessesClose(brightnesses[i], brightnesses[i + 1], range))
                {
                    EdgeLine lineToAdd = foundLines[i + 1];

                    double layerSlope = lineToAdd.Slope;
                    int layerIntercept = lineToAdd.Intercept;

                    Layer layerToAdd = layerTypeSelector.setUpLayer(layerSlope, layerIntercept, layerSlope, layerIntercept, image.Width, depthResolution);
                    layerToAdd.Quality = layerToAdd.Quality;

                    detectedLayers.Add(layerToAdd);

                    foundLines.RemoveAt(i + 1);

                    int combinedArea = areas[i] + areas[i + 1];

                    int combinedBrightness = ((brightnesses[i] * areas[i]) + (brightnesses[i + 1] * areas[i + 1])) / combinedArea;

                    brightnesses.RemoveAt(i + 1);
                    brightnesses.RemoveAt(i);

                    areas.RemoveAt(i + 1);
                    areas.RemoveAt(i);

                    brightnesses.Insert(i, combinedBrightness);
                    areas.Insert(i, combinedArea);
                }
            }
        }

        private void RemoveBoundaryLines()
        {
            foundLines.RemoveAt(0);
            foundLines.RemoveAt(foundLines.Count - 1);
        }

        private void FindLineLayers()
        {
            EdgeLine topLine, bottomLine;

            //Work out which of the remaining edges belong together as Layers
            for (int i = 1; i < brightnesses.Count - 1; i++)
            {
                if (brightnesses[i] < ((brightnesses[i + 1] + brightnesses[i - 1]) / 2))
                {
                    //Combine sines - -1 and i
                    topLine = foundLines[i - 1];
                    bottomLine = foundLines[i];

                    Layer layerToAdd = layerTypeSelector.setUpLayer(topLine.Slope, topLine.Intercept, bottomLine.Slope, bottomLine.Intercept, image.Width, depthResolution);
                    layerToAdd.Quality = topLine.Quality;

                    detectedLayers.Add(layerToAdd);
                    i++;
                }
                else
                {
                    topLine = foundLines[i - 1];

                    Layer layerToAdd = layerTypeSelector.setUpLayer(topLine.Slope, topLine.Intercept, topLine.Slope, topLine.Intercept, image.Width, depthResolution);
                    layerToAdd.Quality = topLine.Quality;

                    detectedLayers.Add(layerToAdd);
                }

                if (i == brightnesses.Count - 2)        //Last line is not a part of a layer so must be added 
                {
                    topLine = foundLines[foundLines.Count - 1];

                    Layer lastLayer = layerTypeSelector.setUpLayer(topLine.Slope, topLine.Intercept, topLine.Slope, topLine.Intercept, image.Width, depthResolution);
                    lastLayer.Quality = topLine.Quality;

                    detectedLayers.Add(lastLayer);
                }
            }
        }

        # endregion

        /// <summary>
        /// Decides if two layer brightnesses are close enough to be considered the same layer
        /// </summary>
        /// <param name="brightness1"></param>
        /// <param name="brightness2"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        private bool AreBrightnessesClose(int brightness1, int brightness2, int difference)
        {
            int diff = Math.Max(brightness1, brightness2) - Math.Min(brightness1, brightness2);
            
            //if (diff < (double)difference / 4.0)
            if (diff < ((double)difference / 10.0) * (double)layerBrightnessSensitivity)
                return true;
            else
                return false;
        }           
    }

}
