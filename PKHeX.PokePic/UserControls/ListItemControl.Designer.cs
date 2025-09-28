namespace PKHeX.PokePic
{
    partial class ListItemControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblText = new Label();
            pictureBoxAlert = new PictureBox();
            pictureBoxLoading = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxAlert).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLoading).BeginInit();
            SuspendLayout();
            // 
            // lblText
            // 
            lblText.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblText.AutoSize = true;
            lblText.Location = new Point(5, 5);
            lblText.Margin = new Padding(0);
            lblText.Name = "lblText";
            lblText.Size = new Size(29, 15);
            lblText.TabIndex = 0;
            lblText.Text = "Title";
            // 
            // pictureBoxAlert
            // 
            pictureBoxAlert.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBoxAlert.Location = new Point(131, 5);
            pictureBoxAlert.Name = "pictureBoxAlert";
            pictureBoxAlert.Size = new Size(16, 16);
            pictureBoxAlert.TabIndex = 1;
            pictureBoxAlert.TabStop = false;
            // 
            // pictureBoxLoading
            // 
            pictureBoxLoading.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pictureBoxLoading.Image = Properties.Resources.loading;
            pictureBoxLoading.Location = new Point(109, 5);
            pictureBoxLoading.Name = "pictureBoxLoading";
            pictureBoxLoading.Size = new Size(16, 16);
            pictureBoxLoading.TabIndex = 2;
            pictureBoxLoading.TabStop = false;
            // 
            // ListItemControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pictureBoxLoading);
            Controls.Add(pictureBoxAlert);
            Controls.Add(lblText);
            Name = "ListItemControl";
            Size = new Size(150, 25);
            ((System.ComponentModel.ISupportInitialize)pictureBoxAlert).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLoading).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblText;
        private PictureBox pictureBoxAlert;
        private PictureBox pictureBoxLoading;
    }
}
