using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncedClient
{
    class Transfers
    {
        public static ConcurrentDictionary<PendingResponse, DownloadProgress> pendingToDownload;
        public static List<DownloadProgress> currentDownloads;
        public static int currentDownloadsCount;

        public static System.Object downloadCountLock = new System.Object();

        public static string download(BackgroundWorker worker, PendingResponse pr, DownloadProgress dp)
        {
            TcpClient tcpClient = new TcpClient();
            //tcpClient.NoDelay = true;
            NetworkStream clientStream;
            
            try
            {
                tcpClient.Connect(pr.uploaderIP, 8002);

                clientStream = tcpClient.GetStream();
                Utils.writeLog("download: Sending download instruction to peer " + pr.uploaderIP + "...");

                //Format: fileHash | transfer ID | transfer-type
                Utils.writeLog("tid is "+ pr.transferID);
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(pr.fileHash + "|" + pr.transferID + "|" + pr.type);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                Utils.writeLog("download: Download started.");
            }
            catch (Exception e)
            {
                Utils.writeLog("download: Could not establish connection. Error : " + e);
                return null;
            }

            // Begin file transfer
            FileStream strLocal = null;

            Transfers.currentDownloads.Add(dp);

            // Add the download to the gridview
            worker.ReportProgress(0, dp);

            #region initvars

            long bytesDownloaded = 0;   //Total bytes downloaded for the file
            int bytesSize;              //Number of bytes read by the stream reader
            int tempTransferRate = 0;   //Instantaneous (for 1 refresh cycle) download rate
            long downloadedInCycle = 0; //Total bytes downloaded in the last refresh cycle

            DateTime startTime = DateTime.Now;  //To track total download time
            DateTime refresh = DateTime.Now;    //To track time since last refresh

            byte[] downBuffer = new byte[4096];

            // Find a free filename

            if (File.Exists(dp.downloadedFilePath))
            {
                String candidatePath = "";
                for (int i = 2; i < 100; i++) // 100 here is arbitrary, just want to make sure it doesnt loop forever
                {
                    candidatePath = dp.downloadedFilePath.Substring(0,dp.downloadedFilePath.LastIndexOf('.'));
                    candidatePath += " ("+i+")";
                    candidatePath += dp.downloadedFilePath.Substring(dp.downloadedFilePath.LastIndexOf('.'));

                    if (!File.Exists(candidatePath))
                    {
                        Utils.writeLog("download: Found free file path " + candidatePath);
                        dp.downloadedFilePath = candidatePath;
                        dp.fileName = dp.downloadedFilePath.Substring(dp.downloadedFilePath.LastIndexOf(@"\") + 1);
                        break;
                    }
                }
            }

            try
            {
                strLocal = new FileStream(dp.downloadedFilePath,
                    FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception e)
            {
                Utils.writeLog("download: Error creating file : " + e);
                // Attempt to recover from error
                String filePath = dp.downloadedFilePath;
                String folderPath = filePath.Substring(0, filePath.LastIndexOf("\\"));
                try
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                    strLocal = new FileStream(dp.downloadedFilePath,
                        FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                catch (Exception e2)
                {
                    Utils.writeLog("download: Unrecoverable error while creating folder to hold file");
                    MessageBox.Show("Could not download file as the download folder could not be found or created",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            #endregion

            try
            {
                // Perform file transfer.
                clientStream.ReadTimeout = 2000; // Need to timeout to prevent stalled appearance in UI
                while ((bytesSize = clientStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    // In case user cancels download
                    if (pendingToDownload[pr]==null || worker.CancellationPending)
                    {
                        Utils.writeLog("Download canceled by user");
                        dp.status = "Canceled";
                        worker.ReportProgress((int)(100 * bytesDownloaded / pr.fileSize), dp);
                        clientStream.Close();
                        strLocal.Close();

                        // Report cancellation to server
                        BackgroundWorker cancelUpdateWorker = new BackgroundWorker();
                        cancelUpdateWorker.DoWork += Transfers.updateWorker_DoWork;
                        cancelUpdateWorker.RunWorkerCompleted += Transfers.updateWorker_RunWorkerCompleted;

                        // Set update parameters
                        UpdateRequest cancelUpdateRequest = new UpdateRequest();
                        cancelUpdateRequest.newHash = dp.hash;
                        cancelUpdateRequest.transferID = dp.transferID;
                        cancelUpdateRequest.status = "canceled";
                        cancelUpdateRequest.uploader = dp.mac;
                        cancelUpdateWorker.RunWorkerAsync(cancelUpdateRequest);

                        // Remove from pendingToDownload
                        DownloadProgress temp = null;
                        pendingToDownload.TryRemove(pr, out temp);

                        return null;
                    }

                    downloadedInCycle += bytesSize;
                    bytesDownloaded = bytesDownloaded + bytesSize;
                    strLocal.Write(downBuffer, 0, bytesSize);

                    // Report progress to UI
                    double msElapsedSinceRefresh = (DateTime.Now - refresh).TotalMilliseconds;
                    if (msElapsedSinceRefresh > 1000) // Determines how fast UI is updated
                    {
                        tempTransferRate = (int)((downloadedInCycle / 1024.0) / (DateTime.Now - refresh).TotalSeconds);
                        dp.completed = bytesDownloaded;
                        dp.status = "Downloading";
                        
                        // Compute download speed with smoothing factor
                        if (dp.averageTransferRate == 0)
                            dp.averageTransferRate = dp.transferRate;
                        
                        dp.transferRate = tempTransferRate;
                        dp.averageTransferRate = (0.01) * tempTransferRate + (0.99) * dp.averageTransferRate;
                        
                        int percentComplete = 0;
                        if(pr.fileSize != 0)
                            percentComplete = (int) (100 * bytesDownloaded / pr.fileSize);

                        worker.ReportProgress(percentComplete, dp);

                        refresh = DateTime.Now;
                        downloadedInCycle = 0;
                    }
                }

                dp.completed = bytesDownloaded;
                dp.transferRate = tempTransferRate;
                dp.status = "Checking file..";
                
                worker.ReportProgress(100, dp);
                strLocal.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("download: Error:" + e.ToString());
                dp.transferRate = 0;
                dp.status = "Failed";
                worker.ReportProgress(0, dp);
                return null;
            }
            finally
            {
                clientStream.Close();
            }

            Utils.writeLog("download: Completed download of " + pr.fileName + 
                " (" + dp.fileSize + ") Type:" + dp.type );

            String newHash = Indexer.GenerateHash(dp.downloadedFilePath);

            // Tell the server we have downloaded the file
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += Transfers.updateWorker_DoWork;
            updateWorker.RunWorkerCompleted += Transfers.updateWorker_RunWorkerCompleted;
            
            // Set update parameters
            UpdateRequest ur = new UpdateRequest();
            ur.newHash = newHash;
            ur.transferID = dp.transferID;
            ur.status = "done";
            ur.uploader = dp.mac;

            if (dp.type == "secondleg" || dp.type == "direct")
            {
                if (newHash == dp.hash)
                {
                    Utils.writeLog("download: Hash verified");
                    dp.isComplete = true;
                    dp.status = "Completed";
                    worker.ReportProgress(100, dp);
                }
                else
                {
                    Utils.writeLog("download: Hash verification failed");
                    dp.status = "Failed integrity check";
                    ur.status = "hash_mismatch"; // Modify the updaterequest
                    worker.ReportProgress(100, dp);

                    try
                    {
                        File.Delete(dp.downloadedFilePath);
                    }
                    catch (Exception e)
                    {
                        // Do nothing. We only want to try and delete the file.
                    }

                    newHash = null;
                }
            }
            else
            {
                // Rename the file to the new hash

                String newPath = dp.downloadedFilePath.Substring(0, dp.downloadedFilePath.LastIndexOf(@"\"));
                newPath = newPath + "\\" + newHash + ".bounced";

                try
                {
                    File.Move(dp.downloadedFilePath, newPath);
                }
                catch (Exception e)
                {
                    Utils.writeLog("download: Critical. Unable to rename file to the new hash.");
                }

                dp.status = "Completed";
                dp.isComplete = true;
                worker.ReportProgress(100, dp);
            }

            updateWorker.RunWorkerAsync(ur);

            return newHash;
        }

        public static void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateRequest ur = e.Argument as UpdateRequest;

            RestClient client = new RestClient("http://" + Configuration.server);
            RestRequest request = new RestRequest("update", Method.POST);

            request.AddParameter("transferID", ur.transferID);
            request.AddParameter("status", ur.status);
            request.AddParameter("newHash", ur.newHash);
            request.AddParameter("uploader", ur.uploader);

            Utils.writeLog("updateWorker_DoWork: Sending update request for transferID " + ur.transferID);

            RestResponse<StatusResponse> response = (RestResponse<StatusResponse>)client.Execute<StatusResponse>(request);

            e.Result = response.Data;
        }

        public static void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StatusResponse sr = e.Result as StatusResponse;
            if (sr != null)
                Utils.writeLog("updateWorker_RunWorkerCompleted: " + sr.ToString());
            else
                Utils.writeLog("updateWorker_RunWorkerCompleted: Status returned was null");
            //TODO: Check for failed update requests.
        }

    }
}
