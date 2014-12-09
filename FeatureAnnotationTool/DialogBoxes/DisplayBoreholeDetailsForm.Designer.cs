namespace FeatureAnnotationTool.DialogBoxes
{
    partial class DisplayDetailsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayDetailsForm));
            this.nameLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.depthLabel = new System.Windows.Forms.Label();
            this.depthResolutionTextBox = new System.Windows.Forms.TextBox();
            this.azimuthResolutionTextBox = new System.Windows.Forms.TextBox();
            this.boreholeDepthTextBox = new System.Windows.Forms.TextBox();
            this.startDepthTextBox = new System.Windows.Forms.TextBox();
            this.endDepthTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(30, 27);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(81, 13);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "Borehole name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Depth resolution:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Azimuth resolution:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Start depth (mm):";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "End depth (mm):";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(408, 254);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(327, 254);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 7;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Visible = false;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Enabled = false;
            this.nameTextBox.Location = new System.Drawing.Point(142, 23);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(339, 20);
            this.nameTextBox.TabIndex = 8;
            // 
            // depthLabel
            // 
            this.depthLabel.AutoSize = true;
            this.depthLabel.Location = new System.Drawing.Point(30, 123);
            this.depthLabel.Name = "depthLabel";
            this.depthLabel.Size = new System.Drawing.Size(107, 13);
            this.depthLabel.TabIndex = 9;
            this.depthLabel.Text = "Borehole depth (mm):";
            // 
            // depthResolutionTextBox
            // 
            this.depthResolutionTextBox.Enabled = false;
            this.depthResolutionTextBox.Location = new System.Drawing.Point(142, 55);
            this.depthResolutionTextBox.Name = "depthResolutionTextBox";
            this.depthResolutionTextBox.Size = new System.Drawing.Size(339, 20);
            this.depthResolutionTextBox.TabIndex = 11;
            // 
            // azimuthResolutionTextBox
            // 
            this.azimuthResolutionTextBox.Enabled = false;
            this.azimuthResolutionTextBox.Location = new System.Drawing.Point(142, 87);
            this.azimuthResolutionTextBox.Name = "azimuthResolutionTextBox";
            this.azimuthResolutionTextBox.Size = new System.Drawing.Size(339, 20);
            this.azimuthResolutionTextBox.TabIndex = 12;
            // 
            // boreholeDepthTextBox
            // 
            this.boreholeDepthTextBox.Enabled = false;
            this.boreholeDepthTextBox.Location = new System.Drawing.Point(142, 119);
            this.boreholeDepthTextBox.Name = "boreholeDepthTextBox";
            this.boreholeDepthTextBox.Size = new System.Drawing.Size(339, 20);
            this.boreholeDepthTextBox.TabIndex = 13;
            // 
            // startDepthTextBox
            // 
            this.startDepthTextBox.Enabled = false;
            this.startDepthTextBox.Location = new System.Drawing.Point(142, 151);
            this.startDepthTextBox.Name = "startDepthTextBox";
            this.startDepthTextBox.Size = new System.Drawing.Size(339, 20);
            this.startDepthTextBox.TabIndex = 14;
            // 
            // endDepthTextBox
            // 
            this.endDepthTextBox.Enabled = false;
            this.endDepthTextBox.Location = new System.Drawing.Point(142, 183);
            this.endDepthTextBox.Name = "endDepthTextBox";
            this.endDepthTextBox.Size = new System.Drawing.Size(339, 20);
            this.endDepthTextBox.TabIndex = 15;
            // 
            // DisplayDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(491, 285);
            this.Controls.Add(this.endDepthTextBox);
            this.Controls.Add(this.startDepthTextBox);
            this.Controls.Add(this.boreholeDepthTextBox);
            this.Controls.Add(this.azimuthResolutionTextBox);
            this.Controls.Add(this.depthResolutionTextBox);
            this.Controls.Add(this.depthLabel);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DisplayDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Borehole Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label depthLabel;
        private System.Windows.Forms.TextBox depthResolutionTextBox;
        private System.Windows.Forms.TextBox azimuthResolutionTextBox;
        private System.Windows.Forms.TextBox boreholeDepthTextBox;
        private System.Windows.Forms.TextBox startDepthTextBox;
        private System.Windows.Forms.TextBox endDepthTextBox;
    }
}