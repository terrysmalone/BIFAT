using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightnessAnalysis;

namespace FeatureAnnotationTool.DialogBoxes
{
    public partial class DepthLuminosityForm : Form
    {
        private bool nonNumberEntered = false;
        private bool okClicked = false;

        private List<string> groupnames = new List<string>();

        private int sampleRate = 10, previousSampleRate = 10;
        
        public DepthLuminosityForm()
        {
            InitializeComponent();

            CheckAll();
        }

        # region Textbox methods

        private void sampleRateTextBox_Leave(object sender, EventArgs e)
        {
            try
            {
                if (sampleRateTextBox.Text.Length < 1)
                    throw new Exception();

                if (NonNumberEntered(sampleRateTextBox.Text))
                    throw new Exception();

                updateFields();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Value must be an integer.", "Incorrect value entered");
                sampleRate  = previousSampleRate;
            }
        }

        private bool NonNumberEntered(string text)
        {
            int num;
            if (Int32.TryParse(text, out num))
            {
                return false;
            }
            else
                return true;
        }

        private void updateFields()
        {
            sampleRate = System.Convert.ToInt32(sampleRateTextBox.Text);
            previousSampleRate = sampleRate;
        }

        # endregion

        # region Button clicks

        private void okayButton_Click(object sender, EventArgs e)
        {
            okClicked = true;
            this.Visible = false;            
        }

        # endregion

        # region Set methods
        
        public void SetGroupsList(List<string> groupnames)
        {
            this.groupnames = groupnames;

            for(int i=groupnames.Count-1; i>=0; i--)
            {    
                includeItemsCheckedListBox.Items.Insert(3, "      " + groupnames[i]);
            }

            CheckAll();
        }

        # endregion

        private void CheckAll()
        {
            for (int i = 0; i < includeItemsCheckedListBox.Items.Count; i++)
            {
                includeItemsCheckedListBox.SetItemChecked(i, true);
            }
        }

        # region Get methods

        public bool GetAllFeaturesChecked()
        {
            return includeItemsCheckedListBox.CheckedItems.Contains("All annotated features"); 
        }

        public bool GetAllLayersChecked()
        {
            return includeItemsCheckedListBox.CheckedItems.Contains("   All annotated layers");
        }

        public bool GetAllClustersChecked()
        {
            return includeItemsCheckedListBox.CheckedItems.Contains("   All annotated clusters");
        }

        public bool GetAllInclusionsChecked()
        {
            return includeItemsCheckedListBox.CheckedItems.Contains("   All annotated inclusions");
        }

        public List<string> GetCheckedGroups()
        {
            List<string> checkedGroups = new List<string>();

            for (int i = 0; i < groupnames.Count; i++)
            {
                if (includeItemsCheckedListBox.CheckedItems.Contains("      " + groupnames[i]))
                {
                    checkedGroups.Add(groupnames[i]);
                }
            }

            return checkedGroups;
        }

        public int getSamplingRate()
        {
            return sampleRate;
        }

        # endregion

