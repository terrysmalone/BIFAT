using System;

namespace BoreholeFeatures
{
    internal sealed class DepthMeasurementHandler
    {
        private int m_SourceStartDepthInMillimetres;
        private int m_DepthResolution;

        private int m_StartDepthInPixels;
        private int m_EndDepthInPixels;

        #region properties
        
        internal int SourceStartDepthInMillimetres
        {
            get { return m_SourceStartDepthInMillimetres; }

            set
            {
                m_SourceStartDepthInMillimetres = value;

                UpdateDepths();
            }
        }

        internal int DepthResolution
        {
            get { return m_DepthResolution; }

            set
            {
                m_DepthResolution = value;

                UpdateDepths();
            }
        }

        internal int StartDepthInPixels
        {
            get { return m_StartDepthInPixels; }

            set
            {
                m_StartDepthInPixels = value;

                UpdateDepths();
            }
        }

        internal int EndDepthInPixels
        {
            get { return m_EndDepthInPixels; }

            set
            {
                m_EndDepthInPixels = value;

                UpdateDepths();
            }
        }

        internal int StartDepthInMillimetres { get; private set; }
        
        internal int EndDepthInMillimetres { get; private set; }
        
        #endregion properties

        internal DepthMeasurementHandler(int depthResolution)
        {
            m_DepthResolution = depthResolution;

            UpdateDepths();
        }

        private void UpdateDepths()
        {
            StartDepthInMillimetres = Convert.ToInt32(m_SourceStartDepthInMillimetres 
                                                      + m_StartDepthInPixels 
                                                      * (double)m_DepthResolution);

            EndDepthInMillimetres = Convert.ToInt32(m_SourceStartDepthInMillimetres 
                                                      + m_EndDepthInPixels 
                                                      * (double)m_DepthResolution);


            
        }
    }
}
