namespace BouncedClient
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.uploadStatusLabel = new System.Windows.Forms.Label();
            this.fixedDownloadLabel = new System.Windows.Forms.Label();
            this.downloadStatusLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.SearchTabPage = new System.Windows.Forms.TabPage();
            this.searchBoxPanel = new System.Windows.Forms.Panel();
            this.searchGoButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.searchHelpText = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchGridView = new System.Windows.Forms.DataGridView();
            this.fileType = new System.Windows.Forms.DataGridViewImageColumn();
            this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Peer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Action = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Mac = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Hash = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSizeBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransfersTabPage = new System.Windows.Forms.TabPage();
            this.downloadGridHelpTextLabel = new System.Windows.Forms.Label();
            this.downloadGridView = new System.Windows.Forms.DataGridView();
            this.IconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.FileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProgressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ETAColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SpeedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSizeTextColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeerColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.MacColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HashColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FilePathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BouncesTabPage = new System.Windows.Forms.TabPage();
            this.bounceGridHelpTextLabel = new System.Windows.Forms.Label();
            this.bounceGridView = new System.Windows.Forms.DataGridView();
            this.settingsTabPage = new System.Windows.Forms.TabPage();
            this.privacyLinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.settingsHelpText = new System.Windows.Forms.Label();
            this.actionButton = new System.Windows.Forms.Button();
            this.macAddrLabel = new System.Windows.Forms.Label();
            this.forceRescanButton = new System.Windows.Forms.Button();
            this.deleteSelectedButton = new System.Windows.Forms.Button();
            this.addFolderButton = new System.Windows.Forms.Button();
            this.sharedFolders = new System.Windows.Forms.CheckedListBox();
            this.helpText3 = new System.Windows.Forms.Label();
            this.downloadFolder = new System.Windows.Forms.LinkLabel();
            this.fixedDownloadLocationLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.helpText5 = new System.Windows.Forms.Label();
            this.fixedAboutLabel = new System.Windows.Forms.Label();
            this.helpText2 = new System.Windows.Forms.Label();
            this.serverTextBox = new System.Windows.Forms.TextBox();
            this.fixedServerLabel = new System.Windows.Forms.Label();
            this.helpText1 = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.fixedUsernameLabel = new System.Windows.Forms.Label();
            this.registerWorker = new System.ComponentModel.BackgroundWorker();
            this.pollPendingTimer = new System.Windows.Forms.Timer(this.components);
            this.pollPendingWorker = new System.ComponentModel.BackgroundWorker();
            this.loadConfigWorker = new System.ComponentModel.BackgroundWorker();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.indexWorker = new System.ComponentModel.BackgroundWorker();
            this.syncWorker = new System.ComponentModel.BackgroundWorker();
            this.loadIndexWorker = new System.ComponentModel.BackgroundWorker();
            this.searchWorker = new System.ComponentModel.BackgroundWorker();
            this.serverWorker = new System.ComponentModel.BackgroundWorker();
            this.reconnectTimer = new System.Windows.Forms.Timer(this.components);
            this.bounceStatusTimer = new System.Windows.Forms.Timer(this.components);
            this.pollStatusWorker = new System.ComponentModel.BackgroundWorker();
            this.uiUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.searchGridHelpTextLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.syncStatusPictureBox = new System.Windows.Forms.PictureBox();
            this.statusPictureBox = new System.Windows.Forms.PictureBox();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.BouncesFileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BouncesFileSizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BouncesStatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.BouncesFileHashColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BouncesTransferIDColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BouncesUploaderMacColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusPanel.SuspendLayout();
            this.MainTabControl.SuspendLayout();
            this.SearchTabPage.SuspendLayout();
            this.searchBoxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchGridView)).BeginInit();
            this.TransfersTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.downloadGridView)).BeginInit();
            this.BouncesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bounceGridView)).BeginInit();
            this.settingsTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusPanel
            // 
            this.StatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(239)))), ((int)(((byte)(239)))));
            this.StatusPanel.Controls.Add(this.syncStatusPictureBox);
            this.StatusPanel.Controls.Add(this.label4);
            this.StatusPanel.Controls.Add(this.statusPictureBox);
            this.StatusPanel.Controls.Add(this.label6);
            this.StatusPanel.Controls.Add(this.uploadStatusLabel);
            this.StatusPanel.Controls.Add(this.fixedDownloadLabel);
            this.StatusPanel.Controls.Add(this.downloadStatusLabel);
            this.StatusPanel.Controls.Add(this.statusLabel);
            this.StatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusPanel.Location = new System.Drawing.Point(0, 506);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(784, 35);
            this.StatusPanel.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(693, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "UPLOADS";
            // 
            // uploadStatusLabel
            // 
            this.uploadStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uploadStatusLabel.AutoSize = true;
            this.uploadStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.uploadStatusLabel.Location = new System.Drawing.Point(756, 9);
            this.uploadStatusLabel.Name = "uploadStatusLabel";
            this.uploadStatusLabel.Size = new System.Drawing.Size(13, 15);
            this.uploadStatusLabel.TabIndex = 7;
            this.uploadStatusLabel.Text = "0";
            // 
            // fixedDownloadLabel
            // 
            this.fixedDownloadLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fixedDownloadLabel.AutoSize = true;
            this.fixedDownloadLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedDownloadLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.fixedDownloadLabel.Location = new System.Drawing.Point(567, 10);
            this.fixedDownloadLabel.Name = "fixedDownloadLabel";
            this.fixedDownloadLabel.Size = new System.Drawing.Size(79, 13);
            this.fixedDownloadLabel.TabIndex = 3;
            this.fixedDownloadLabel.Text = "DOWNLOADS";
            // 
            // downloadStatusLabel
            // 
            this.downloadStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadStatusLabel.AutoSize = true;
            this.downloadStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.downloadStatusLabel.Location = new System.Drawing.Point(651, 9);
            this.downloadStatusLabel.Name = "downloadStatusLabel";
            this.downloadStatusLabel.Size = new System.Drawing.Size(13, 15);
            this.downloadStatusLabel.TabIndex = 2;
            this.downloadStatusLabel.Text = "0";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.statusLabel.Location = new System.Drawing.Point(29, 10);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(131, 15);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Loading configuration..";
            // 
            // MainTabControl
            // 
            this.MainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainTabControl.Controls.Add(this.SearchTabPage);
            this.MainTabControl.Controls.Add(this.TransfersTabPage);
            this.MainTabControl.Controls.Add(this.BouncesTabPage);
            this.MainTabControl.Controls.Add(this.settingsTabPage);
            this.MainTabControl.ItemSize = new System.Drawing.Size(200, 40);
            this.MainTabControl.Location = new System.Drawing.Point(0, 12);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.Padding = new System.Drawing.Point(10, 3);
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(784, 495);
            this.MainTabControl.TabIndex = 1;
            this.MainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
            // 
            // SearchTabPage
            // 
            this.SearchTabPage.Controls.Add(this.searchGridHelpTextLabel);
            this.SearchTabPage.Controls.Add(this.searchBoxPanel);
            this.SearchTabPage.Controls.Add(this.searchGridView);
            this.SearchTabPage.Location = new System.Drawing.Point(4, 44);
            this.SearchTabPage.Name = "SearchTabPage";
            this.SearchTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.SearchTabPage.Size = new System.Drawing.Size(776, 447);
            this.SearchTabPage.TabIndex = 0;
            this.SearchTabPage.Text = "Search";
            this.SearchTabPage.UseVisualStyleBackColor = true;
            // 
            // searchBoxPanel
            // 
            this.searchBoxPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBoxPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.searchBoxPanel.Controls.Add(this.searchGoButton);
            this.searchBoxPanel.Controls.Add(this.label1);
            this.searchBoxPanel.Controls.Add(this.searchHelpText);
            this.searchBoxPanel.Controls.Add(this.searchBox);
            this.searchBoxPanel.Location = new System.Drawing.Point(8, 6);
            this.searchBoxPanel.Name = "searchBoxPanel";
            this.searchBoxPanel.Size = new System.Drawing.Size(762, 34);
            this.searchBoxPanel.TabIndex = 5;
            // 
            // searchGoButton
            // 
            this.searchGoButton.Location = new System.Drawing.Point(450, 3);
            this.searchGoButton.Name = "searchGoButton";
            this.searchGoButton.Size = new System.Drawing.Size(56, 28);
            this.searchGoButton.TabIndex = 5;
            this.searchGoButton.Text = "Go";
            this.searchGoButton.UseVisualStyleBackColor = true;
            this.searchGoButton.Click += new System.EventHandler(this.searchGoButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(509, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(249, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "SEARCH BY KEYWORDS, FILE NAME, FILE TYPE";
            // 
            // searchHelpText
            // 
            this.searchHelpText.AutoSize = true;
            this.searchHelpText.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.searchHelpText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchHelpText.ForeColor = System.Drawing.Color.Gray;
            this.searchHelpText.Location = new System.Drawing.Point(43, 7);
            this.searchHelpText.Name = "searchHelpText";
            this.searchHelpText.Size = new System.Drawing.Size(367, 20);
            this.searchHelpText.TabIndex = 4;
            this.searchHelpText.Text = "type in keywords and press enter to search...";
            this.searchHelpText.Click += new System.EventHandler(this.searchHelpText_Click);
            // 
            // searchBox
            // 
            this.searchBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchBox.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.searchBox.Location = new System.Drawing.Point(4, 4);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(443, 26);
            this.searchBox.TabIndex = 1;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchBox_KeyPress);
            // 
            // searchGridView
            // 
            this.searchGridView.AllowUserToAddRows = false;
            this.searchGridView.AllowUserToDeleteRows = false;
            this.searchGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.searchGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.searchGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.searchGridView.BackgroundColor = System.Drawing.Color.White;
            this.searchGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.searchGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.searchGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.searchGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.searchGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fileType,
            this.FileName,
            this.FileSize,
            this.Peer,
            this.Action,
            this.Mac,
            this.Hash,
            this.FileSizeBytes});
            this.searchGridView.Location = new System.Drawing.Point(8, 45);
            this.searchGridView.MultiSelect = false;
            this.searchGridView.Name = "searchGridView";
            this.searchGridView.ReadOnly = true;
            this.searchGridView.RowHeadersVisible = false;
            this.searchGridView.RowTemplate.Height = 35;
            this.searchGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.searchGridView.Size = new System.Drawing.Size(761, 396);
            this.searchGridView.TabIndex = 0;
            this.searchGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.searchGridView_CellClick);
            // 
            // fileType
            // 
            this.fileType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.fileType.FillWeight = 40.60914F;
            this.fileType.HeaderText = "";
            this.fileType.Name = "fileType";
            this.fileType.ReadOnly = true;
            this.fileType.Width = 25;
            // 
            // FileName
            // 
            this.FileName.FillWeight = 119.797F;
            this.FileName.HeaderText = "File Name";
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FileSize
            // 
            this.FileSize.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FileSize.FillWeight = 119.797F;
            this.FileSize.HeaderText = "File Size";
            this.FileSize.Name = "FileSize";
            this.FileSize.ReadOnly = true;
            this.FileSize.Width = 80;
            // 
            // Peer
            // 
            this.Peer.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Peer.HeaderText = "Peer";
            this.Peer.Name = "Peer";
            this.Peer.ReadOnly = true;
            this.Peer.Width = 55;
            // 
            // Action
            // 
            this.Action.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Action.FillWeight = 119.797F;
            this.Action.HeaderText = "Action";
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            this.Action.Width = 150;
            // 
            // Mac
            // 
            this.Mac.HeaderText = "Mac";
            this.Mac.Name = "Mac";
            this.Mac.ReadOnly = true;
            this.Mac.Visible = false;
            // 
            // Hash
            // 
            this.Hash.HeaderText = "Hash";
            this.Hash.Name = "Hash";
            this.Hash.ReadOnly = true;
            this.Hash.Visible = false;
            // 
            // FileSizeBytes
            // 
            this.FileSizeBytes.HeaderText = "File Size (Bytes)";
            this.FileSizeBytes.Name = "FileSizeBytes";
            this.FileSizeBytes.ReadOnly = true;
            this.FileSizeBytes.Visible = false;
            // 
            // TransfersTabPage
            // 
            this.TransfersTabPage.Controls.Add(this.downloadGridHelpTextLabel);
            this.TransfersTabPage.Controls.Add(this.downloadGridView);
            this.TransfersTabPage.Location = new System.Drawing.Point(4, 44);
            this.TransfersTabPage.Name = "TransfersTabPage";
            this.TransfersTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.TransfersTabPage.Size = new System.Drawing.Size(776, 447);
            this.TransfersTabPage.TabIndex = 1;
            this.TransfersTabPage.Text = "Transfers";
            this.TransfersTabPage.UseVisualStyleBackColor = true;
            // 
            // downloadGridHelpTextLabel
            // 
            this.downloadGridHelpTextLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.downloadGridHelpTextLabel.AutoSize = true;
            this.downloadGridHelpTextLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.downloadGridHelpTextLabel.Location = new System.Drawing.Point(222, 208);
            this.downloadGridHelpTextLabel.Name = "downloadGridHelpTextLabel";
            this.downloadGridHelpTextLabel.Size = new System.Drawing.Size(330, 30);
            this.downloadGridHelpTextLabel.TabIndex = 9;
            this.downloadGridHelpTextLabel.Text = "No transfers currently in progress.\r\nClick the \'download\' button next to a file t" +
    "o see it appear here";
            this.downloadGridHelpTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // downloadGridView
            // 
            this.downloadGridView.AllowUserToAddRows = false;
            this.downloadGridView.AllowUserToDeleteRows = false;
            this.downloadGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.downloadGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.downloadGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.downloadGridView.BackgroundColor = System.Drawing.Color.White;
            this.downloadGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.downloadGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.downloadGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.downloadGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.downloadGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IconColumn,
            this.FileNameColumn,
            this.StatusColumn,
            this.ProgressColumn,
            this.ETAColumn,
            this.SpeedColumn,
            this.FileSizeTextColumn,
            this.PeerColumn,
            this.ActionColumn,
            this.MacColumn,
            this.HashColumn,
            this.FileSizeColumn,
            this.FilePathColumn});
            this.downloadGridView.Location = new System.Drawing.Point(7, 6);
            this.downloadGridView.MultiSelect = false;
            this.downloadGridView.Name = "downloadGridView";
            this.downloadGridView.ReadOnly = true;
            this.downloadGridView.RowHeadersVisible = false;
            this.downloadGridView.RowTemplate.Height = 35;
            this.downloadGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.downloadGridView.Size = new System.Drawing.Size(761, 435);
            this.downloadGridView.TabIndex = 1;
            this.downloadGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.downloadGridView_CellClick);
            // 
            // IconColumn
            // 
            this.IconColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.IconColumn.FillWeight = 40.60914F;
            this.IconColumn.HeaderText = "";
            this.IconColumn.Name = "IconColumn";
            this.IconColumn.ReadOnly = true;
            this.IconColumn.Width = 25;
            // 
            // FileNameColumn
            // 
            this.FileNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FileNameColumn.FillWeight = 119.797F;
            this.FileNameColumn.HeaderText = "File Name";
            this.FileNameColumn.Name = "FileNameColumn";
            this.FileNameColumn.ReadOnly = true;
            this.FileNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FileNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // StatusColumn
            // 
            this.StatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.Width = 90;
            // 
            // ProgressColumn
            // 
            this.ProgressColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ProgressColumn.HeaderText = "Progress";
            this.ProgressColumn.Name = "ProgressColumn";
            this.ProgressColumn.ReadOnly = true;
            this.ProgressColumn.Width = 70;
            // 
            // ETAColumn
            // 
            this.ETAColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ETAColumn.HeaderText = "ETA";
            this.ETAColumn.Name = "ETAColumn";
            this.ETAColumn.ReadOnly = true;
            this.ETAColumn.Width = 70;
            // 
            // SpeedColumn
            // 
            this.SpeedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SpeedColumn.HeaderText = "Speed";
            this.SpeedColumn.Name = "SpeedColumn";
            this.SpeedColumn.ReadOnly = true;
            this.SpeedColumn.Width = 80;
            // 
            // FileSizeTextColumn
            // 
            this.FileSizeTextColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FileSizeTextColumn.FillWeight = 119.797F;
            this.FileSizeTextColumn.HeaderText = "File Size";
            this.FileSizeTextColumn.Name = "FileSizeTextColumn";
            this.FileSizeTextColumn.ReadOnly = true;
            this.FileSizeTextColumn.Width = 80;
            // 
            // PeerColumn
            // 
            this.PeerColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PeerColumn.HeaderText = "Peer";
            this.PeerColumn.Name = "PeerColumn";
            this.PeerColumn.ReadOnly = true;
            this.PeerColumn.Width = 110;
            // 
            // ActionColumn
            // 
            this.ActionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ActionColumn.FillWeight = 119.797F;
            this.ActionColumn.HeaderText = "Action";
            this.ActionColumn.Name = "ActionColumn";
            this.ActionColumn.ReadOnly = true;
            // 
            // MacColumn
            // 
            this.MacColumn.HeaderText = "Mac";
            this.MacColumn.Name = "MacColumn";
            this.MacColumn.ReadOnly = true;
            this.MacColumn.Visible = false;
            // 
            // HashColumn
            // 
            this.HashColumn.HeaderText = "Hash";
            this.HashColumn.Name = "HashColumn";
            this.HashColumn.ReadOnly = true;
            this.HashColumn.Visible = false;
            // 
            // FileSizeColumn
            // 
            this.FileSizeColumn.HeaderText = "File Size (Bytes)";
            this.FileSizeColumn.Name = "FileSizeColumn";
            this.FileSizeColumn.ReadOnly = true;
            this.FileSizeColumn.Visible = false;
            // 
            // FilePathColumn
            // 
            this.FilePathColumn.HeaderText = "File Path";
            this.FilePathColumn.Name = "FilePathColumn";
            this.FilePathColumn.ReadOnly = true;
            this.FilePathColumn.Visible = false;
            // 
            // BouncesTabPage
            // 
            this.BouncesTabPage.Controls.Add(this.bounceGridHelpTextLabel);
            this.BouncesTabPage.Controls.Add(this.bounceGridView);
            this.BouncesTabPage.Location = new System.Drawing.Point(4, 44);
            this.BouncesTabPage.Name = "BouncesTabPage";
            this.BouncesTabPage.Size = new System.Drawing.Size(776, 447);
            this.BouncesTabPage.TabIndex = 2;
            this.BouncesTabPage.Text = "Bounces";
            this.BouncesTabPage.UseVisualStyleBackColor = true;
            // 
            // bounceGridHelpTextLabel
            // 
            this.bounceGridHelpTextLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.bounceGridHelpTextLabel.AutoSize = true;
            this.bounceGridHelpTextLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.bounceGridHelpTextLabel.Location = new System.Drawing.Point(218, 208);
            this.bounceGridHelpTextLabel.Name = "bounceGridHelpTextLabel";
            this.bounceGridHelpTextLabel.Size = new System.Drawing.Size(340, 30);
            this.bounceGridHelpTextLabel.TabIndex = 10;
            this.bounceGridHelpTextLabel.Text = "There aren\'t any files pending for you.\r\nFiles you requested that aren\'t currentl" +
    "y available are listed here";
            this.bounceGridHelpTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bounceGridView
            // 
            this.bounceGridView.AllowUserToAddRows = false;
            this.bounceGridView.AllowUserToDeleteRows = false;
            this.bounceGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.bounceGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.bounceGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bounceGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.bounceGridView.BackgroundColor = System.Drawing.Color.White;
            this.bounceGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.bounceGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.bounceGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.bounceGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bounceGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewImageColumn1,
            this.BouncesFileNameColumn,
            this.BouncesFileSizeColumn,
            this.BouncesStatusColumn,
            this.dataGridViewButtonColumn1,
            this.BouncesFileHashColumn,
            this.BouncesTransferIDColumn,
            this.BouncesUploaderMacColumn});
            this.bounceGridView.Location = new System.Drawing.Point(8, 6);
            this.bounceGridView.MultiSelect = false;
            this.bounceGridView.Name = "bounceGridView";
            this.bounceGridView.ReadOnly = true;
            this.bounceGridView.RowHeadersVisible = false;
            this.bounceGridView.RowTemplate.Height = 35;
            this.bounceGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.bounceGridView.Size = new System.Drawing.Size(761, 435);
            this.bounceGridView.TabIndex = 1;
            this.bounceGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.bounceGridView_CellClick);
            // 
            // settingsTabPage
            // 
            this.settingsTabPage.Controls.Add(this.privacyLinkLabel);
            this.settingsTabPage.Controls.Add(this.panel1);
            this.settingsTabPage.Controls.Add(this.macAddrLabel);
            this.settingsTabPage.Controls.Add(this.forceRescanButton);
            this.settingsTabPage.Controls.Add(this.deleteSelectedButton);
            this.settingsTabPage.Controls.Add(this.addFolderButton);
            this.settingsTabPage.Controls.Add(this.sharedFolders);
            this.settingsTabPage.Controls.Add(this.helpText3);
            this.settingsTabPage.Controls.Add(this.downloadFolder);
            this.settingsTabPage.Controls.Add(this.fixedDownloadLocationLabel);
            this.settingsTabPage.Controls.Add(this.label2);
            this.settingsTabPage.Controls.Add(this.helpText5);
            this.settingsTabPage.Controls.Add(this.fixedAboutLabel);
            this.settingsTabPage.Controls.Add(this.helpText2);
            this.settingsTabPage.Controls.Add(this.serverTextBox);
            this.settingsTabPage.Controls.Add(this.fixedServerLabel);
            this.settingsTabPage.Controls.Add(this.helpText1);
            this.settingsTabPage.Controls.Add(this.usernameTextBox);
            this.settingsTabPage.Controls.Add(this.fixedUsernameLabel);
            this.settingsTabPage.Location = new System.Drawing.Point(4, 44);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Size = new System.Drawing.Size(776, 447);
            this.settingsTabPage.TabIndex = 3;
            this.settingsTabPage.Text = "Settings";
            this.settingsTabPage.UseVisualStyleBackColor = true;
            // 
            // privacyLinkLabel
            // 
            this.privacyLinkLabel.AutoSize = true;
            this.privacyLinkLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacyLinkLabel.Location = new System.Drawing.Point(13, 424);
            this.privacyLinkLabel.Name = "privacyLinkLabel";
            this.privacyLinkLabel.Size = new System.Drawing.Size(164, 13);
            this.privacyLinkLabel.TabIndex = 20;
            this.privacyLinkLabel.TabStop = true;
            this.privacyLinkLabel.Text = "Privacy Policy and Terms of Use";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel1.Controls.Add(this.settingsHelpText);
            this.panel1.Controls.Add(this.actionButton);
            this.panel1.Location = new System.Drawing.Point(8, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(762, 34);
            this.panel1.TabIndex = 19;
            // 
            // settingsHelpText
            // 
            this.settingsHelpText.AutoSize = true;
            this.settingsHelpText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsHelpText.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.settingsHelpText.Location = new System.Drawing.Point(11, 10);
            this.settingsHelpText.Name = "settingsHelpText";
            this.settingsHelpText.Size = new System.Drawing.Size(578, 13);
            this.settingsHelpText.TabIndex = 2;
            this.settingsHelpText.Text = "Changing the username or server will disconnect you from the network. Reconnect w" +
    "hen you\'re done editing.";
            // 
            // actionButton
            // 
            this.actionButton.Enabled = false;
            this.actionButton.Location = new System.Drawing.Point(610, 3);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(138, 27);
            this.actionButton.TabIndex = 2;
            this.actionButton.Text = "Reconnect";
            this.actionButton.UseVisualStyleBackColor = true;
            this.actionButton.Click += new System.EventHandler(this.actionButton_Click);
            // 
            // macAddrLabel
            // 
            this.macAddrLabel.AutoSize = true;
            this.macAddrLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.macAddrLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.macAddrLabel.Location = new System.Drawing.Point(13, 406);
            this.macAddrLabel.Name = "macAddrLabel";
            this.macAddrLabel.Size = new System.Drawing.Size(37, 13);
            this.macAddrLabel.TabIndex = 18;
            this.macAddrLabel.Text = "MAC: ";
            // 
            // forceRescanButton
            // 
            this.forceRescanButton.Location = new System.Drawing.Point(618, 322);
            this.forceRescanButton.Name = "forceRescanButton";
            this.forceRescanButton.Size = new System.Drawing.Size(138, 28);
            this.forceRescanButton.TabIndex = 17;
            this.forceRescanButton.Text = "Force Rescan";
            this.forceRescanButton.UseVisualStyleBackColor = true;
            this.forceRescanButton.Click += new System.EventHandler(this.forceRescanButton_Click);
            // 
            // deleteSelectedButton
            // 
            this.deleteSelectedButton.Location = new System.Drawing.Point(618, 289);
            this.deleteSelectedButton.Name = "deleteSelectedButton";
            this.deleteSelectedButton.Size = new System.Drawing.Size(138, 28);
            this.deleteSelectedButton.TabIndex = 16;
            this.deleteSelectedButton.Text = "Delete Selected";
            this.deleteSelectedButton.UseVisualStyleBackColor = true;
            this.deleteSelectedButton.Click += new System.EventHandler(this.deleteSelectedButton_Click);
            // 
            // addFolderButton
            // 
            this.addFolderButton.Location = new System.Drawing.Point(618, 256);
            this.addFolderButton.Name = "addFolderButton";
            this.addFolderButton.Size = new System.Drawing.Size(138, 28);
            this.addFolderButton.TabIndex = 15;
            this.addFolderButton.Text = "Add Folder";
            this.addFolderButton.UseVisualStyleBackColor = true;
            this.addFolderButton.Click += new System.EventHandler(this.addFolderButton_Click);
            // 
            // sharedFolders
            // 
            this.sharedFolders.CheckOnClick = true;
            this.sharedFolders.FormattingEnabled = true;
            this.sharedFolders.Location = new System.Drawing.Point(16, 257);
            this.sharedFolders.Name = "sharedFolders";
            this.sharedFolders.Size = new System.Drawing.Size(596, 94);
            this.sharedFolders.TabIndex = 14;
            // 
            // helpText3
            // 
            this.helpText3.AutoSize = true;
            this.helpText3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText3.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.helpText3.Location = new System.Drawing.Point(13, 206);
            this.helpText3.Name = "helpText3";
            this.helpText3.Size = new System.Drawing.Size(312, 13);
            this.helpText3.TabIndex = 13;
            this.helpText3.Text = "This is the location your downloaded files will be placed in.";
            // 
            // downloadFolder
            // 
            this.downloadFolder.AutoSize = true;
            this.downloadFolder.Location = new System.Drawing.Point(13, 187);
            this.downloadFolder.Name = "downloadFolder";
            this.downloadFolder.Size = new System.Drawing.Size(243, 15);
            this.downloadFolder.TabIndex = 12;
            this.downloadFolder.TabStop = true;
            this.downloadFolder.Text = "C:\\Users\\Raghav Sethi\\Bounced Downloads\\";
            this.downloadFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadFolder_LinkClicked);
            // 
            // fixedDownloadLocationLabel
            // 
            this.fixedDownloadLocationLabel.AutoSize = true;
            this.fixedDownloadLocationLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedDownloadLocationLabel.ForeColor = System.Drawing.Color.Black;
            this.fixedDownloadLocationLabel.Location = new System.Drawing.Point(13, 167);
            this.fixedDownloadLocationLabel.Name = "fixedDownloadLocationLabel";
            this.fixedDownloadLocationLabel.Size = new System.Drawing.Size(123, 13);
            this.fixedDownloadLocationLabel.TabIndex = 11;
            this.fixedDownloadLocationLabel.Text = "DOWNLOADS FOLDER";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "SHARED FOLDERS";
            // 
            // helpText5
            // 
            this.helpText5.AutoSize = true;
            this.helpText5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.helpText5.Location = new System.Drawing.Point(13, 387);
            this.helpText5.Name = "helpText5";
            this.helpText5.Size = new System.Drawing.Size(488, 13);
            this.helpText5.TabIndex = 9;
            this.helpText5.Text = "Bounced Client v0.9. Built with love at IIIT-D by Raghav Sethi, Naved Alam and Ma" +
    "yank Pundir.";
            // 
            // fixedAboutLabel
            // 
            this.fixedAboutLabel.AutoSize = true;
            this.fixedAboutLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedAboutLabel.ForeColor = System.Drawing.Color.Black;
            this.fixedAboutLabel.Location = new System.Drawing.Point(13, 368);
            this.fixedAboutLabel.Name = "fixedAboutLabel";
            this.fixedAboutLabel.Size = new System.Drawing.Size(44, 13);
            this.fixedAboutLabel.TabIndex = 8;
            this.fixedAboutLabel.Text = "ABOUT";
            // 
            // helpText2
            // 
            this.helpText2.AutoSize = true;
            this.helpText2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.helpText2.Location = new System.Drawing.Point(186, 131);
            this.helpText2.Name = "helpText2";
            this.helpText2.Size = new System.Drawing.Size(123, 13);
            this.helpText2.TabIndex = 7;
            this.helpText2.Text = "Double-click to modify";
            // 
            // serverTextBox
            // 
            this.serverTextBox.Location = new System.Drawing.Point(16, 127);
            this.serverTextBox.MaxLength = 100;
            this.serverTextBox.Name = "serverTextBox";
            this.serverTextBox.ReadOnly = true;
            this.serverTextBox.Size = new System.Drawing.Size(164, 23);
            this.serverTextBox.TabIndex = 6;
            this.serverTextBox.Text = "localhost:3000";
            this.serverTextBox.TextChanged += new System.EventHandler(this.serverTextBox_TextChanged);
            this.serverTextBox.DoubleClick += new System.EventHandler(this.serverTextBox_DoubleClick);
            this.serverTextBox.Leave += new System.EventHandler(this.serverTextBox_Leave);
            // 
            // fixedServerLabel
            // 
            this.fixedServerLabel.AutoSize = true;
            this.fixedServerLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedServerLabel.ForeColor = System.Drawing.Color.Black;
            this.fixedServerLabel.Location = new System.Drawing.Point(13, 111);
            this.fixedServerLabel.Name = "fixedServerLabel";
            this.fixedServerLabel.Size = new System.Drawing.Size(46, 13);
            this.fixedServerLabel.TabIndex = 5;
            this.fixedServerLabel.Text = "SERVER";
            // 
            // helpText1
            // 
            this.helpText1.AutoSize = true;
            this.helpText1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpText1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.helpText1.Location = new System.Drawing.Point(186, 78);
            this.helpText1.Name = "helpText1";
            this.helpText1.Size = new System.Drawing.Size(570, 13);
            this.helpText1.TabIndex = 4;
            this.helpText1.Text = "Usernames are guaranteed to be unique. No one else will be able to use your usern" +
    "ame unless you change it.";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(16, 74);
            this.usernameTextBox.MaxLength = 30;
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(164, 23);
            this.usernameTextBox.TabIndex = 3;
            this.usernameTextBox.TextChanged += new System.EventHandler(this.usernameTextBox_TextChanged);
            this.usernameTextBox.Leave += new System.EventHandler(this.usernameTextBox_Leave);
            // 
            // fixedUsernameLabel
            // 
            this.fixedUsernameLabel.AutoSize = true;
            this.fixedUsernameLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fixedUsernameLabel.ForeColor = System.Drawing.Color.Black;
            this.fixedUsernameLabel.Location = new System.Drawing.Point(13, 57);
            this.fixedUsernameLabel.Name = "fixedUsernameLabel";
            this.fixedUsernameLabel.Size = new System.Drawing.Size(68, 13);
            this.fixedUsernameLabel.TabIndex = 2;
            this.fixedUsernameLabel.Text = "USERNAME";
            // 
            // registerWorker
            // 
            this.registerWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.registerWorker_DoWork);
            this.registerWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.registerWorker_RunWorkerCompleted);
            // 
            // pollPendingTimer
            // 
            this.pollPendingTimer.Interval = 3000;
            this.pollPendingTimer.Tick += new System.EventHandler(this.pollPendingTimer_Tick);
            // 
            // pollPendingWorker
            // 
            this.pollPendingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.pollPendingWorker_DoWork);
            this.pollPendingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.pollPendingWorker_RunWorkerCompleted);
            // 
            // loadConfigWorker
            // 
            this.loadConfigWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadConfigWorker_DoWork);
            this.loadConfigWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadConfigWorker_RunWorkerCompleted);
            // 
            // indexWorker
            // 
            this.indexWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.indexWorker_DoWork);
            this.indexWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.indexWorker_RunWorkerCompleted);
            // 
            // syncWorker
            // 
            this.syncWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.syncWorker_DoWork);
            this.syncWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.syncWorker_RunWorkerCompleted);
            // 
            // loadIndexWorker
            // 
            this.loadIndexWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadIndexWorker_DoWork);
            this.loadIndexWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadIndexWorker_RunWorkerCompleted);
            // 
            // searchWorker
            // 
            this.searchWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.searchWorker_DoWork);
            this.searchWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.searchWorker_RunWorkerCompleted);
            // 
            // serverWorker
            // 
            this.serverWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.serverWorker_DoWork);
            // 
            // reconnectTimer
            // 
            this.reconnectTimer.Interval = 3000;
            this.reconnectTimer.Tick += new System.EventHandler(this.reconnectTimer_Tick);
            // 
            // bounceStatusTimer
            // 
            this.bounceStatusTimer.Interval = 2000;
            this.bounceStatusTimer.Tick += new System.EventHandler(this.bounceStatusTimer_Tick);
            // 
            // pollStatusWorker
            // 
            this.pollStatusWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.pollStatusWorker_DoWork);
            this.pollStatusWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.pollStatusWorker_RunWorkerCompleted);
            // 
            // uiUpdateTimer
            // 
            this.uiUpdateTimer.Enabled = true;
            this.uiUpdateTimer.Interval = 1000;
            this.uiUpdateTimer.Tick += new System.EventHandler(this.uiUpdateTimer_Tick);
            // 
            // searchGridHelpTextLabel
            // 
            this.searchGridHelpTextLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.searchGridHelpTextLabel.AutoSize = true;
            this.searchGridHelpTextLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.searchGridHelpTextLabel.Location = new System.Drawing.Point(339, 208);
            this.searchGridHelpTextLabel.Name = "searchGridHelpTextLabel";
            this.searchGridHelpTextLabel.Size = new System.Drawing.Size(98, 15);
            this.searchGridHelpTextLabel.TabIndex = 10;
            this.searchGridHelpTextLabel.Text = "No results found.";
            this.searchGridHelpTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.searchGridHelpTextLabel.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(484, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "SYNC";
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(647, 7);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(140, 40);
            this.logoPictureBox.TabIndex = 3;
            this.logoPictureBox.TabStop = false;
            // 
            // syncStatusPictureBox
            // 
            this.syncStatusPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.syncStatusPictureBox.Image = global::BounceClient.Properties.Resources.sync_working;
            this.syncStatusPictureBox.Location = new System.Drawing.Point(524, 8);
            this.syncStatusPictureBox.Name = "syncStatusPictureBox";
            this.syncStatusPictureBox.Size = new System.Drawing.Size(16, 16);
            this.syncStatusPictureBox.TabIndex = 10;
            this.syncStatusPictureBox.TabStop = false;
            // 
            // statusPictureBox
            // 
            this.statusPictureBox.Image = global::BounceClient.Properties.Resources.connection_working;
            this.statusPictureBox.Location = new System.Drawing.Point(8, 10);
            this.statusPictureBox.Name = "statusPictureBox";
            this.statusPictureBox.Size = new System.Drawing.Size(16, 16);
            this.statusPictureBox.TabIndex = 4;
            this.statusPictureBox.TabStop = false;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewImageColumn1.FillWeight = 40.60914F;
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Width = 25;
            // 
            // BouncesFileNameColumn
            // 
            this.BouncesFileNameColumn.FillWeight = 119.797F;
            this.BouncesFileNameColumn.HeaderText = "File Name";
            this.BouncesFileNameColumn.Name = "BouncesFileNameColumn";
            this.BouncesFileNameColumn.ReadOnly = true;
            this.BouncesFileNameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.BouncesFileNameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // BouncesFileSizeColumn
            // 
            this.BouncesFileSizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.BouncesFileSizeColumn.FillWeight = 119.797F;
            this.BouncesFileSizeColumn.HeaderText = "File Size";
            this.BouncesFileSizeColumn.Name = "BouncesFileSizeColumn";
            this.BouncesFileSizeColumn.ReadOnly = true;
            this.BouncesFileSizeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BouncesFileSizeColumn.Width = 80;
            // 
            // BouncesStatusColumn
            // 
            this.BouncesStatusColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.BouncesStatusColumn.HeaderText = "Status";
            this.BouncesStatusColumn.Name = "BouncesStatusColumn";
            this.BouncesStatusColumn.ReadOnly = true;
            this.BouncesStatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BouncesStatusColumn.Width = 250;
            // 
            // dataGridViewButtonColumn1
            // 
            this.dataGridViewButtonColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewButtonColumn1.FillWeight = 119.797F;
            this.dataGridViewButtonColumn1.HeaderText = "Action";
            this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
            this.dataGridViewButtonColumn1.ReadOnly = true;
            // 
            // BouncesFileHashColumn
            // 
            this.BouncesFileHashColumn.HeaderText = "Hash";
            this.BouncesFileHashColumn.Name = "BouncesFileHashColumn";
            this.BouncesFileHashColumn.ReadOnly = true;
            this.BouncesFileHashColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BouncesFileHashColumn.Visible = false;
            // 
            // BouncesTransferIDColumn
            // 
            this.BouncesTransferIDColumn.HeaderText = "TransferID";
            this.BouncesTransferIDColumn.Name = "BouncesTransferIDColumn";
            this.BouncesTransferIDColumn.ReadOnly = true;
            this.BouncesTransferIDColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BouncesTransferIDColumn.Visible = false;
            // 
            // BouncesUploaderMacColumn
            // 
            this.BouncesUploaderMacColumn.HeaderText = "Uploader Mac";
            this.BouncesUploaderMacColumn.Name = "BouncesUploaderMacColumn";
            this.BouncesUploaderMacColumn.ReadOnly = true;
            this.BouncesUploaderMacColumn.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 541);
            this.Controls.Add(this.logoPictureBox);
            this.Controls.Add(this.MainTabControl);
            this.Controls.Add(this.StatusPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 580);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bounce";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.StatusPanel.ResumeLayout(false);
            this.StatusPanel.PerformLayout();
            this.MainTabControl.ResumeLayout(false);
            this.SearchTabPage.ResumeLayout(false);
            this.SearchTabPage.PerformLayout();
            this.searchBoxPanel.ResumeLayout(false);
            this.searchBoxPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchGridView)).EndInit();
            this.TransfersTabPage.ResumeLayout(false);
            this.TransfersTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.downloadGridView)).EndInit();
            this.BouncesTabPage.ResumeLayout(false);
            this.BouncesTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bounceGridView)).EndInit();
            this.settingsTabPage.ResumeLayout(false);
            this.settingsTabPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.syncStatusPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel StatusPanel;
        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage SearchTabPage;
        private System.Windows.Forms.TabPage TransfersTabPage;
        private System.Windows.Forms.TabPage BouncesTabPage;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.TabPage settingsTabPage;
        private System.Windows.Forms.DataGridView searchGridView;
        private System.Windows.Forms.Label statusLabel;
        private System.ComponentModel.BackgroundWorker registerWorker;
        private System.Windows.Forms.Timer pollPendingTimer;
        private System.ComponentModel.BackgroundWorker pollPendingWorker;
        private System.Windows.Forms.Label searchHelpText;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel searchBoxPanel;
        private System.Windows.Forms.Button searchGoButton;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label fixedUsernameLabel;
        private System.Windows.Forms.Label helpText1;
        private System.Windows.Forms.Label helpText2;
        private System.Windows.Forms.TextBox serverTextBox;
        private System.Windows.Forms.Label fixedServerLabel;
        private System.Windows.Forms.Label helpText5;
        private System.Windows.Forms.Label fixedAboutLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label fixedDownloadLocationLabel;
        private System.Windows.Forms.LinkLabel downloadFolder;
        private System.Windows.Forms.Label helpText3;
        private System.Windows.Forms.Button forceRescanButton;
        private System.Windows.Forms.Button deleteSelectedButton;
        private System.Windows.Forms.Button addFolderButton;
        private System.Windows.Forms.CheckedListBox sharedFolders;
        private System.ComponentModel.BackgroundWorker loadConfigWorker;
        private System.Windows.Forms.Button actionButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.ComponentModel.BackgroundWorker indexWorker;
        private System.ComponentModel.BackgroundWorker syncWorker;
        private System.ComponentModel.BackgroundWorker loadIndexWorker;
        private System.ComponentModel.BackgroundWorker searchWorker;
        private System.Windows.Forms.Label fixedDownloadLabel;
        private System.Windows.Forms.Label downloadStatusLabel;
        private System.Windows.Forms.DataGridViewImageColumn fileType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn Peer;
        private System.Windows.Forms.DataGridViewButtonColumn Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mac;
        private System.Windows.Forms.DataGridViewTextBoxColumn Hash;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSizeBytes;
        private System.Windows.Forms.DataGridView downloadGridView;
        private System.ComponentModel.BackgroundWorker serverWorker;
        private System.Windows.Forms.Timer reconnectTimer;
        private System.Windows.Forms.Label macAddrLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label uploadStatusLabel;
        private System.Windows.Forms.DataGridView bounceGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label settingsHelpText;
        private System.Windows.Forms.PictureBox statusPictureBox;
        private System.Windows.Forms.Timer bounceStatusTimer;
        private System.ComponentModel.BackgroundWorker pollStatusWorker;
        private System.Windows.Forms.LinkLabel privacyLinkLabel;
        private System.Windows.Forms.Timer uiUpdateTimer;
        private System.Windows.Forms.Label downloadGridHelpTextLabel;
        private System.Windows.Forms.Label bounceGridHelpTextLabel;
        private System.Windows.Forms.DataGridViewImageColumn IconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProgressColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ETAColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpeedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSizeTextColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeerColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ActionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MacColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn HashColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileSizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilePathColumn;
        private System.Windows.Forms.Label searchGridHelpTextLabel;
        private System.Windows.Forms.PictureBox syncStatusPictureBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesFileNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesFileSizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesStatusColumn;
        private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesFileHashColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesTransferIDColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BouncesUploaderMacColumn;
    }
}

