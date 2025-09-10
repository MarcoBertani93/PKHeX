namespace PKHeX.PokePic
{
    partial class ExportForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportForm));
            SaveButton = new Button();
            ConfigList = new ListBox();
            PreviewBox = new PictureBox();
            listPanel = new Panel();
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
            ConfigList.Size = new Size(169, 304);
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
            // listPanel
            // 
            listPanel.AutoScroll = true;
            listPanel.BackColor = SystemColors.Window;
            listPanel.BorderStyle = BorderStyle.FixedSingle;
            listPanel.Location = new Point(547, 12);
            listPanel.Name = "listPanel";
            listPanel.Size = new Size(199, 304);
            listPanel.TabIndex = 5;
            // 
            // ExportForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(758, 361);
            Controls.Add(listPanel);
            Controls.Add(PreviewBox);
            Controls.Add(ConfigList);
            Controls.Add(SaveButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ExportForm";
            Text = "PokePic_Selector";
            FormClosed += ExportForm_FormClosed;
            ((System.ComponentModel.ISupportInitialize)PreviewBox).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.ListBox ConfigList;
        private PictureBox PreviewBox;
        private Panel listPanel;
    }
}
