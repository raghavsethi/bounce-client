using BounceClient;
using BounceClient.Properties;
using Bounced;
using RestSharp;
using RestSharp.Contrib;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncedClient
{
    public partial class MainForm : Form
    {
        List<SearchResult> currentlyDisplayedSearchResults;
        bool textboxesInitialized; // Used to make sure textChanged events don't fire during initialization
        bool syncPending; // Used to keep track of whether a sync has failed and can be completed when online next
        bool reconnectBlinking, forceRescanBlinking;
        bool minimizingToTray; // To keep track of whether the close call should show close completely or minimize

        public MainForm()
        {
            textboxesInitialized = false;
            syncPending = false;
            reconnectBlinking = false;
            forceRescanBlinking = false;
            minimizingToTray = true;

            InitializeComponent();

            Downloads.currentDownloadsCount = 0;
            Server.currentUploadsCount = 0;

            Downloads.currentDownloads = new List<DownloadProgress>();
            Downloads.pendingToDownload = new ConcurrentDictionary<PendingResponse, DownloadProgress>(new PendingResponse.EqualityComparer());

            // Create bounce folder in APPDATA and write the hasher executable to it
            if (!Directory.Exists(Utils.getAppDataPath("")))
            {
                Directory.CreateDirectory(Utils.getAppDataPath(""));
                Utils.writeHasherToDisk();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadConfigWorker.RunWorkerAsync();
            loadIndexWorker.RunWorkerAsync();
            Utils.clearLog();
            macAddrLabel.Text = "MAC Address: " + Utils.getMACAddress();
            helpText5.Text = "Bounced Client v" + Utils.getVersion() + ". Built with love at IIIT-D by Raghav Sethi, Naved Alam and Mayank Pundir.";
            
            // Generate tray icon
            notifyIcon.Visible = true;
            
            ContextMenu notifyIconMenu = new ContextMenu();
            MenuItem showMenuItem = new MenuItem("S&how", new System.EventHandler(showMenuItem_Click));
            notifyIconMenu.MenuItems.Add(showMenuItem);
            MenuItem exitMenuItem = new MenuItem("E&xit", new System.EventHandler(exitMenuItem_Click));
            notifyIconMenu.MenuItems.Add(exitMenuItem);
            
            notifyIcon.ContextMenu = notifyIconMenu;
        }

        private void exitMenuItem_Click(object sender, System.EventArgs e)
        {
            notifyIcon.Visible = false;
            minimizingToTray = false;
            MainForm_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
        }

        private void showMenuItem_Click(object sender, System.EventArgs e)
        {
            this.Visible = true;
        }

        private void registerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient("http://"+Configuration.server);
            RestRequest request = new RestRequest("register", Method.POST);

            request.AddParameter("mac", Utils.getMACAddress());
            request.AddParameter("nick", Configuration.username);
            request.AddParameter("space_allocated", "123"); // TODO: Functionality to be added later.
            request.AddParameter("version", Utils.getVersion());

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        private void registerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = (StatusResponse)e.Result;

            // Happens in both cases - if disconnected, will attempt to reconnect.
            if (sr == null || sr.status==null)
            {
                Utils.writeLog("registerWorker_RunWorkerCompleted: Error in registering");
                statusPictureBox.Image = Resources.connection_working;
                mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
                actionButton.Enabled = true;
                reconnectTimer.Enabled = true;
                return;
            }
           
            if (sr.status.Equals("OK"))
            {
                statusPictureBox.Image = Resources.connection_done;
                mainToolTip.SetToolTip(statusPictureBox, "Connected");
                statusLabel.Text = "Connected";
                Utils.writeLog("registerWorker_RunWorkerCompleted: Registered successfully");
                pollPendingTimer.Enabled = true;
                reconnectTimer.Enabled = false;

                // If we had a failed sync, and are now connected
                if (syncPending && !syncWorker.IsBusy)
                {
                    syncWorker.RunWorkerAsync();
                }

            }
            else
            {
                statusPictureBox.Image = Resources.connection_working;
                mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
                statusLabel.Text = sr.text;
                Utils.writeLog("registerWorker_RunWorkerCompleted: Could not register..");
                actionButton.Enabled = true;
                reconnectTimer.Enabled = true;
            }
        }

        private void pollPendingTimer_Tick(object sender, EventArgs e)
        {
            if(!pollPendingWorker.IsBusy)
                pollPendingWorker.RunWorkerAsync();
        }

        private void pollPendingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient("http://"+Configuration.server);
            client.Timeout = 5000;
            RestRequest request = new RestRequest("pending", Method.GET);

            RestResponse<List<PendingResponse>> response =
                (RestResponse<List<PendingResponse>>)client.Execute<List<PendingResponse>>(request);

            e.Result = response.Data;
        }

        private void pollPendingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<PendingResponse> latestPending = (List<PendingResponse>)e.Result;

            if (latestPending == null)
            {
                statusPictureBox.Image = Resources.connection_working;
                mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
                statusLabel.Text = "Lost connection to network";
                pollPendingTimer.Enabled = false;
                reconnectTimer.Enabled = true;
                Utils.writeLog("pollPendingWorker_RunWorkerCompleted: Received null response from server");
                return;
            }

            foreach (PendingResponse pr in latestPending)
            {
                if (pr == null || pr.type == null)
                {
                    Utils.writeLog("pollPendingWorker_RunWorkerCompleted: Received a null pending");
                    continue;
                }
                if (pr.type.Equals("delete"))
                {
                    Utils.writeLog("pollPendingWorker_RunWorkerCompleted: Got delete request for " + pr.fileHash + " (" + pr.fileName + ") ");
                    String filePath = Utils.getAppDataPath(@"\Bounces\" + pr.fileHash + ".bounced");
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (Exception e2)
                    {
                        //TODO: Do nothing.
                    }

                    // Tell the server we have deleted the file
                    BackgroundWorker updateWorker = new BackgroundWorker();
                    updateWorker.DoWork += Downloads.updateWorker_DoWork;
                    updateWorker.RunWorkerCompleted += Downloads.updateWorker_RunWorkerCompleted;

                    // Set update parameters and kick off update
                    UpdateRequest ur = new UpdateRequest();
                    ur.transferID = pr.transferID;
                    ur.status = "done";
                    ur.uploader = pr.uploader;
                    updateWorker.RunWorkerAsync(ur);

                    continue;
                }
                
                bool performPending = false;

                // Checking if the same file is already being downloaded by the client
                if (!Downloads.pendingToDownload.ContainsKey(pr))
                {
                    foreach(PendingResponse existingPending in Downloads.pendingToDownload.Keys)
                    {
                        if (existingPending.fileHash.Equals(pr.fileHash))
                            return;
                    }
                    performPending = true;
                }

                if (performPending)
                {
                    Utils.writeLog("Added " + pr.fileName + " to download queue");
                    DownloadProgress dip = new DownloadProgress(pr);
                    Downloads.pendingToDownload[pr] = dip;

                    BackgroundWorker downloadWorker = new BackgroundWorker();
                    downloadWorker.WorkerReportsProgress = true;
                    downloadWorker.WorkerSupportsCancellation = true;
                    downloadWorker.DoWork += downloadWorker_DoWork;
                    downloadWorker.ProgressChanged += downloadWorker_ProgressChanged;
                    downloadWorker.RunWorkerCompleted += downloadWorker_RunWorkerCompleted;
                    
                    // This is what is sent to the backgroundworker
                    Tuple<PendingResponse, DownloadProgress> downloadArgs = 
                        new Tuple<PendingResponse, DownloadProgress>(pr, dip);

                    downloadWorker.RunWorkerAsync(downloadArgs);
                }
            }
        }

        void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker currentWorker = sender as BackgroundWorker;
            Tuple<PendingResponse, DownloadProgress> downloadArgs = e.Argument as Tuple<PendingResponse, DownloadProgress>;

            lock (Downloads.downloadCountLock)
            {
                Downloads.currentDownloadsCount++;
            }
            
            String fileHash = Downloads.download(currentWorker, downloadArgs.Item1, downloadArgs.Item2, 0);
            while(fileHash!=null && fileHash.Equals("FAILED"))
            {
                fileHash = Downloads.download(currentWorker, downloadArgs.Item1, downloadArgs.Item2, 
                    downloadArgs.Item2.bytesDownloaded);
            }

            e.Result = new Tuple<string, long>(fileHash, downloadArgs.Item2.transferID);
        }

        void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadProgress dp = e.UserState as DownloadProgress;
            int row = -1;

            // Don't need to update the UI for transfers that are invisible
            if (!dp.visible)
                return;

            // Identify which row contains the download whose progress is being reported
            for (int i = 0; i < downloadGridView.RowCount; i++)
            {
                if ((((String)(downloadGridView["HashColumn", i].Value)).Equals(dp.hash))
                    && (((String)(downloadGridView["MacColumn", i].Value)).Equals(dp.mac)))
                {
                    row = i;
                    break;
                }
            }

            // This download is not yet shown in the UI
            if(row == -1)
            {
                String type = dp.fileName.Substring(dp.fileName.LastIndexOf('.') + 1);
                Icon zipIcon = null;
                
                try
                {
                    zipIcon = Icons.IconFromExtension(type);
                }
                catch (Exception e2)
                {
                }

                downloadGridView.Rows.Add(new object[] { zipIcon, dp.fileName, dp.status, e.ProgressPercentage + "%", 
            0, "Unknown", Utils.getHumanSize(dp.fileSize), dp.uploaderIP, "Cancel", Resources.remove_file_locked, dp.mac, 
            dp.hash, dp.fileSize, dp.downloadedFilePath, "false" });

                // To kick off UI update instantly
                uiUpdateTimer_Tick(sender, e);

                return;
            }

            downloadGridView["SpeedColumn", row].Value = Utils.getHumanSpeed(dp.transferRate);
            downloadGridView["StatusColumn", row].Value = dp.status;
            downloadGridView["ProgressColumn", row].Value = e.ProgressPercentage + "%";
            downloadGridView["PeerColumn", row].Value = dp.nick;
            downloadGridView["FilePathColumn", row].Value = dp.downloadedFilePath;

            // Handles the case when a download is canceled, then restarted
            // It makes sure that the new download has a 'Cancel' button rather than a 'Clear' button
            if (!dp.status.Equals("Canceled") && downloadGridView["ActionColumn", row].Value.Equals("Clear")) // TODO: Finicky, change later
            {
                downloadGridView["ActionColumn", row].Value = "Cancel";
            }

            if (dp.isComplete)
            {
                downloadGridView["ActionColumn", row].Value = "Open folder";
                downloadGridView["RemoveColumn", row].Value = Resources.remove_file;
                downloadGridView["RemovableColumn", row].Value = "true";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipTitle = "Download complete   ";
                notifyIcon.BalloonTipText = (String)downloadGridView["FileNameColumn", row].Value;
                notifyIcon.ShowBalloonTip(5000);
            }

            if (dp.isFailedUnrecoverably)
            {
                downloadGridView["RemoveColumn", row].Value = Resources.remove_file;
                downloadGridView["RemovableColumn", row].Value = "true";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                notifyIcon.BalloonTipTitle = "Download failed   ";
                notifyIcon.BalloonTipText = (String)downloadGridView["FileNameColumn", row].Value;
                notifyIcon.ShowBalloonTip(5000);
            }
            
            double secondsToComplete = ((dp.fileSize - dp.bytesDownloaded) / 1024.0) / dp.averageTransferRate;

            if (secondsToComplete > 0)
                downloadGridView["ETAColumn", row].Value = Utils.getHumanTime(secondsToComplete);
            else
                downloadGridView["ETAColumn", row].Value = "";
        }

        void downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utils.writeLog("downloadWorker_RunWorkerCompleted: Worker completed job.");

            Tuple<string, long> downloadResult = e.Result as Tuple<string, long>;

            foreach (DownloadProgress dp in Downloads.currentDownloads)
            {
                if (dp.hash == downloadResult.Item1)
                {
                    Downloads.currentDownloads.Remove(dp);
                    break;
                }
            }

            if (e.Error !=null)
            {
                Utils.writeLog("downloadWorker_RunWorkerCompleted: Error : " + e.Error);
            }

            lock (Downloads.downloadCountLock)
            {
                Downloads.currentDownloadsCount--;
            }
        }

        private void loadConfigWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = Configuration.loadConfiguration();
        }

        private void loadConfigWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!(bool)e.Result)
            {
                // First run behaviour

                /*
                actionButton.Enabled = true;
                statusLabel.Text = "Invalid username";
                MessageBox.Show("Select a valid username, and check that at least " +
                    "one shared folder is present. When you're ready, press 'Reconnect'", "Can't connect to server");
                */

                //Defaults for config should be set here

                String defaultDownloadFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
                    + "\\Bounce Downloads";

                serverTextBox.Text = "192.168.1.40:3000";
                downloadFolder.Text = defaultDownloadFolder;
                sharedFolders.Items.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos));
                sharedFolders.Items.Add(defaultDownloadFolder);

                Configuration.downloadFolder = defaultDownloadFolder;
                Configuration.server = serverTextBox.Text;
                Configuration.sharedFolders = new List<string>();
                Configuration.sharedFolders.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos));
                Configuration.sharedFolders.Add(defaultDownloadFolder);

                WelcomeForm welcomeForm = new WelcomeForm();
                welcomeForm.ShowDialog();

                if(!welcomeForm.verified)
                    MainTabControl.SelectedIndex = 3;

                usernameTextBox.Text = welcomeForm.username;
                Configuration.username = usernameTextBox.Text;
                Configuration.saveConfiguration();
            }
            else
            {
                statusLabel.Text = "Loaded configuration successfully";

                usernameTextBox.Text = Configuration.username;
                downloadFolder.Text = Configuration.downloadFolder;
                serverTextBox.Text = Configuration.server;
                foreach (string sharedFolder in Configuration.sharedFolders)
                {
                    sharedFolders.Items.Add(sharedFolder);
                }

            }

            textboxesInitialized = true;
            registerWorker.RunWorkerAsync();
            statusPictureBox.Image = Resources.connection_working;
            mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
            statusLabel.Text = "Connecting..";
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            if (folderBrowser.SelectedPath != "")
            {
                sharedFolders.Items.Add(folderBrowser.SelectedPath);
                Configuration.sharedFolders.Add(folderBrowser.SelectedPath);
                Configuration.saveConfiguration();
            }
            indexWorker.CancelAsync();
            forceRescanButton.Font = new Font(forceRescanButton.Font, FontStyle.Bold);
            forceRescanBlinking = true;
            uiBlinkTimer.Enabled = true;
        }
        
        private void deleteSelectedButton_Click(object sender, EventArgs e)
        {
            List<int> checkedItems = new List<int>();
            foreach (int deletedIndex in sharedFolders.SelectedIndices)
            {
                checkedItems.Add(deletedIndex);
            }
            foreach (int deletedIndex in checkedItems)
            {
                sharedFolders.Items.RemoveAt(deletedIndex);
                Configuration.sharedFolders.RemoveAt(deletedIndex);
            }
            Configuration.saveConfiguration();
            indexWorker.CancelAsync();
            forceRescanButton.Font = new Font(forceRescanButton.Font, FontStyle.Bold);
            forceRescanBlinking = true;
            uiBlinkTimer.Enabled = true;
        }

        private void forceRescanButton_Click(object sender, EventArgs e)
        {
            forceRescanButton.Font = new Font(forceRescanButton.Font, FontStyle.Regular);
            forceRescanBlinking = false;

            forceRescanButton.Enabled = false;
            forceRescanButton.Text = "Indexing..";
            if (!indexWorker.IsBusy)
            {
                indexWorker.RunWorkerAsync();
                syncStatusPictureBox.Image = Resources.sync_working;
                mainToolTip.SetToolTip(syncStatusPictureBox, "Indexing your shared folders..");
            }
        }

        private void indexWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("indexWorker_DoWork: Starting to build index");
            
            Indexer.buildIndex(sharedFolders.Items, indexWorker, e);
        }

        private void indexWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            forceRescanButton.Enabled = true;
            forceRescanButton.Text = "Force Rescan";

            if (e.Cancelled)
            {
                Utils.writeLog("indexWorker_RunWorkerCompleted: Indexing was cancelled, not syncing.");
                syncStatusPictureBox.Image = Resources.sync_failed;
                return;
            }

            Utils.writeLog("indexWorker_RunWorkerCompleted: Starting syncWorker");
            syncWorker.RunWorkerAsync();
            statusLabel.Text = "Syncing..";
            syncStatusPictureBox.Image = Resources.sync_sending;
            mainToolTip.SetToolTip(syncStatusPictureBox, "Syncing the file list with server..");
        }


        private void usernameTextBox_Leave(object sender, EventArgs e)
        {
            Configuration.username = usernameTextBox.Text;
            Configuration.saveConfiguration();
        }

        private void downloadFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Configuration.downloadFolder);
        }

        private void syncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("syncWorker_DoWork: Started sync..");
            // This should work, but I don't know for the life of me why it doesn't

            /*
            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("sync", Method.POST);

            request.AddParameter("added", Indexer.getAddedJson());
            request.AddParameter("removed", Indexer.getRemovedJson());

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);
            e.Result = response.Data;
            */
            
            using (var wb = new WebClient())
            {
                var data = new NameValueCollection();
                data["added"] = Indexer.getAddedJson();
                data["removed"] = Indexer.getRemovedJson();

                String response = null;

                try
                {
                    response = Encoding.ASCII.GetString(wb.UploadValues("http://" + Configuration.server + "/sync", "POST", data));
                    //MessageBox.Show(response);
                }
                catch (Exception we)
                {
                    Utils.writeLog("syncWorker_DoWork: " + we);
                    response = "";
                }
                StatusResponse sr = new StatusResponse();
                sr.status = (response.Contains("OK")) ? "OK" : "Error";
                sr.text = (response.Contains("OK")) ? "Synced successfully" : "Failed to sync";

                if (response == "")
                    sr = null;

                syncStatusPictureBox.Image = Resources.sync_failed;
                e.Result = sr;
            }
            
        }

        private void syncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = e.Result as StatusResponse;

            if (sr == null || sr.status==null)
            {
                statusLabel.Text = "Unable to sync";
                Utils.writeLog("syncWorker_RunWorkerCompleted: Sync failed");
                syncStatusPictureBox.Image = Resources.sync_failed;
                mainToolTip.SetToolTip(syncStatusPictureBox, "Sync with server failed");
                syncPending = true;
                return;
            }

            statusLabel.Text = sr.text;

            if (sr.status.Equals("OK"))
            {
                Indexer.successfulSync();
                Utils.writeLog("syncWorker_RunWorkerCompleted: Sync complete");
                syncStatusPictureBox.Image = Resources.sync_successful;
                mainToolTip.SetToolTip(syncStatusPictureBox, "Successfully synced");
                syncPending = false;
            }
            
        }

        private void loadIndexWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Indexer.deserializeHashTables();
        }

        private void loadIndexWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utils.writeLog("loadIndexWorker_RunWorkerCompleted: loadIndex completed");
            serverWorker.RunWorkerAsync();
            Utils.writeLog("loadIndexWorker_RunWorkerCompleted: Kicking off index job");
            indexWorker.RunWorkerAsync();
            syncStatusPictureBox.Image = Resources.sync_working;
            mainToolTip.SetToolTip(syncStatusPictureBox, "Indexing your shared folders..");
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text != "")
                searchHelpText.Visible = false;
            else
                searchHelpText.Visible = true;
        }

        private void searchHelpText_Click(object sender, EventArgs e)
        {
            searchBox.Focus();
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13 && searchBox.Text != "")
            {
                performSearch(searchBox.Text);
                e.Handled = true; // To prevent the annoying ding sound when pressing enter
            }
        }

        private void searchGoButton_Click(object sender, EventArgs e)
        {
            performSearch(searchBox.Text);
        }

        public void performSearch(String searchQuery)
        {
            if (!searchWorker.IsBusy && searchGoButton.Enabled)
            {
                searchGridHelpTextLabel.Visible = false;
                searchGridView.Rows.Clear();
                searchWorker.RunWorkerAsync(searchQuery);
            }
            searchGoButton.Enabled = false;
        }

        private void searchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String searchQuery = (String)e.Argument;

            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("search/" + HttpUtility.UrlEncode(searchQuery), Method.GET);

            RestResponse<List<SearchResult>> response =
               (RestResponse<List<SearchResult>>)client.Execute<List<SearchResult>>(request);

            currentlyDisplayedSearchResults = (List<SearchResult>)response.Data;

            if (currentlyDisplayedSearchResults == null)
            {
                MessageBox.Show("Unable to talk to the search service. Please try again later.",
                    "No results found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentlyDisplayedSearchResults = new List<SearchResult>();
                return;
            }
        }
        
        private void searchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            repopulateSearchResults();
            searchGoButton.Enabled = true;

            if(searchGridView.Rows.Count==0)
                searchGridHelpTextLabel.Visible = true;
        }

        public void repopulateSearchResults()
        {
            searchGridView.Rows.Clear();

            Dictionary<String, Icon> extToIcon = new Dictionary<string, Icon>();
            
            foreach (SearchResult sr in currentlyDisplayedSearchResults)
            {
                Icon zipIcon = null;
                if (!extToIcon.ContainsKey(sr.type))
                {
                    try
                    {
                        zipIcon = Icons.IconFromExtension(sr.type);
                    }
                    catch (Exception e)
                    { }
                    extToIcon[sr.type] = zipIcon;
                }
                else
                {
                    zipIcon = extToIcon[sr.type];
                }

                String buttonText = "Bounce";
                if (sr.online == true)
                    buttonText = "Download";

                searchGridView.Rows.Add(new object[] { zipIcon, sr.name, Utils.getHumanSize(sr.size), sr.nick, 
            buttonText, sr.mac, sr.hash, sr.size });

            }

            GC.Collect();
            Utils.writeLog("repopulateSearchResults: Memory used after repop and GC: " + GC.GetTotalMemory(true) / 1024.0);
        }

        private void searchGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && e.ColumnIndex==4 && e.RowIndex>-1)
            {
                DataGridViewTextBoxCell fileNameCell = dgv.Rows[e.RowIndex].Cells[1] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell macCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + 1] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell hashCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + 2] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell sizeCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + 3] as DataGridViewTextBoxCell;
                DataGridViewButtonCell buttonCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;

                BackgroundWorker downloadRequestWorker = new BackgroundWorker();
                downloadRequestWorker.DoWork += new DoWorkEventHandler(downloadRequestWorker_DoWork);

                DownloadRequest dr = new DownloadRequest();
                dr.mac = (String)macCell.Value;
                dr.hash = (String)hashCell.Value;
                dr.type = ((String)buttonCell.Value).Equals("Download") ? "direct":"bounced";
                dr.fileName = (String)fileNameCell.Value;
                dr.fileSize = (long)sizeCell.Value;

                Utils.writeLog("searchGridView_CellClick: Sending download request for file : " + dr.fileName);

                downloadRequestWorker.RunWorkerAsync(dr);

                // Switch the tab to show the new download/bounce
                MainTabControl.SelectedIndex = ((String)buttonCell.Value).Equals("Download") ? 1 : 2;
            }

        }

        private void downloadRequestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("downloadRequestWorker_DoWork: Sending download request");

            DownloadRequest dr = e.Argument as DownloadRequest;
            
            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("download", Method.POST);

            request.AddParameter("mac", dr.mac); //TODO: Change this
            request.AddParameter("filehash", dr.hash);
            request.AddParameter("filename", dr.fileName);
            request.AddParameter("filesize", dr.fileSize);
            request.AddParameter("type", dr.type);

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            StatusResponse sr = response.Data as StatusResponse;

            if (sr != null)
            {
                Utils.writeLog("downloadRequestWorker_DoWork: Download request returned : " + sr.ToString());
                if (!sr.status.Equals("OK"))
                {
                    MessageBox.Show("Couldn't process the download request. Error: " + sr.text, "Download Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // To make the download appear in the UI faster
                    if (!pollPendingWorker.IsBusy)
                        pollPendingWorker.RunWorkerAsync();
                }
            }
            else
            {
                Utils.writeLog("downloadRequestWorker_DoWork: Download request returned null");
                MessageBox.Show("Couldn't process the download request. Error: The server failed to return a valid response", 
                    "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void serverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Server.serverWorker_DoWork(sender, e);
        }

        private void reconnectTimer_Tick(object sender, EventArgs e)
        {
            if (!registerWorker.IsBusy) // In the worst case, it will try again on next tick
            {
                registerWorker.RunWorkerAsync();
                statusPictureBox.Image = Resources.connection_working;
                mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
            }
        }

        private void downloadGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            // For remove icon
            if (dgv != null && e.ColumnIndex == 9 && e.RowIndex > -1)
            {
                DataGridViewImageCell removeCell = dgv.Rows[e.RowIndex].Cells["RemoveColumn"] as DataGridViewImageCell;
                if (dgv.Rows[e.RowIndex].Cells["RemovableColumn"].Value.Equals("true"))
                {
                    dgv.Rows.RemoveAt(e.RowIndex);
                    return;
                }

            }

            // For the full buttons
            if (dgv != null && e.ColumnIndex==8 && e.RowIndex>-1)
            {
                DataGridViewButtonCell buttonCell = dgv.Rows[e.RowIndex].Cells["ActionColumn"] as DataGridViewButtonCell;

                String action = (String)buttonCell.Value;

                if (action == "Clear")
                {
                    //TODO : Add this
                    dgv.Rows.RemoveAt(e.RowIndex);
                    return;
                }

                // Open explorer and highlight downloaded file
                if (action == "Open folder")
                {
                    string argument = "/select, \"" + dgv.Rows[e.RowIndex].Cells["FilePathColumn"].Value + "\"";
                    System.Diagnostics.Process.Start("explorer.exe", argument);
                    return;
                }

                // User clicked cancel
                Utils.writeLog("downloadGridView_CellClick: Canceling download");

                DataGridViewTextBoxCell macCell = dgv.Rows[e.RowIndex].Cells["MacColumn"] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell hashCell = dgv.Rows[e.RowIndex].Cells["HashColumn"] as DataGridViewTextBoxCell;

                String canceledMac = (String)macCell.Value;
                String canceledHash = (String)hashCell.Value;

                foreach (PendingResponse pr in Downloads.pendingToDownload.Keys)
                {
                    if (pr.uploader == canceledMac && pr.fileHash == canceledHash)
                    {
                        Downloads.pendingToDownload[pr] = null; // This effectively cancels the download
                        buttonCell.Value = "Clear";
                        downloadGridView["RemoveColumn", e.RowIndex].Value = Resources.remove_file;
                        downloadGridView["RemovableColumn", e.RowIndex].Value = "true";
                        break;
                    }
                }
            }

        }

        private void serverTextBox_Leave(object sender, EventArgs e)
        {
            Configuration.server = serverTextBox.Text;
            Configuration.saveConfiguration();
        }


        // In both the following cases, stop polling

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            serverTextBox_TextChanged(sender,e);
        }

        private void serverTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!textboxesInitialized) // To ensure TextChanged was fired by a user
                return;
            statusPictureBox.Image = null; //TODO: Put X image instead
            statusLabel.Text = "Click on the 'Reconnect' button to retry";
            mainToolTip.SetToolTip(statusPictureBox, "No connection"); 
            actionButton.Enabled = true;
            reconnectTimer.Enabled = false;
            pollPendingTimer.Enabled = false;
            actionButton.Font = new Font(actionButton.Font, FontStyle.Bold);
            reconnectBlinking = true;
            uiBlinkTimer.Enabled = true;
        }

        
        private void actionButton_Click(object sender, EventArgs e)
        {
            statusPictureBox.Image = Resources.connection_working;
            mainToolTip.SetToolTip(statusPictureBox, "Connecting..");
            actionButton.Font = new Font(actionButton.Font, FontStyle.Regular);
            reconnectBlinking = false;

            if (!registerWorker.IsBusy)
            {
                registerWorker.RunWorkerAsync();
                statusPictureBox.Image = Resources.connection_working;
                mainToolTip.SetToolTip(statusPictureBox, "Trying to connect..");
            }
            actionButton.Enabled = false;
            serverTextBox.ReadOnly = true;
        }

        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Bounces tab is selected
            if (MainTabControl.SelectedIndex == 2)
            {
                updateBounceStatus();
                bounceStatusTimer.Enabled = true;
            }
            else
            {
                bounceStatusTimer.Enabled = false;
            }
        }

        private void updateBounceStatus()
        {
            if (!pollStatusWorker.IsBusy)
                pollStatusWorker.RunWorkerAsync();
        }

        private void pollStatusWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("status", Method.GET);

            RestResponse<List<StatusResult>> response =
                (RestResponse<List<StatusResult>>)client.Execute<List<StatusResult>>(request);

            e.Result = response.Data;
        }

        private void pollStatusWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<StatusResult> bounceStatusResults = (List<StatusResult>)e.Result;

            if (bounceStatusResults == null)
            {
                Utils.writeLog("Unable to get status of bounce requests");
                bounceStatusTimer.Enabled = false;
                MessageBox.Show("Sorry, we were unable to get the status of your bounce requests. You may be disconnected from the server, or " + 
                "the server may have malfunctioned.", "Error getting bounce requests", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bounceGridView.Rows.Clear();
            String buttonText = "Cancel";
            String type = "";
            String status = "";
            Icon zipIcon = null;

            if (bounceStatusResults.Count == 0)
                bounceGridHelpTextLabel.Visible = true;
            else
                bounceGridHelpTextLabel.Visible = false;

            foreach (StatusResult sr in bounceStatusResults)
            {
                type = sr.fileName.Substring(sr.fileName.LastIndexOf('.') + 1);
                try
                {
                    zipIcon = Icons.IconFromExtension(type);
                }
                catch (Exception e2)
                {
                }
                status = "Replicated to " + sr.sent + " of " + sr.total + " users";
                bounceGridView.Rows.Add(new object[] { zipIcon, sr.fileName, Utils.getHumanSize(sr.fileSize), status, 
            buttonText, sr.hash, sr.transferID, sr.uploader});
            }
        }

        private void bounceStatusTimer_Tick(object sender, EventArgs e)
        {
            updateBounceStatus();
        }

        private void serverTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (!serverTextBox.ReadOnly)
                return;

            DialogResult x = MessageBox.Show("Using a malicious server may result in malware or other harmful content" +
                " being downloaded onto your computer.\n\n" +
                "The easiest way to be safe is to use the server address displayed on the web page controlled by " +
                "your administrator.\n\nAre you sure you want to change the server address?"
                , "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if(x.Equals(DialogResult.Yes))
                serverTextBox.ReadOnly = false;
        }

        // Will be used to perform simple, fast UI updates
        private void uiUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (downloadGridView.Rows.Count == 0)
                downloadGridHelpTextLabel.Visible = true;
            else
                downloadGridHelpTextLabel.Visible = false;

            downloadStatusLabel.Text = "" + Downloads.currentDownloadsCount;
            uploadStatusLabel.Text = "" + Server.currentUploadsCount;
        }

        private void bounceGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && e.ColumnIndex == 4 && e.RowIndex > -1)
            {
                DataGridViewTextBoxCell transferIdCell = dgv.Rows[e.RowIndex].Cells["BouncesTransferIDColumn"] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell macCell = dgv.Rows[e.RowIndex].Cells["BouncesUploaderMacColumn"] as DataGridViewTextBoxCell;

                // Tell the server to cancel bounce
                BackgroundWorker updateWorker = new BackgroundWorker();
                updateWorker.DoWork += Downloads.updateWorker_DoWork;
                updateWorker.RunWorkerCompleted += Downloads.updateWorker_RunWorkerCompleted;

                // Set update parameters and kick off update
                UpdateRequest ur = new UpdateRequest();
                ur.transferID = long.Parse(transferIdCell.Value.ToString());
                ur.status = "canceled";
                ur.uploader = (String)macCell.Value;
                updateWorker.RunWorkerAsync(ur);

                DataGridViewButtonCell buttonCell = dgv.Rows[e.RowIndex].Cells[4] as DataGridViewButtonCell;
                buttonCell.Value = "Canceling...";
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration.username = usernameTextBox.Text;
            Configuration.server = serverTextBox.Text;
            Configuration.saveConfiguration();

            if (minimizingToTray)
            {
                e.Cancel = true;
                this.Hide();

                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.BalloonTipTitle = "Bounce is running in the tray";
                notifyIcon.BalloonTipText = "To exit Bounce, right-click on the icon and select 'Exit'";
                notifyIcon.ShowBalloonTip(500);
                minimizingToTray = true;
            }
            else
            {
                this.Dispose();
                Application.Exit();
            }
        }

        private void viewLogLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string argument = "/select, \"" + Utils.getAppDataPath("log.txt");
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void searchGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == searchGridView.Columns["Action"].Index)
        && e.Value != null)
            {
                DataGridViewCell cell = searchGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (e.Value.Equals("Download"))
                {
                    cell.ToolTipText = "Downloads the file directly from the peer";
                }
                else if (e.Value.Equals("Bounce"))
                {
                    cell.ToolTipText = "Tells the server to transfer the file to you as soon as it is available";
                }
            }
        }

        private void helpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://muc.iiitd.edu.in/bounce/help.html");
        }

        private void homeLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://muc.iiitd.edu.in/bounce/");
        }

        private void reIndexTimer_Tick(object sender, EventArgs e)
        {
            if (forceRescanButton.Enabled)
            {
                forceRescanButton_Click(sender, e);
            }
        }

        private void uiBlinkTimer_Tick(object sender, EventArgs e)
        {
            if (!forceRescanBlinking && !reconnectBlinking)
                uiBlinkTimer.Enabled = false;

            if (forceRescanBlinking)
            {
                if (forceRescanButton.Font.Bold)
                    forceRescanButton.Font = new Font(forceRescanButton.Font, FontStyle.Regular);
                else
                    forceRescanButton.Font = new Font(forceRescanButton.Font, FontStyle.Bold);
            }

            if (reconnectBlinking)
            {
                if (actionButton.Font.Bold)
                    actionButton.Font = new Font(actionButton.Font, FontStyle.Regular);
                else
                    actionButton.Font = new Font(actionButton.Font, FontStyle.Bold);
            }
        }

        private void changeFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            Configuration.downloadFolder = folderBrowser.SelectedPath;
            if (folderBrowser.SelectedPath != "")
            {
                downloadFolder.Text = folderBrowser.SelectedPath;
                Configuration.saveConfiguration();
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            //if (this.Visible == false)
                this.Show();
        }

        private void searchGridHelpTextLabel_Click(object sender, EventArgs e)
        {

        }

    }
}
