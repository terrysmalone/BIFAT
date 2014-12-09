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
    public partial class LayerPropertiesOptionsForm : Form
    {
        private List<string> selectedItems;
        private bool okayClicked;
        private string imageType;

        public LayerPropertiesOptionsForm(string imageType)
        {
            this.imageType = imageType;

            InitializeComponent();

            AddAllProperties();

            CheckAllProperties();

            selectedItems = new List<string>();
        }

        private void AddAllProperties()
        {
            Object[] items;

            if(imageType == "Borehole")
                items = new Object[] { "Start depth (mm)", "End depth (mm)", "Top sine depth (mm)", "Top sine azimuth", "Top sine amplitude (mm)", "Bottom sine depth (mm)", "Bottom sine azimuth", "Bottom sine amplitude (mm)", "Group", "Layer type", "Layer description", "Layer quality", "Mean layer brightness"};
            else
                items = new Object[] { "Start depth (mm)", "End depth (mm)", "Top edge intercept (mm)", "Top edge slope", "Bottom edge intercept (mm)", "Bottom edge slope", "Group", "Layer type", "Layer description", "Layer quality", "Mean layer brightness" };
            

            propertiesCheckedListBox.Items.Clear();
            propertiesCheckedListBox.Items.AddRange(items);
        }

        private void CheckAllProperties()
        {
            for (int i = 0; i < propertiesCheckedListBox.Items.Count; i++)
            {
                propertiesCheckedListBox.SetItemChecked(i, true);
            }
        }


        public List<string> GetSelectedItems()
        {
            return selectedItems;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void doneButton_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < propertiesCheckedListBox.CheckedItems.Count; i++)
            {
                selectedItems.Add(propertiesCheckedListBox.CheckedItems[i].ToString());
            }

            okayClicked = true;
            this.Visible = false;
        }

        public bool IsOkayClicked()
        {
            return okayClicked;
        }
    }
}
