using BounceClient.Properties;
using BouncedClient;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BounceClient
{
    public partial class WelcomeForm : Form
    {
        public String username;
        public bool verified = false;
        private bool serverOK = true;
        private bool sharedFolderShown = false;
        
        public WelcomeForm()
        {
            InitializeComponent();
            username = "";
        }

        private void WelcomeForm_Load(object sender, EventArgs e)
        {

        }

        private void beginButton_Click(object sender, EventArgs e)
        {
            if (sharedFolderShown)
            {
                username = usernameTextBox.Text;
                this.Close();
                return;
            }

            if (!verified && serverOK)
            {
                beginButton.Enabled = false;
                usernameTextBox.Enabled = false;
                beginButton.Text = "Checking..";
                statusPictureBox.Image = Resources.connection_working;
                checkWorker.RunWorkerAsync();
                return;
            }

            licenseLinkLabel.Visible = false;
            licenseLabel.Visible = false;
            usernameTextBox.Enabled = false;
            checkStatusLabel.Visible = false;

            usernameHelpTextlabel.Text = "We've added your 'Videos' folder to the\nshared list. Remember to add to or modify your" +
                "\nshared folder list in the settings tab.";
            
            if(!serverOK)
            {
                licenseLabel.Visible = true;
                licenseLabel.Text = "We couldn't verify that your username was \navailable. We'll keep trying to connect.";
            }

            beginButton.Text = "Finish »";
            helloLabel.Text = "Almost done.";
            sharedFolderShown = true;
        }

        private void checkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("checkuser", Method.POST);

            request.AddParameter("mac", Utils.getMACAddress());
            request.AddParameter("nick", usernameTextBox.Text);

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        private void checkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = (StatusResponse)e.Result;

            if (sr == null || sr.status == null)
            {
                Utils.writeLog("checkWorker_RunWorkerCompleted: Error in checking username");
                serverOK = false;
                checkStatusLabel.Text = "Couldn't connect to server";
                beginButton.Text = "Continue »";
                beginButton.Enabled = true;
                return;
            }
            else if (sr.status.Equals("OK"))
            {
                statusPictureBox.Image = Resources.connection_done;
                Utils.writeLog("checkWorker_RunWorkerCompleted: Username available");
                verified = true;
                username = usernameTextBox.Text;
                beginButton.Text = "Continue »";
                checkStatusLabel.Text = sr.text;
                usernameTextBox.Enabled = true;
            }
            else
            {
                statusPictureBox.Image = null;
                Utils.writeLog("registerWorker_RunWorkerCompleted: Username not available");
                verified = false;
                checkStatusLabel.Text = sr.text;
                usernameTextBox.Enabled = true;
            }

            beginButton.Enabled = true;
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (verified)
                verified = false;

            beginButton.Text = "Begin »";
        }

        private void licenseLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://raw.github.com/raghavsethi/bounced/master/LICENSE");
            Process.Start(sInfo);
        }
    }
}
