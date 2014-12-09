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
    public partial class ClusterPropertiesOptionsForm : Form
    {
        private List<string> selectedItems;
        private bool okayClicked;
        
        public ClusterPropertiesOptionsForm()
        {
            InitializeComponent();

            CheckAllProperties();

            selectedItems = new List<string>();
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
