namespace FeatureAnnotationTool.DialogBoxes
{
    partial class DepthLuminosityForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DepthLuminosityForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sampleRateTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.includeItemsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.sampleRateTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(545, 78);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // sampleRateTextBox
            // 
            this.sampleRateTextBox.Location = new System.Drawing.Point(117, 31);
            this.sampleRateTextBox.Name = "sampleRateTextBox";
            this.sampleRateTextBox.Size = new System.Drawing.Size(196, 20);
            this.sampleRateTextBox.TabIndex = 1;
            this.sampleRateTextBox.Text = "10";
            this.sampleRateTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.sampleRateTextBox_KeyDown);
            this.sampleRateTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.sampleRateTextBox_KeyPress);
            this.sampleRateTextBox.Leave += new System.EventHandler(this.sampleRateTextBox_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sample Rate (mm)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.includeItemsCheckedListBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 106);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(545, 197);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Include";
            // 
            // includeItemsCheckedListBox
            // 
            this.includeItemsCheckedListBox.CheckOnClick = true;
            this.includeItemsCheckedListBox.FormattingEnabled = true;
            this.includeItemsCheckedListBox.Items.AddRange(new object[] {
            "Background  (anything not annotated)",
            "All annotated features",
            "   All annotated layers",
            "   All annotated clusters",
            "   All annotated inclusions"});
            this.includeItemsCheckedListBox.Location = new System.Drawing.Point(21, 19);
            this.includeItemsCheckedListBox.Name = "includeItemsCheckedListBox";
            this.includeItemsCheckedListBox.Size = new System.Drawing.Size(507, 154);
            this.includeItemsCheckedListBox.TabIndex = 4;
            this.includeItemsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.includeItemsCheckedListBox_ItemCheck);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(401, 311);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okayButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(482, 311);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // DepthLuminosityForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 341);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DepthLuminosityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Depth/Luminosity Profile";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sampleRateTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckedListBox includeItemsCheckedListBox;
        private System.Windows.Forms.Button cancelButton;
    }
}