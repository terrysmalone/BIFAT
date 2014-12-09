using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FeatureAnnotationTool.Interfaces;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class OptionsForm : Form
    {
        private IView _view;


        public OptionsForm(IView view)
        {
            this._view = view;
            InitializeComponent();
        }

        private void closeOptionsButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        
        private void newFromOTVOptionsButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            _view.OpenOTVFile();
            this.Close();
        }

        private void openOptionsButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            _view.openExistingFile();
            this.Close();
        }

        private void newBoreholeFromImageOptionsButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            _view.SetProjectType("Borehole");
            _view.OpenImageFile();
            this.Close();
        }

        private void newCoreFromImageProjectButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            _view.SetProjectType("Core");
            _view.OpenImageFile();
            this.Close();
        }
    }
}