        private void includeItemsCheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            object one = includeItemsCheckedListBox.Items[e.Index];
            object two = includeItemsCheckedListBox.SelectedItem;
            if (one == two)
            {
                //All annotated features clicked
                if (includeItemsCheckedListBox.SelectedItem == "All annotated features")
                {
                    int numOfItems = includeItemsCheckedListBox.Items.Count;

                    if (e.NewValue == CheckState.Checked)
                    {
                        for (int i = 2; i < numOfItems; i++)
                        {
                            includeItemsCheckedListBox.SetItemChecked(i, true);
                        }

                    }
                    else
                    {
                        for (int i = 2; i < numOfItems; i++)
                        {
                            includeItemsCheckedListBox.SetItemChecked(i, false);
                        }
                    }
                }
                else if (includeItemsCheckedListBox.SelectedItem == "   All annotated layers")
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        for (int i = 3; i < 3 + groupnames.Count; i++)
                        {
                            includeItemsCheckedListBox.SetItemChecked(i, true);
                        }

                        int exception = e.Index;
                        if (EverythingSelected(exception))
                            includeItemsCheckedListBox.SetItemChecked(1, true);
                    }
                    else
                    {
                        for (int i = 3; i < 3 + groupnames.Count; i++)
                        {
                            includeItemsCheckedListBox.SetItemChecked(i, false);
                        }

                        includeItemsCheckedListBox.SetItemChecked(1, false);

                        
                    }
                }
                else if (includeItemsCheckedListBox.SelectedItem == "   All annotated clusters")
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        int exception = e.Index;
                        if (EverythingSelected(exception))
                            includeItemsCheckedListBox.SetItemChecked(1, true);
                    }
                    else
                        includeItemsCheckedListBox.SetItemChecked(1, false);
                }
                else if (includeItemsCheckedListBox.SelectedItem == "   All annotated inclusions")
                {
                    if (e.NewValue == CheckState.Checked)
                    {
                        int exception = e.Index;
                        if (EverythingSelected(exception))
                            includeItemsCheckedListBox.SetItemChecked(1, true);
                    }
                    else
                        includeItemsCheckedListBox.SetItemChecked(1, false);
                }
                else if (includeItemsCheckedListBox.SelectedItem == "Background  (anything not annotated)")
                {

                }
                else
                {
                    int exception = e.Index;

                    //One of the layer groups are selected
                    if (e.NewValue == CheckState.Checked)
                    {
                        if (EverythingSelected(exception, 2))
                        {
                            includeItemsCheckedListBox.SetItemChecked(1, true);
                        }

                        if (AllLayerGroupsChecked(exception - 3))     //if (AllLayerGroupsChecked(exception-2))
                        {
                            includeItemsCheckedListBox.SetItemChecked(2, true);
                        }
                    }
                    else
                    {
                        includeItemsCheckedListBox.SetItemChecked(1, false);
                        includeItemsCheckedListBox.SetItemChecked(2, false);
                    }
                }
            }
        }

        private bool AllLayerGroupsChecked(int exception)
        {
            bool allChecked = true;

            for (int i = 0; i < groupnames.Count; i++)
            {
                if (!GetCheckedGroups().Contains(groupnames[i]) && i != exception)
                {
                    allChecked = false;
                    return allChecked;
                }
            }

            return allChecked;
        }

        private bool EverythingSelected(int exception)
        {
            int numOfItems = includeItemsCheckedListBox.Items.Count;
            bool allChecked = true;

            for (int i = 2; i < numOfItems; i++)
            {
                if(!includeItemsCheckedListBox.CheckedIndices.Contains(i) && i != exception)
                {
                    allChecked = false;
                    return allChecked;
                }
            }

            return allChecked;
        }

        private bool EverythingSelected(int exception1, int exception2)
        {
            int numOfItems = includeItemsCheckedListBox.Items.Count;
            bool allChecked = true;

            for (int i = 2; i < numOfItems; i++)
            {
                if (!includeItemsCheckedListBox.CheckedIndices.Contains(i) && i != exception1 && i != exception2)
                {
                    allChecked = false;
                    return allChecked;
                }
            }

            return allChecked;
        }

        private void sampleRateTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                e.Handled = true;
            }
        }

        private void sampleRateTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            nonNumberEntered = IsKeyNonNumber(e.KeyCode);

            //if (e.KeyCode == Keys.Subtract)
            //{
            //    if (sampleRateTextBox.Text.Length == 0 || sampleRateTextBox.SelectionLength == sampleRateTextBox.Text.Length || (sampleRateTextBox.SelectionStart == 0 && (sampleRateTextBox.Text[0] != '-' || sampleRateTextBox.SelectionLength > 0)))
            //        nonNumberEntered = false;
            //}

            if (e.Shift)
                nonNumberEntered = true;
        }

        private bool IsKeyNonNumber(Keys key)
        {
            nonNumberEntered = false;

            if (key == Keys.Subtract || key == Keys.OemMinus)
            {
                nonNumberEntered = true;
            }
            else if (key < Keys.D0 || key > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (key < Keys.NumPad0 || key > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (key != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }

            return nonNumberEntered;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        public bool IsOKClicked()
        {
            return okClicked;
        }

        public bool GetIncludeBackground()
        {
            bool backgroundIncluded = includeItemsCheckedListBox.CheckedItems.Contains("Background  (anything not annotated)");
            
            return backgroundIncluded;
        }
    }
}
