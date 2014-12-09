namespace FeatureAnnotationTool.DialogBoxes
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.closeOptionsButton = new System.Windows.Forms.Button();
            this.newFromImageOptionsButton = new System.Windows.Forms.Button();
            this.newFromOTVOptionsButton = new System.Windows.Forms.Button();
            this.openOptionsButton = new System.Windows.Forms.Button();
            this.newCoreFromImageProjectButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // closeOptionsButton
            // 
            this.closeOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeOptionsButton.Location = new System.Drawing.Point(316, 197);
            this.closeOptionsButton.Name = "closeOptionsButton";
            this.closeOptionsButton.Size = new System.Drawing.Size(75, 23);
            this.closeOptionsButton.TabIndex = 0;
            this.closeOptionsButton.Text = "Close";
            this.closeOptionsButton.UseVisualStyleBackColor = true;
            this.closeOptionsButton.Click += new System.EventHandler(this.closeOptionsButton_Click);
            // 
            // newFromImageOptionsButton
            // 
            this.newFromImageOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newFromImageOptionsButton.Location = new System.Drawing.Point(12, 38);
            this.newFromImageOptionsButton.Name = "newFromImageOptionsButton";
            this.newFromImageOptionsButton.Size = new System.Drawing.Size(379, 28);
            this.newFromImageOptionsButton.TabIndex = 1;
            this.newFromImageOptionsButton.Text = "Create new borehole project from image file";
            this.newFromImageOptionsButton.UseVisualStyleBackColor = true;
            this.newFromImageOptionsButton.Click += new System.EventHandler(this.newBoreholeFromImageOptionsButton_Click);
            // 
            // newFromOTVOptionsButton
            // 
            this.newFromOTVOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.newFromOTVOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newFromOTVOptionsButton.Location = new System.Drawing.Point(12, 110);
            this.newFromOTVOptionsButton.Name = "newFromOTVOptionsButton";
            this.newFromOTVOptionsButton.Size = new System.Drawing.Size(379, 28);
            this.newFromOTVOptionsButton.TabIndex = 2;
            this.newFromOTVOptionsButton.Text = "Create new project from .otv file";
            this.newFromOTVOptionsButton.UseVisualStyleBackColor = true;
            this.newFromOTVOptionsButton.Click += new System.EventHandler(this.newFromOTVOptionsButton_Click);
            // 
            // openOptionsButton
            // 
            this.openOptionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openOptionsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openOptionsButton.Location = new System.Drawing.Point(12, 150);
            this.openOptionsButton.Name = "openOptionsButton";
            this.openOptionsButton.Size = new System.Drawing.Size(379, 28);
            this.openOptionsButton.TabIndex = 3;
            this.openOptionsButton.Text = "Open existing project";
            this.openOptionsButton.UseVisualStyleBackColor = true;
            this.openOptionsButton.Click += new System.EventHandler(this.openOptionsButton_Click);
            // 
            // newCoreFromImageProjectButton
            // 
            this.newCoreFromImageProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newCoreFromImageProjectButton.Location = new System.Drawing.Point(12, 76);
            this.newCoreFromImageProjectButton.Name = "newCoreFromImageProjectButton";
            this.newCoreFromImageProjectButton.Size = new System.Drawing.Size(379, 28);
            this.newCoreFromImageProjectButton.TabIndex = 4;
            this.newCoreFromImageProjectButton.Text = "Create new core project from image file";
            this.newCoreFromImageProjectButton.UseVisualStyleBackColor = true;
            this.newCoreFromImageProjectButton.Click += new System.EventHandler(this.newCoreFromImageProjectButton_Click);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.newFromImageOptionsButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 230);
            this.Controls.Add(this.newCoreFromImageProjectButton);
            this.Controls.Add(this.openOptionsButton);
            this.Controls.Add(this.newFromOTVOptionsButton);
            this.Controls.Add(this.newFromImageOptionsButton);
            this.Controls.Add(this.closeOptionsButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Action";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeOptionsButton;
        private System.Windows.Forms.Button newFromImageOptionsButton;
        private System.Windows.Forms.Button newFromOTVOptionsButton;
        private System.Windows.Forms.Button openOptionsButton;
        private System.Windows.Forms.Button newCoreFromImageProjectButton;
    }
}