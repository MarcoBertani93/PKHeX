namespace PKHeX.PokePic
{
    partial class PokePic_Selector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokePic_Selector));
            SaveButton = new Button();
            ConfigList = new ListBox();
            PreviewBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)PreviewBox).BeginInit();
            SuspendLayout();
            // 
            // SaveButton
            // 
            SaveButton.AccessibleName = "SaveButton";
            SaveButton.Location = new Point(497, 326);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 2;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // ConfigList
            // 
            ConfigList.AccessibleName = "Config List";
            ConfigList.FormattingEnabled = true;
            ConfigList.Location = new Point(372, 12);
            ConfigList.Name = "ConfigList";
            ConfigList.Size = new Size(200, 304);
            ConfigList.TabIndex = 3;
            // 
            // PreviewBox
            // 
            PreviewBox.AccessibleName = "Preview Box";
            PreviewBox.Location = new Point(12, 12);
            PreviewBox.Name = "PreviewBox";
            PreviewBox.Size = new Size(354, 304);
            PreviewBox.TabIndex = 4;
            PreviewBox.TabStop = false;
            // 
            // PokePic_Selector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(PreviewBox);
            Controls.Add(ConfigList);
            Controls.Add(SaveButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PokePic_Selector";
            Text = "PokePic_Selector";
            ((System.ComponentModel.ISupportInitialize)PreviewBox).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ListBox ConfigList;
        private PictureBox PreviewBox;
    }
}
