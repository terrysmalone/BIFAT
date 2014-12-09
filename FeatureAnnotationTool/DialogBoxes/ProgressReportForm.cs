using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class ProgressReportForm : Form
    {
        public ProgressReportForm(string title)
        {
            InitializeComponent();

            this.Text = title;
        }

        public void Begin()
        {
            

            
        }

        private void ProgressReportForm_Activated(object sender, EventArgs e)
        {
            if (Application.RenderWithVisualStyles)
                progressBar.Style = ProgressBarStyle.Marquee;
            else
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Maximum = 100;
                progressBar.Value = 0;


                int count = 0;

                while (true)
                {
                    progressBar.Value = count;

                    if (count >= 100)
                        count = 0;

                    count++;
                }
            }
        }

        /**public void UpdateProgress(int progress)
        {
            this.progress = progress;

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateLabel));
            }
            else
            {
                UpdateLabel();
            }
        }

        public void UpdateLabel()
        {
            if (progress == 100)
                this.Dispose();
            else
                progressLabel.Text = progress.ToString();
        }**/
    }
}
