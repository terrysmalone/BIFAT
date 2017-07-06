using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace BoreholeFeatures
{
    internal sealed class MeasurementTranslator
    {
        private int m_MeasurementResolution;
        private int m_MeasurementInPixels;
        private int m_StartMeasurementInMillimetres;
        
        internal int MeasurementResolution
        {
            get => m_MeasurementResolution;

            set
            {
                m_MeasurementResolution = value;

                UpdateMeasurement();
            }
        }

        internal int MeasurementInPixels
        {
            get => m_MeasurementResolution;

            set
            {
                m_MeasurementInPixels = value;

                UpdateMeasurement();
            }
        }

        internal int StartMeasurementInMillimetres
        {
            get => m_StartMeasurementInMillimetres;

            set
            {
                m_StartMeasurementInMillimetres = value;

                UpdateMeasurement();
            }
        }

        public int MeasurementInMillimetres { get; private set; }
        
        internal MeasurementTranslator(int depthInPixels, 
                                     int sourceStartMeasurementInMillimetres, 
                                     int measurementResolution)
        {
            m_MeasurementResolution = measurementResolution;
            m_MeasurementInPixels = depthInPixels;
            m_StartMeasurementInMillimetres = sourceStartMeasurementInMillimetres;

            UpdateMeasurement();
        }

        private void UpdateMeasurement()
        {
            
            MeasurementInMillimetres = Convert.ToInt32(m_StartMeasurementInMillimetres 
                                                       + m_MeasurementInPixels 
                                                       * (double)m_MeasurementResolution);
        }
    }
}
