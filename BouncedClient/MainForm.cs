﻿using Bounced;
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
        
        //Makes sure in-progress transfers are not re-added
        
        public MainForm()
        {
            InitializeComponent();
            Transfers.outstandingDownloadRequests = new List<DownloadRequest>();
            Transfers.currentDownloads = new List<DownloadProgress>();
            Transfers.pendingToDownload = new ConcurrentDictionary<PendingResponse, DownloadProgress>(new PendingResponse.EqualityComparer());
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            loadConfigWorker.RunWorkerAsync();
            loadIndexWorker.RunWorkerAsync();
            Utils.clearLog();
        }

        private void registerWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient("http://"+Configuration.server);
            RestRequest request = new RestRequest("register", Method.POST);

            request.AddParameter("mac", Utils.GetMACAddress());
            request.AddParameter("nick", Configuration.username);
            request.AddParameter("space_allocated", "123");

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        private void registerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = (StatusResponse)e.Result;

            if (sr == null || sr.status==null)
            {
                Utils.writeLog("registerWorker_RunWorkerCompleted: Error in registering");
                //TODO: Put a timeout to start retry
                statusLabel.Text = "Unable to connect";
                actionButton.Enabled = true;
                reconnectTimer.Enabled = true;
                return;
            }

            statusLabel.Text = sr.text;

            //Happens in both cases - if disconnected, will attempt to reconnect.
            
            if (sr.status.Equals("OK"))
            {
                Utils.writeLog("registerWorker_RunWorkerCompleted: Registered successfully");
                pollPendingTimer.Enabled = true;
                reconnectTimer.Enabled = false;
            }
            else
            {
                Utils.writeLog("registerWorker_RunWorkerCompleted: Could not register..");
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
                statusLabel.Text = "No connection to server";
                pollPendingTimer.Enabled = false;
                reconnectTimer.Enabled = true;
                return;
            }

            foreach (PendingResponse pr in latestPending)
            {
                if (!Transfers.pendingToDownload.ContainsKey(pr))
                {
                    Utils.writeLog("Added " + pr.fileName + " to download queue");
                    DownloadProgress dip = new DownloadProgress(pr);
                    Transfers.pendingToDownload[pr] = dip;

                    BackgroundWorker downloadWorker = new BackgroundWorker();
                    downloadWorker.WorkerReportsProgress = true;
                    downloadWorker.WorkerSupportsCancellation = true;
                    downloadWorker.DoWork += downloadWorker_DoWork;
                    downloadWorker.ProgressChanged += downloadWorker_ProgressChanged;
                    downloadWorker.RunWorkerCompleted += downloadWorker_RunWorkerCompleted;
                    //This is what is sent to the backgroundworker
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
            String fileHash = Transfers.download(currentWorker, downloadArgs.Item1, downloadArgs.Item2);

            e.Result = new Tuple<string, long>(fileHash, downloadArgs.Item2.transferID);
        }

        void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadProgress dp = e.UserState as DownloadProgress;

            updateDownloadStatus();

            int row = -1;
            
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
                Icon zipIcon = Icons.IconFromExtension(type);

                downloadGridView.Rows.Add(new object[] { zipIcon, dp.fileName, dp.status, e.ProgressPercentage + "%", 
            0, "Unknown", Utils.getHumanSize(dp.fileSize), dp.uploaderIP, "Cancel", dp.mac, dp.hash, dp.fileSize, dp.downloadedFilePath });
                return;
            }

            downloadGridView["SpeedColumn", row].Value = dp.transferRate + " KB/s";
            downloadGridView["StatusColumn", row].Value = dp.status;
            downloadGridView["ProgressColumn", row].Value = e.ProgressPercentage + "%";
            downloadGridView["PeerColumn", row].Value = dp.nick;

            if (dp.isComplete)
            {
                downloadGridView["ActionColumn", row].Value = "Open folder";
            }

            double secondsToComplete = ((dp.fileSize - dp.completed) / 1024.0) / dp.averageTransferRate;
            if (secondsToComplete > 0)
                downloadGridView["ETAColumn", row].Value = Utils.getHumanTime(secondsToComplete);
            else
                downloadGridView["ETAColumn", row].Value = "";
        }

        void downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utils.writeLog("downloadWorker_RunWorkerCompleted: Download completed.");
            
            if (e.Error !=null)
            {
                Utils.writeLog("downloadWorker_RunWorkerCompleted: Error : " + e.Error);
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
                MainTabControl.SelectedIndex = 3;
                actionButton.Enabled = true;
                statusLabel.Text = "Invalid username";
                MessageBox.Show("Select a valid username, and check that at least " +
                    "one shared folder is present. When you're ready, press 'Reconnect'", "Can't connect to server");

                //Defaults for config should be set here

                serverTextBox.Text = "localhost:3000";
                downloadFolder.Text = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)
                    + "\\Bounced Downloads";
                sharedFolders.Items.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic));

                Configuration.downloadFolder = downloadFolder.Text;
                Configuration.server = serverTextBox.Text;
                Configuration.sharedFolders = new List<string>();
                Configuration.sharedFolders.Add(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyMusic));
                Configuration.saveConfiguration();
            }
            else
            {
                statusLabel.Text = "Loaded configuration successfully";
                registerWorker.RunWorkerAsync();

                usernameTextBox.Text = Configuration.username;
                downloadFolder.Text = Configuration.downloadFolder;
                serverTextBox.Text = Configuration.server;
                foreach (string sharedFolder in Configuration.sharedFolders)
                {
                    sharedFolders.Items.Add(sharedFolder);
                }

            }
        }

        private void actionButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Connecting..";
            registerWorker.RunWorkerAsync();
            actionButton.Enabled = false;
        }

        private void addFolderButton_Click(object sender, EventArgs e)
        {
            folderBrowser.ShowDialog();
            sharedFolders.Items.Add(folderBrowser.SelectedPath);
            Configuration.sharedFolders.Add(folderBrowser.SelectedPath);
            Configuration.saveConfiguration();
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
        }

        private void forceRescanButton_Click(object sender, EventArgs e)
        {
            forceRescanButton.Enabled = false;
            forceRescanButton.Text = "Indexing..";
            if(!indexWorker.IsBusy)
                indexWorker.RunWorkerAsync();
        }

        private void indexWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("indexWorker_DoWork: Starting to build index");
            Indexer.buildIndex(sharedFolders.Items);
        }

        private void indexWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            forceRescanButton.Enabled = true;
            forceRescanButton.Text = "Force Rescan";
            Utils.writeLog("indexWorker_RunWorkerCompleted: Starting syncWorker");
            syncWorker.RunWorkerAsync();
            statusLabel.Text = "Syncing..";
        }

        private void usernameTextBox_Leave(object sender, EventArgs e)
        {
            Configuration.username = usernameTextBox.Text;
            Configuration.saveConfiguration();
        }

        private void downloadFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            folderBrowser.ShowDialog();
            Configuration.downloadFolder = folderBrowser.SelectedPath;
            if (folderBrowser.SelectedPath != "")
            {
                downloadFolder.Text = folderBrowser.SelectedPath;
                Configuration.saveConfiguration();
            }
        }

        private void syncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("syncWorker_DoWork: Started sync..");

            // This should work, but doesn't

            /*
            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("xbsnrixb", Method.POST);

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
                return;
            }

            statusLabel.Text = sr.text;

            if (sr.status.Equals("OK"))
            {
                Indexer.successfulSync();
                Utils.writeLog("syncWorker_RunWorkerCompleted: Sync complete");
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
            }
        }

        public void performSearch(String searchQuery)
        {
            if (!searchWorker.IsBusy && searchGoButton.Enabled)
                searchWorker.RunWorkerAsync(searchQuery);

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

            if (currentlyDisplayedSearchResults.Count == 0)
            {
                MessageBox.Show("We were unable to find any results for your search query. Please rephrase and try again.",
                    "No results found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        private void searchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            repopulateSearchResults();
            searchGoButton.Enabled = true;
        }

        public void repopulateSearchResults()
        {
            searchGridView.Rows.Clear();

            foreach (SearchResult sr in currentlyDisplayedSearchResults)
            {
                //Utils.writeLog(sr.ToString());
                Icon zipIcon = Icons.IconFromExtension(sr.type);

                String buttonText = "Bounce";
                if (sr.online == true)
                    buttonText = "Download";

                searchGridView.Rows.Add(new object[] { zipIcon, sr.name, Utils.getHumanSize(sr.size), sr.nick, 
            buttonText, sr.mac, sr.hash, sr.size });
            }
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
                dr.type = ((String)buttonCell.Value).Equals("Download") ? "direct":"firstleg";
                dr.fileName = (String)fileNameCell.Value;
                dr.fileSize = (long)sizeCell.Value;

                Utils.writeLog("searchGridView_CellClick: Sending download request for file : " + dr.fileName);

                Transfers.outstandingDownloadRequests.Add(dr);
                updateDownloadStatus();
                downloadRequestWorker.RunWorkerAsync(dr);
            }

        }

        private void updateDownloadStatus()
        {
            downloadStatusLabel.Text = Transfers.outstandingDownloadRequests.Count + 
                " downloads pending, " + Transfers.currentDownloads.Count + " in progress.";
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

            if(sr!=null)
                Utils.writeLog("downloadRequestWorker_DoWork: Download request returned : " + sr.ToString());
            else
                Utils.writeLog("downloadRequestWorker_DoWork: Download request returned null");
        }

        private void serverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Server.serverWorker_DoWork(sender, e);
        }

        private void reconnectTimer_Tick(object sender, EventArgs e)
        {
            if (!registerWorker.IsBusy)
            {
                registerWorker.RunWorkerAsync();
            }
        }

        private void downloadGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv != null && e.ColumnIndex==8 && e.RowIndex>-1)
            {
                DataGridViewButtonCell buttonCell = dgv.Rows[e.RowIndex].Cells["ActionColumn"] as DataGridViewButtonCell;

                String action = (String)buttonCell.Value;

                if (action == "Clear")
                {
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

                Utils.writeLog("downloadGridView_CellClick: Canceling download");

                DataGridViewTextBoxCell macCell = dgv.Rows[e.RowIndex].Cells["MacColumn"] as DataGridViewTextBoxCell;
                DataGridViewTextBoxCell hashCell = dgv.Rows[e.RowIndex].Cells["HashColumn"] as DataGridViewTextBoxCell;

                String canceledMac = (String)macCell.Value;
                String canceledHash = (String)hashCell.Value;

                foreach (PendingResponse pr in Transfers.pendingToDownload.Keys)
                {
                    if (pr.uploader == canceledMac && pr.fileHash == canceledHash)
                    {
                        //TODO: Update cancellation here
                        Transfers.pendingToDownload[pr] = null;
                        buttonCell.Value = "Clear";
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

    }
}
