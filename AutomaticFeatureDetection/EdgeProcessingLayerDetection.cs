using System;
using System.Collections.Generic;
using System.Drawing;
using Edges;
using DrawEdges;
using BoreholeFeatures;
using LayerDetection;
using DrawEdges.DrawEdgesFactory;
using PerformEdgeOperations;
using EdgeFitting;
using EdgeDetector;

namespace AutomaticFeatureDetection
{
    /// <summary>
    /// Sets up all automatic layer detection classes
    /// </summary>
    public class EdgeProcessingLayerDetection
    {
        //parameters

        //Data
        private int m_ImageWidth;
        private int m_ImageHeight;

        private int[] m_ImageData;

        private readonly int m_AzimuthResolution;
        
        private readonly Bitmap m_OriginalImage;
        private List<Edge> m_FoundEdges = new List<Edge>();
        //private List<Edge> m_EdgesBeforeOperations = new List<Edge>();
        //private readonly List<Edge> m_EdgesAfterSineFitting = new List<Edge>();

        private List<Sine> m_DetectedFitSines = new List<Sine>();
        private List<EdgeLine> m_DetectedFitLines = new List<EdgeLine>();
        
        //Detected features

        private readonly LayerTypeSelector m_LayerTypeSelector = new LayerTypeSelector("Borehole");


        # region properties
        
        public List<Layer> DetectedLayers { get; private set; } = new List<Layer>();
        
        public int StartHeight { get; }

        public float CannyLow { get; set; } = 0.01f;

        public float CannyHigh { get; set; } = 0.016f;

        public int GaussianWidth { get; set; } = 20;

        public int GaussianAngle { get; set; } = 6;

        public float VerticalWeight { get; set; }

        public float HorizontalWeight { get; set; }

        public int EdgeLinkingThreshold { get; set; } = 320;

        public int EdgeLinkingDistance { get; set; } = 5;

        public int RemoveEdgeThreshold { get; set; } = 320;

        public int JoinEdgeThreshold { get; set; } = 20;

        public int LayerSensitivity { get; set; } = 1;

        public bool DisableLayerDetection { get; set; }

        public int MaxSineAmplitude { get; set; } = 100;
        
        public string ImageType { get; set; } = "Borehole";

        public int DepthResolution { get; set; }

        # region Test properties

        public bool DrawTestImages { get; set; }

        public Color TestDrawingBackgroundColour { get; set; }

        public Color TestDrawingEdgeColour { get; set; }

        public Color TestDrawingOverBackgroundColour { get; set; }

        public bool TestDrawMultiColouredEdges { get; set; }

        # endregion

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public EdgeProcessingLayerDetection(Bitmap originalImage, int startHeight)
        {
            var initializeTime = DateTime.Now;
            Console.WriteLine("Initialising at: " + initializeTime);

            m_OriginalImage = originalImage;
            StartHeight = startHeight;

            m_AzimuthResolution = originalImage.Width;
            DepthResolution = 1;

            SetUpImageData();

            var setUpTime = DateTime.Now;
            Console.WriteLine("Setup Done: " + setUpTime);
        }

        /// <summary>
        /// Sets up the imageData byte array from the given Bitmap
        /// </summary>
        private void SetUpImageData()
        {
            m_ImageData = BitmapConverter.GetRgbFromBitmap(m_OriginalImage);

            m_ImageHeight = m_OriginalImage.Height;
            m_ImageWidth = m_OriginalImage.Width;
        }

