namespace BounceClient
{
    partial class WelcomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeForm));
            this.helloLabel = new System.Windows.Forms.Label();
            this.usernameHelpTextlabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.beginButton = new System.Windows.Forms.Button();
            this.statusPictureBox = new System.Windows.Forms.PictureBox();
            this.subheadLabel = new System.Windows.Forms.Label();
            this.licenseLabel = new System.Windows.Forms.Label();
            this.licenseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.checkStatusLabel = new System.Windows.Forms.Label();
            this.checkWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // helloLabel
            // 
            this.helloLabel.AutoSize = true;
            this.helloLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helloLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.helloLabel.Location = new System.Drawing.Point(7, 21);
            this.helloLabel.Name = "helloLabel";
            this.helloLabel.Size = new System.Drawing.Size(104, 37);
            this.helloLabel.TabIndex = 1;
            this.helloLabel.Text = "Hello.";
            // 
            // usernameHelpTextlabel
            // 
            this.usernameHelpTextlabel.AutoSize = true;
            this.usernameHelpTextlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameHelpTextlabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.usernameHelpTextlabel.Location = new System.Drawing.Point(10, 174);
            this.usernameHelpTextlabel.Name = "usernameHelpTextlabel";
            this.usernameHelpTextlabel.Size = new System.Drawing.Size(336, 108);
            this.usernameHelpTextlabel.TabIndex = 2;
            this.usernameHelpTextlabel.Text = "By entering your username, you agree to \r\nabide by the terms in license agreement" +
    " \r\nlinked above.\r\n\r\nSelect a username to identify yourself.\r\nWe don\'t recommend " +
    "using your real name.";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.usernameTextBox.Location = new System.Drawing.Point(13, 299);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(307, 24);
            this.usernameTextBox.TabIndex = 3;
            this.usernameTextBox.Text = "username";
            this.usernameTextBox.TextChanged += new System.EventHandler(this.usernameTextBox_TextChanged);
            // 
            // beginButton
            // 
            this.beginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.beginButton.Location = new System.Drawing.Point(182, 339);
            this.beginButton.Name = "beginButton";
            this.beginButton.Size = new System.Drawing.Size(160, 43);
            this.beginButton.TabIndex = 4;
            this.beginButton.Text = "Begin »";
            this.beginButton.UseVisualStyleBackColor = true;
            this.beginButton.Click += new System.EventHandler(this.beginButton_Click);
            // 
            // statusPictureBox
            // 
            this.statusPictureBox.Location = new System.Drawing.Point(326, 303);
            this.statusPictureBox.Name = "statusPictureBox";
            this.statusPictureBox.Size = new System.Drawing.Size(16, 16);
            this.statusPictureBox.TabIndex = 5;
            this.statusPictureBox.TabStop = false;
            // 
            // subheadLabel
            // 
            this.subheadLabel.AutoSize = true;
            this.subheadLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subheadLabel.ForeColor = System.Drawing.Color.Gray;
            this.subheadLabel.Location = new System.Drawing.Point(10, 58);
            this.subheadLabel.Name = "subheadLabel";
            this.subheadLabel.Size = new System.Drawing.Size(268, 50);
            this.subheadLabel.TabIndex = 7;
            this.subheadLabel.Text = "Welcome to the Bounce \r\nfilesharing network.";
            // 
            // licenseLabel
            // 
            this.licenseLabel.AutoSize = true;
            this.licenseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.licenseLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.licenseLabel.Location = new System.Drawing.Point(10, 122);
            this.licenseLabel.Name = "licenseLabel";
            this.licenseLabel.Size = new System.Drawing.Size(321, 36);
            this.licenseLabel.TabIndex = 8;
            this.licenseLabel.Text = "To use Bounce, you must read and agree to the\r\nterms set out in the ";
            // 
            // licenseLinkLabel
            // 
            this.licenseLinkLabel.AutoSize = true;
            this.licenseLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.licenseLinkLabel.Location = new System.Drawing.Point(140, 140);
            this.licenseLinkLabel.Name = "licenseLinkLabel";
            this.licenseLinkLabel.Size = new System.Drawing.Size(134, 18);
            this.licenseLinkLabel.TabIndex = 9;
            this.licenseLinkLabel.TabStop = true;
            this.licenseLinkLabel.Text = "License Agreement";
            this.licenseLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.licenseLinkLabel_LinkClicked);
            // 
            // checkStatusLabel
            // 
            this.checkStatusLabel.AutoSize = true;
            this.checkStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.checkStatusLabel.Location = new System.Drawing.Point(11, 352);
            this.checkStatusLabel.Name = "checkStatusLabel";
            this.checkStatusLabel.Size = new System.Drawing.Size(0, 16);
            this.checkStatusLabel.TabIndex = 10;
            // 
            // checkWorker
            // 
            this.checkWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.checkWorker_DoWork);
            this.checkWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.checkWorker_RunWorkerCompleted);
            // 
            // WelcomeForm
            // 
            this.AcceptButton = this.beginButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(354, 393);
            this.Controls.Add(this.checkStatusLabel);
            this.Controls.Add(this.licenseLinkLabel);
            this.Controls.Add(this.licenseLabel);
            this.Controls.Add(this.subheadLabel);
            this.Controls.Add(this.statusPictureBox);
            this.Controls.Add(this.beginButton);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameHelpTextlabel);
            this.Controls.Add(this.helloLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            this.Load += new System.EventHandler(this.WelcomeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label helloLabel;
        private System.Windows.Forms.Label usernameHelpTextlabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Button beginButton;
        private System.Windows.Forms.PictureBox statusPictureBox;
        private System.Windows.Forms.Label subheadLabel;
        private System.Windows.Forms.Label licenseLabel;
        private System.Windows.Forms.LinkLabel licenseLinkLabel;
        private System.Windows.Forms.Label checkStatusLabel;
        private System.ComponentModel.BackgroundWorker checkWorker;
    }
}