using Bounced;
using RestSharp;
using RestSharp.Contrib;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncedClient
{
    public partial class MainForm : Form
    {
        List<SearchResult> currentlyDisplayedSearchResults;
        List<DownloadRequest> outstandingDownloadRequests;
        List<DownloadRequest> currentDownloads;

        ConcurrentDictionary<PendingResponse, DownloadProgress> pendingToDownload;
        
        public MainForm()
        {
            InitializeComponent();
            outstandingDownloadRequests = new List<DownloadRequest>();
            currentDownloads = new List<DownloadRequest>();
            pendingToDownload = new ConcurrentDictionary<PendingResponse, DownloadProgress>();
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

            request.AddParameter("mac", "raghav"); //TODO: Change this
            request.AddParameter("nick", "raghav");
            request.AddParameter("space_allocated", "123");

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        private void registerWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = (StatusResponse)e.Result;

            if (sr == null)
            {
                //TODO: Put a timeout on retrying
                statusLabel.Text = "Unable to connect";
                actionButton.Enabled = true;
                return;
            }

            statusLabel.Text = sr.text;
            if(sr.status.Equals("OK"))
                pollPendingTimer.Enabled=true;
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

            //TODO: Handle latestpending=null

            foreach (PendingResponse pr in latestPending)
            {
                if (!pendingToDownload.ContainsKey(pr))
                {
                    DownloadProgress dip = new DownloadProgress();

                }
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
                statusLabel.Text = "Select a valid username, and check that at least " +
                    "one shared folder is present before reconnecting";
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
            downloadFolder.Text = folderBrowser.SelectedPath;
            Configuration.saveConfiguration();
        }

        private void syncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("syncWorker_DoWork: Started sync..");

            RestClient client = new RestClient("http://"+Configuration.server);
            RestRequest request = new RestRequest("sync", Method.POST);

            request.AddParameter("added", Indexer.getAddedJson());
            request.AddParameter("removed", Indexer.getRemovedJson());

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        private void syncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = (StatusResponse)e.Result;

            if (sr == null || sr.status==null)
            {
                statusLabel.Text = "Unable to sync";
                Utils.writeLog("syncWorker_RunWorkerCompleted: Sync failed");
                return;
            }

            statusLabel.Text = sr.text;

            //Clear change tables
            if (sr.status.Equals("OK"))
            {
                Indexer.addedFiles = new Hashtable();
                Indexer.removedFiles = new Hashtable();
                Utils.writeLog("syncWorker_RunWorkerCompleted: Sync complete");
            }
            
        }

        private void loadIndexWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Indexer.deserializeHashTable();
        }

        private void loadIndexWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Utils.writeLog("loadIndexWorker_RunWorkerCompleted: loadIndex completed");

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
                Utils.writeLog(sr.ToString());
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

                outstandingDownloadRequests.Add(dr);
                updateDownloadStatus();
                downloadRequestWorker.RunWorkerAsync(dr);
            }

        }

        private void updateDownloadStatus()
        {
            downloadStatusLabel.Text = outstandingDownloadRequests.Count + 
                " downloads pending, " + currentDownloads.Count + " in progress.";
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

            Utils.writeLog("downloadRequestWorker_DoWork: Download request returned : " + sr.ToString());
        }


    }
}
