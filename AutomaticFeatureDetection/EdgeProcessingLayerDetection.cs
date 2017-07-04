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
        private float m_CannyLowThreshold = 0.01f;
        private float m_CannyHighThreshold = 0.016f;
        private int m_GaussianWidth = 20;
        private int m_GaussianAngle = 6;

        private float m_VerticalWeight, m_HorizontalWeight;

        private int m_EdgeLinkingThreshold = 320;
        private int m_EdgeLinkingMaxNeighbourDistance = 5;
        private int m_EdgeRemovalThreshold = 320;
        private int m_EdgeJoiningThreshold = 20;

        private int m_LayerSensitivity = 1;

        private int m_MaxSineAmplitude = 100;

        //Data
        private int m_ImageWidth;
        private int m_ImageHeight;

        private int[] m_ImageData;

        private int m_DepthResolution;

        private readonly int m_AzimuthResolution;
        
        private readonly Bitmap m_OriginalImage;
        private List<Edge> m_FoundEdges = new List<Edge>();
        private List<Edge> m_EdgesBeforeOperations = new List<Edge>();
        private readonly List<Edge> m_EdgesAfterSineFitting = new List<Edge>();

        private List<Sine> m_DetectedFitSines = new List<Sine>();
        private List<EdgeLine> m_DetectedFitLines = new List<EdgeLine>();
        
        //Detected features
        private List<Layer> m_DetectedLayers = new List<Layer>();

        private readonly int m_StartHeight;

        private bool m_DrawTestImages;

        private Color m_TestDrawingBackgroundColour;
        private Color m_TestDrawingEdgeColour;
        private Color m_TestDrawingOverBackgroundColour;

        private readonly LayerTypeSelector m_LayerTypeSelector = new LayerTypeSelector("Borehole");
        
        private bool m_DisableLayerDetection;

        private string m_ImageType = "Borehole";


        private bool m_DrawMultiColouredEdges;

        # region properties

        public List<Layer> DetectedLayers
        {
            get
            {
                return m_DetectedLayers;
            }
        }

        # region Edge detection parameters

        public int StartHeight
        {
            get
            {
                return m_StartHeight;
            }
        }

        public float CannyLow
        {
            get
            {
                return m_CannyLowThreshold;
            }
            set
            {
                m_CannyLowThreshold = value;
            }
        }

        public float CannyHigh
        {
            get
            {
                return m_CannyHighThreshold;
            }
            set
            {
                m_CannyHighThreshold = value;
            }
        }

        public int GaussianWidth
        {
            get
            {
                return m_GaussianWidth;
            }
            set
            {
                m_GaussianWidth = value;
            }
        }

        public int GaussianAngle
        {
            get
            {
                return m_GaussianAngle;
            }
            set
            {
                m_GaussianAngle = value;
            }
        }

        public float VerticalWeight
        {
            get
            {
                return m_VerticalWeight;
            }
            set
            {
                m_VerticalWeight = value;
            }
        }

        public float HorizontalWeight
        {
            get
            {
                return m_HorizontalWeight;
            }
            set
            {
                m_HorizontalWeight = value;
            }
        }

        public int EdgeLinkingThreshold
        {
            get
            {
                return m_EdgeLinkingThreshold;
            }
            set
            {
                m_EdgeLinkingThreshold = value;
            }
        }

        public int EdgeLinkingDistance
        {
            get
            {
                return m_EdgeLinkingMaxNeighbourDistance;
            }
            set
            {
                m_EdgeLinkingMaxNeighbourDistance = value;
            }
        }

        public int RemoveEdgeThreshold
        {
            get
            {
                return m_EdgeRemovalThreshold;
            }
            set
            {
                m_EdgeRemovalThreshold = value;
            }
        }

        public int JoinEdgeThreshold
        {
            get
            {
                return m_EdgeJoiningThreshold;
            }
            set
            {
                m_EdgeJoiningThreshold = value;
            }
        }

        public int LayerSensitivity
        {
            get
            {
                return m_LayerSensitivity;
            }
            set
            {
                m_LayerSensitivity = value;
            }
        }

        public bool DisableLayerDetection
        {
            get
            {
                return m_DisableLayerDetection;
            }
            set
            {
                m_DisableLayerDetection = value;
            }
        }

        public int MaxSineAmplitude
        {
            get
            {
                return m_MaxSineAmplitude;
            }
            set
            {
                m_MaxSineAmplitude = value;
            }
        }
        
        # endregion

        public string ImageType
        {
            get
            {
                return m_ImageType;
            }
            set
            {
                m_ImageType = value;
            }
        }

        public int DepthResolution
        {
            get
            {
                return m_DepthResolution;
            }
            set
            {
                m_DepthResolution = value;
            }
        }

        # region Test properties

        public bool DrawTestImages
        {
            get
            {
                return m_DrawTestImages;
            }
            set
            {
                m_DrawTestImages = value;
            }
        }

        public Color TestDrawingBackgroundColour
        {
            get
            {
                return m_TestDrawingBackgroundColour;
            }
            set
            {
                m_TestDrawingBackgroundColour = value;
            }
        }

        public Color TestDrawingEdgeColour
        {
            get
            {
                return m_TestDrawingEdgeColour;
            }
            set
            {
                m_TestDrawingEdgeColour = value;
            }
        }

        public Color TestDrawingOverBackgroundColour
        {
            get
            {
                return m_TestDrawingOverBackgroundColour;
            }
            set
            {
                m_TestDrawingOverBackgroundColour = value;
            }
        }

        public bool TestDrawMultiColouredEdges
        {
            get
            {
                return m_DrawMultiColouredEdges;
            }
            set
            {
                m_DrawMultiColouredEdges = value;
            }
        }
        
        # endregion

        # endregion

        /// <summary>
        /// Constructor method
        /// </summary>
        public EdgeProcessingLayerDetection(Bitmap originalImage, int startHeight)
        {
            var initializeTime = DateTime.Now;
            Console.WriteLine("Initialising at: " + initializeTime);

            this.m_OriginalImage = originalImage;
            this.m_StartHeight = startHeight;

            m_AzimuthResolution = originalImage.Width;
            m_DepthResolution = 1;

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

            if (m_ImageType == "Borehole")
            {
                m_DetectedFitSines = PerformSineFitting();

                if (m_DrawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(m_OriginalImage, m_DetectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (m_DisableLayerDetection == false)
                {
                    DetectLayers detectLayers = new DetectLayers(m_DetectedFitSines, m_OriginalImage, m_DepthResolution);

                    detectLayers.LayerBrightnessSensitivity = m_LayerSensitivity;

                    detectLayers.Run();

                    m_DetectedLayers = detectLayers.DetectedLayers;

                    //detectedLayers.Sort(new LayerDepthComparer());
                }
                else
                {
                    Layer currentLayerToAdd;
                    Sine currentSineToAdd;

                    for (int sinePlace = 0; sinePlace < m_DetectedFitSines.Count; sinePlace++)
                    {
                        currentSineToAdd = m_DetectedFitSines[sinePlace];

                        currentLayerToAdd = m_LayerTypeSelector.setUpLayer(currentSineToAdd.Depth + m_StartHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, currentSineToAdd.Depth + m_StartHeight, currentSineToAdd.Amplitude, currentSineToAdd.Azimuth, m_AzimuthResolution, m_DepthResolution);

                        m_DetectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
            else
            {
                m_DetectedFitLines = PerformLineFitting();

                if (m_DrawTestImages)
                {
                    DrawSinesImage drawSines = new DrawSinesImage(m_OriginalImage, m_DetectedFitSines);
                    drawSines.DrawnImage.Save("06 - Image with fit sines.bmp");
                }

                //Layer detection
                if (m_DisableLayerDetection == false)
                {
                    DetectLayers detectLayers = new DetectLayers(m_DetectedFitLines, m_OriginalImage, m_DepthResolution);

                    detectLayers.LayerBrightnessSensitivity = m_LayerSensitivity;

                    detectLayers.Run();

                    m_DetectedLayers = detectLayers.DetectedLayers;
                }
                else
                {
                    Layer currentLayerToAdd;
                    EdgeLine currentLineToAdd;

                    for (int linePlace = 0; linePlace < m_DetectedFitLines.Count; linePlace++)
                    {
                        currentLineToAdd = m_DetectedFitLines[linePlace];

                        currentLayerToAdd = m_LayerTypeSelector.setUpLayer(currentLineToAdd.Slope, currentLineToAdd.Intercept + m_StartHeight, currentLineToAdd.Slope, currentLineToAdd.Intercept + m_StartHeight, m_AzimuthResolution, m_DepthResolution);

                        m_DetectedLayers.Add(currentLayerToAdd);
                    }
                }
            }
        }

        private bool[] PerformCannyEdgeDetection()
        {
            CannyDetector detector = new CannyDetector(m_ImageData, m_ImageWidth, m_ImageHeight);

            detector.WrapHorizontally = true;
            detector.WrapVertically = true;

            detector.LowThreshold = m_CannyLowThreshold;
            detector.HighThreshold = m_CannyHighThreshold;
            detector.GaussianWidth = m_GaussianWidth;
            detector.GaussianSigma = m_GaussianAngle;

            detector.HorizontalWeight = m_HorizontalWeight;
            detector.VerticalWeight = m_VerticalWeight;

            detector.DetectEdges();

            bool[] edgeData = detector.GetEdgeData();

            if (m_DrawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Bool");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(edgeData, m_ImageWidth, m_ImageHeight);

                drawImage.setBackgroundColour(m_TestDrawingBackgroundColour);
                drawImage.setEdgeColour(m_TestDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(m_DrawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("01a - Canny(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");

                drawImage.setEdgeColour(m_TestDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
                drawImage.getDrawnEdges().Save("01b - Canny over original(" + detector.LowThreshold + "," + detector.HighThreshold + "," + detector.GaussianWidth + ", " + (int)detector.GaussianSigma + ").bmp");
            }

            return edgeData;
        }

        private List<Edge> PerformEdgeOperations(bool[] cannyData)
        {
            List<Edge> foundEdges = new List<Edge>();

            EdgeOperations performEdgeOperations = new EdgeOperations(cannyData, m_ImageWidth, m_ImageHeight);
            performEdgeOperations.DrawTestImages = m_DrawTestImages;

            if (m_DrawTestImages)
                performEdgeOperations.SetTestImage(m_OriginalImage);
            
            performEdgeOperations.EdgeLinkingThreshold = m_EdgeLinkingThreshold;
            performEdgeOperations.EdgeLinkingDistance = m_EdgeLinkingMaxNeighbourDistance;

            performEdgeOperations.EdgeRemovalThreshold = m_EdgeRemovalThreshold;
            performEdgeOperations.EdgeJoiningThreshold = m_EdgeJoiningThreshold;

            performEdgeOperations.TestDrawingBackgroundColour = m_TestDrawingBackgroundColour;
            performEdgeOperations.TestDrawingEdgeColour = m_TestDrawingEdgeColour;
            performEdgeOperations.TestDrawingOverBackgroundColour = m_TestDrawingOverBackgroundColour;
            performEdgeOperations.TestDrawMultiColouredEdges = m_DrawMultiColouredEdges;

            performEdgeOperations.Run();

            foundEdges = performEdgeOperations.FoundEdges;
            m_EdgesBeforeOperations = performEdgeOperations.EdgesBeforeOperations;

            return foundEdges;
        }

        private List<Sine> PerformSineFitting()
        {
            EdgeFit edgeFitting = new EdgeFit(m_FoundEdges, m_ImageWidth, m_ImageHeight);
            edgeFitting.ImageType = "Borehole";

            //edgeFitting.MaxAmplitude = maxSineAmplitude;
            edgeFitting.FitEdges();

            List<Sine> sines = edgeFitting.FitSines;

            sines.Sort(new SineDepthComparer());

            return sines;
        }

        private List<EdgeLine> PerformLineFitting()
        {
            EdgeFit edgeFitting = new EdgeFit(m_FoundEdges, m_ImageWidth, m_ImageHeight);
            edgeFitting.ImageType = "Core";

            edgeFitting.MaxAmplitude = m_MaxSineAmplitude;
            edgeFitting.FitEdges();

            List<EdgeLine> lines = edgeFitting.FitLines;

            lines.Sort(new LineDepthComparer());

            return lines;
        }

        private void deleteSinesFromEdgeData()
        {
            if (m_DrawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(m_EdgesBeforeOperations, m_ImageWidth, m_ImageHeight);

                drawImage.setBackgroundColour(m_TestDrawingBackgroundColour);
                drawImage.setEdgeColour(m_TestDrawingEdgeColour);
                drawImage.SetDrawMultiColouredEdges(m_DrawMultiColouredEdges);

                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("07a - All edges before operations.bmp");

                drawImage.setEdgeColour(m_TestDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
                drawImage.getDrawnEdges().Save("07b - All edges before operations.bmp");
            }

            for (int i = 0; i < m_EdgesBeforeOperations.Count; i++)
            {
                removeIfAlreadyUsed(m_EdgesBeforeOperations[i]);
            }

            if (m_DrawTestImages)
            {
                DrawEdgesImageFactory factory = new DrawEdgesImageFactory("Edge");
                DrawEdgesImage drawImage = factory.setUpDrawEdges(m_EdgesAfterSineFitting, m_ImageWidth, m_ImageHeight);

                drawImage.setBackgroundColour(m_TestDrawingBackgroundColour);
                drawImage.setEdgeColour(m_TestDrawingEdgeColour);
                drawImage.drawEdgesImage();
                drawImage.getDrawnEdges().Save("08a - Remaining edges after sine removal.bmp");

                drawImage.setEdgeColour(m_TestDrawingOverBackgroundColour);
                drawImage.drawEdgesOverBackgroundImage(m_OriginalImage);
                drawImage.getDrawnEdges().Save("08b - Remaining edges after sine removal.bmp");
            }
        }

        /// <summary>
        /// Checks if this edge is part of any of the Edges used for sinefitting.  If not it keeps the edge
        /// </summary>
        /// <param name="point"></param>
        private void removeIfAlreadyUsed(Edge edge)
        {
            Point point = edge.EdgeEnd1;
            bool edgeUsed = false;

            for (int i = 0; i < m_FoundEdges.Count; i++)
            {
                if (m_FoundEdges[i].Points.Contains(point))
                    edgeUsed = true;
            }

            if (edgeUsed == false)
                m_EdgesAfterSineFitting.Add(edge);
        }
    }
}
