namespace FeatureAnnotationTool.DialogBoxes
{
    partial class SectionSizeDialogBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SectionSizeDialogBox));
            this.sectionSizeTrackbar = new System.Windows.Forms.TrackBar();
            this.sectionSizeTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numberOfBitmapsTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.sectionSizeTrackbar)).BeginInit();
            this.SuspendLayout();
            // 
            // sectionSizeTrackbar
            // 
            this.sectionSizeTrackbar.LargeChange = 10000;
            this.sectionSizeTrackbar.Location = new System.Drawing.Point(199, 45);
            this.sectionSizeTrackbar.Name = "sectionSizeTrackbar";
            this.sectionSizeTrackbar.Size = new System.Drawing.Size(299, 45);
            this.sectionSizeTrackbar.SmallChange = 10000;
            this.sectionSizeTrackbar.TabIndex = 0;
            this.sectionSizeTrackbar.TickFrequency = 10000;
            this.sectionSizeTrackbar.Scroll += new System.EventHandler(this.sectionSizeTrackbar_Scroll);
            // 
            // sectionSizeTextBox
            // 
            this.sectionSizeTextBox.Enabled = false;
            this.sectionSizeTextBox.Location = new System.Drawing.Point(116, 45);
            this.sectionSizeTextBox.Name = "sectionSizeTextBox";
            this.sectionSizeTextBox.Size = new System.Drawing.Size(65, 20);
            this.sectionSizeTextBox.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(423, 99);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Done";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Height (pixels)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Split into separate bitmaps";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Number of bitmaps";
            // 
            // numberOfBitmapsTextBox
            // 
            this.numberOfBitmapsTextBox.Enabled = false;
            this.numberOfBitmapsTextBox.Location = new System.Drawing.Point(116, 83);
            this.numberOfBitmapsTextBox.Name = "numberOfBitmapsTextBox";
            this.numberOfBitmapsTextBox.Size = new System.Drawing.Size(65, 20);
            this.numberOfBitmapsTextBox.TabIndex = 6;
            // 
            // SectionSizeDialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 133);
            this.Controls.Add(this.numberOfBitmapsTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.sectionSizeTextBox);
            this.Controls.Add(this.sectionSizeTrackbar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SectionSizeDialogBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SectionSizeDialogBox";
            ((System.ComponentModel.ISupportInitialize)(this.sectionSizeTrackbar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar sectionSizeTrackbar;
        private System.Windows.Forms.TextBox sectionSizeTextBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox numberOfBitmapsTextBox;
    }
}