        /// <summary>
        /// Calls all other methods and runs the feature detection
        /// </summary>
        public void DetectFeatures()
        {
            bool[] cannyData = PerformCannyEdgeDetection();

            m_FoundEdges = PerformEdgeOperations(cannyData);

            if (ImageType == "Borehole")
            {
                m_DetectedFitSines = PerformSineFitting();

                if (DrawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(m_OriginalImage, m_DetectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (DisableLayerDetection == false)
                {
                    var detectLayers = 
                        new DetectLayers(m_DetectedFitSines, m_OriginalImage, DepthResolution)
                        {
                            LayerBrightnessSensitivity = LayerSensitivity
                        };

                    detectLayers.Run();

                    DetectedLayers = detectLayers.DetectedLayers;
                }
                else
                {
                    foreach (var detectedFitSines in m_DetectedFitSines)
                    {
                        var currentLayerToAdd = m_LayerTypeSelector.setUpLayer(detectedFitSines.Depth 
                                                                               + StartHeight, 
                                                                               detectedFitSines.Amplitude, 
                                                                               detectedFitSines.Azimuth, 
                                                                               detectedFitSines.Depth 
                                                                               + StartHeight, 
                                                                               detectedFitSines.Amplitude, 
                                                                               detectedFitSines.Azimuth, 
                                                                               m_AzimuthResolution, 
                                                                               DepthResolution);

                        DetectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
            else
            {
                m_DetectedFitLines = PerformLineFitting();

                if (DrawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(m_OriginalImage, m_DetectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (DisableLayerDetection == false)
                {
                    DetectLayers detectLayers =
                        new DetectLayers(m_DetectedFitLines, m_OriginalImage, DepthResolution)
                        {
                            LayerBrightnessSensitivity = LayerSensitivity
                        };
                    
                    detectLayers.Run();

                    DetectedLayers = detectLayers.DetectedLayers;
                }
                else
                {
                    foreach (var detectedFitLines in m_DetectedFitLines)
                    {
                        var currentLayerToAdd = m_LayerTypeSelector.setUpLayer(detectedFitLines.Slope, 
                                                                               detectedFitLines.Intercept 
                                                                                  + StartHeight, 
                                                                               detectedFitLines.Slope, 
                                                                               detectedFitLines.Intercept 
                                                                                  + StartHeight, 
                                                                               m_AzimuthResolution, 
                                                                               DepthResolution);

                        DetectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
        }

        private bool[] PerformCannyEdgeDetection()
        {
            CannyDetector detector =
                new CannyDetector(m_ImageData, m_ImageWidth, m_ImageHeight)
                {
                    WrapHorizontally = true,
                    WrapVertically = true,
                    LowThreshold = CannyLow,
                    HighThreshold = CannyHigh,
                    GaussianWidth = GaussianWidth,
                    GaussianSigma = GaussianAngle,
                    HorizontalWeight = HorizontalWeight,
                    VerticalWeight = VerticalWeight
                };

            detector.DetectEdges();

            var edgeData = detector.GetEdgeData();

            if (DrawTestImages)
            {
                var factory = new DrawEdgesImageFactory("Bool");
                var drawImage = factory.setUpDrawEdges(edgeData, m_ImageWidth, m_ImageHeight);

                drawImage.setBackgroundColour(TestDrawingBackgroundColour);
                drawImage.setEdgeColour(TestDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(TestDrawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save($"01a - Canny({detector.LowThreshold}" +
                                               $",{detector.HighThreshold}" +
                                               $",{detector.GaussianWidth}" +
                                               $",{(int)detector.GaussianSigma}.bmp");

                drawImage.setEdgeColour(TestDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
                drawImage.getDrawnEdges().Save($"01b - Canny over original({detector.LowThreshold}" +
                                               $",{detector.HighThreshold}" +
                                               $",{detector.GaussianWidth}" +
                                               $",{(int)detector.GaussianSigma}.bmp");
            }

            return edgeData;
        }

        private List<Edge> PerformEdgeOperations(bool[] cannyData)
        {
            var performEdgeOperations =
                new EdgeOperations(cannyData, m_ImageWidth, m_ImageHeight)
                {
                    DrawTestImages = DrawTestImages,
                    EdgeLinkingThreshold = EdgeLinkingThreshold,
                    EdgeLinkingDistance = EdgeLinkingDistance,
                    EdgeRemovalThreshold = RemoveEdgeThreshold,
                    EdgeJoiningThreshold = JoinEdgeThreshold,
                    TestDrawingBackgroundColour = TestDrawingBackgroundColour,
                    TestDrawingEdgeColour = TestDrawingEdgeColour,
                    TestDrawingOverBackgroundColour = TestDrawingOverBackgroundColour,
                    TestDrawMultiColouredEdges = TestDrawMultiColouredEdges
                };

            if (DrawTestImages)
                performEdgeOperations.SetTestImage(m_OriginalImage);
            
            performEdgeOperations.Run();

            var foundEdges = performEdgeOperations.FoundEdges;

            //m_EdgesBeforeOperations = performEdgeOperations.EdgesBeforeOperations;

            return foundEdges;
        }

        private List<Sine> PerformSineFitting()
        {
            var edgeFitting = 
                new EdgeFit(m_FoundEdges, m_ImageWidth, m_ImageHeight)
                {
                    ImageType = "Borehole"
                };

            //edgeFitting.MaxAmplitude = maxSineAmplitude;
            edgeFitting.FitEdges();

            var sines = edgeFitting.FitSines;

            sines.Sort(new SineDepthComparer());

            return sines;
        }

        private List<EdgeLine> PerformLineFitting()
        {
            var edgeFitting = new EdgeFit(m_FoundEdges, m_ImageWidth, m_ImageHeight)
            {
                ImageType = "Core",
                MaxAmplitude = MaxSineAmplitude
            };
            
            edgeFitting.FitEdges();

            var lines = edgeFitting.FitLines;

            lines.Sort(new LineDepthComparer());

            return lines;
        }

        //private void DeleteSinesFromEdgeData()
        //{
        //    if (m_DrawTestImages)
        //    {
        //        var factory = new DrawEdgesImageFactory("Edge");
        //        var drawImage = factory.setUpDrawEdges(m_EdgesBeforeOperations, m_ImageWidth, m_ImageHeight);

        //        drawImage.setBackgroundColour(m_TestDrawingBackgroundColour);
        //        drawImage.setEdgeColour(m_TestDrawingEdgeColour);
        //        drawImage.SetDrawMultiColouredEdges(m_DrawMultiColouredEdges);

        //        drawImage.drawEdgesImage();
        //        drawImage.getDrawnEdges().Save("07a - All edges before operations.bmp");

        //        drawImage.setEdgeColour(m_TestDrawingOverBackgroundColour);
        //        drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
        //        drawImage.getDrawnEdges().Save("07b - All edges before operations.bmp");
        //    }
        //    foreach (var edge in m_EdgesBeforeOperations)
        //    {
        //        RemoveIfAlreadyUsed(edge);
        //    }

        //    if (m_DrawTestImages)
        //    {
        //        var factory = new DrawEdgesImageFactory("Edge");
        //        var drawImage = factory.setUpDrawEdges(m_EdgesAfterSineFitting, m_ImageWidth, m_ImageHeight);

        //        drawImage.setBackgroundColour(m_TestDrawingBackgroundColour);
        //        drawImage.setEdgeColour(m_TestDrawingEdgeColour);
        //        drawImage.drawEdgesImage();
        //        drawImage.getDrawnEdges().Save("08a - Remaining edges after sine removal.bmp");

        //        drawImage.setEdgeColour(m_TestDrawingOverBackgroundColour);
        //        drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
        //        drawImage.getDrawnEdges().Save("08b - Remaining edges after sine removal.bmp");
        //    }
        //}

        // Checks if this edge is part of any of the Edges used for sinefitting.  If not it keeps the edge
        //private void RemoveIfAlreadyUsed(Edge edge)
        //{
        //    var point = edge.EdgeEnd1;
        //    var edgeUsed = false;

        //    foreach (var foundEdge in m_FoundEdges)
        //    {
        //        if (foundEdge.Points.Contains(point))
        //        {
        //            edgeUsed = true;
        //        }
        //    }

        //    if (edgeUsed == false)
        //    {
        //        m_EdgesAfterSineFitting.Add(edge);
        //    }
        //}
    }
}
