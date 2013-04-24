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
    class Downloads
    {
        public static ConcurrentDictionary<PendingResponse, DownloadProgress> pendingToDownload;
        public static List<DownloadProgress> currentDownloads;
        public static int currentDownloadsCount;

        public static System.Object downloadCountLock = new System.Object();

        public static string download(BackgroundWorker worker, PendingResponse pr, DownloadProgress dp, long startByte)
        {
            TcpClient tcpClient = new TcpClient();
            NetworkStream clientStream = null;
            
            try
            {
                tcpClient.Connect(pr.uploaderIP, 8002);

                clientStream = tcpClient.GetStream();
                Utils.writeLog("download: Sending download instruction to peer " + pr.uploaderIP + "...");

                //Format: fileHash | transfer ID | transfer-type
                Utils.writeLog("tid is "+ pr.transferID);
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] buffer = encoder.GetBytes(pr.fileHash + "|" + pr.transferID + "|" + pr.type + "|" + startByte);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                Utils.writeLog("download: Download started.");
            }
            catch (Exception e)
            {
                Utils.writeLog("download: Could not establish connection. Error : " + e);
                dp.isFailed = true;
                return postDownload(dp, pr, worker);
            }

            // Begin file transfer
            FileStream strLocal = null;

            Downloads.currentDownloads.Add(dp);

            // Add the download to the gridview
            worker.ReportProgress(0, dp);

            #region initvars

            long bytesDownloaded = startByte;   //Total bytes downloaded for the file
            int bytesSize;              //Number of bytes read by the stream reader
            int tempTransferRate = 0;   //Instantaneous (for 1 refresh cycle) download rate
            long downloadedInCycle = 0; //Total bytes downloaded in the last refresh cycle
            int percentComplete = 0;

            DateTime startTime = DateTime.Now;  //To track total download time
            DateTime refresh = DateTime.Now;    //To track time since last refresh

            byte[] downBuffer = new byte[4096];

            bool appendToExistingFile = false;

            // Find a free filename
            
            if (File.Exists(dp.downloadedFilePath))
            {

                long existingFileLength = new FileInfo(dp.downloadedFilePath).Length;

                // First file found should be appended to
                if (startByte > 0 && startByte == existingFileLength)
                    appendToExistingFile = true;
                else
                {
                    String candidatePath = "";
                    for (int i = 2; i < 100; i++) // 100 here is arbitrary, just want to make sure it doesnt loop forever
                    {
                        candidatePath = dp.downloadedFilePath.Substring(0, dp.downloadedFilePath.LastIndexOf('.'));
                        candidatePath += " (" + i + ")";
                        candidatePath += dp.downloadedFilePath.Substring(dp.downloadedFilePath.LastIndexOf('.'));

                        if (!File.Exists(candidatePath))
                        {
                            Utils.writeLog("download: Found free file path " + candidatePath);
                            dp.downloadedFilePath = candidatePath;
                            dp.fileName = dp.downloadedFilePath.Substring(dp.downloadedFilePath.LastIndexOf(@"\") + 1);
                            break;
                        }
                        else
                        {
                            existingFileLength = new FileInfo(candidatePath).Length;
                            if (startByte > 0 && startByte == existingFileLength)
                            {
                                dp.downloadedFilePath = candidatePath;
                                appendToExistingFile = true;
                                break;
                            }
                        }
                    }
                }
            
            }

            try
            {
                if (appendToExistingFile)
                {
                    strLocal = new FileStream(dp.downloadedFilePath,
                    FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    strLocal = new FileStream(dp.downloadedFilePath,
                    FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
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
                clientStream.ReadTimeout = 6000; // Need to timeout now to prevent stalled appearance in UI

                while ((bytesSize = clientStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    // In case user cancels download
                    if (pendingToDownload[pr] == null || worker.CancellationPending)
                    {
                        throw new DownloadCanceledException("Download canceled");
                    }

                    downloadedInCycle += bytesSize;
                    bytesDownloaded = bytesDownloaded + bytesSize;
                    dp.bytesDownloaded = bytesDownloaded;
                    strLocal.Write(downBuffer, 0, bytesSize);

                    if (pr.fileSize != 0)
                        percentComplete = (int)(100 * bytesDownloaded / pr.fileSize);

                    // Report progress to UI at intervals
                    double msElapsedSinceRefresh = (DateTime.Now - refresh).TotalMilliseconds;
                    if (msElapsedSinceRefresh > 1000)
                    {
                        tempTransferRate = (int)((downloadedInCycle / 1024.0) / (DateTime.Now - refresh).TotalSeconds);

                        dp.status = "Downloading";

                        // Initialize download speed
                        if (dp.averageTransferRate == 0)
                            dp.averageTransferRate = tempTransferRate;

                        // Compute download speed with smoothing factor
                        dp.transferRate = tempTransferRate;
                        dp.averageTransferRate = (0.01) * tempTransferRate + (0.99) * dp.averageTransferRate;

                        worker.ReportProgress(percentComplete, dp);

                        refresh = DateTime.Now;
                        downloadedInCycle = 0;
                    }
                }

                dp.bytesDownloaded = bytesDownloaded;
                dp.transferRate = tempTransferRate;
                worker.ReportProgress(100, dp);

                Utils.writeLog("download: Completed download of " + pr.fileName +
                " (" + dp.fileSize + ") Type:" + dp.type);
                dp.isComplete = true;
                dp.status = "Checking file..";
            }
            catch (DownloadCanceledException dce) // Occurs when user cancels
            {
                Utils.writeLog("download: Canceled by user");
                dp.status = "Canceled";
                dp.isCanceled = true;
            }
            catch (Exception e) // Should only occur due to poor network/peer going offline
            {
                Utils.writeLog("download: Error:" + e.ToString());
                dp.status = "Failed";
                dp.isFailed = true;
            }
            finally
            {
                worker.ReportProgress(percentComplete, dp);
                strLocal.Close();
                clientStream.Close();
            }

            return postDownload(dp, pr, worker);
        }

        public static string postDownload(DownloadProgress dp, PendingResponse pr, BackgroundWorker worker)
        {
            int percentComplete = (int)(100 * dp.bytesDownloaded / pr.fileSize);

            if (dp.isFailed)
            {
                dp.attempts = dp.attempts + 1;
                if (dp.attempts > 10)
                {
                    dp.status = "Failed";
                    dp.isFailedUnrecoverably = true;
                    worker.ReportProgress(percentComplete, dp);
                    return null;
                }

                dp.status = "Retrying.. (" + dp.attempts + ")";
                dp.isFailed = false;
                worker.ReportProgress(percentComplete, dp);

                System.Threading.Thread.Sleep(3000);

                return "FAILED";
            }

            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += Downloads.updateWorker_DoWork;
            updateWorker.RunWorkerCompleted += Downloads.updateWorker_RunWorkerCompleted;

            // Set update parameters
            UpdateRequest updateRequest = new UpdateRequest();
            updateRequest.transferID = dp.transferID;
            updateRequest.uploader = dp.mac;

            if (dp.isCanceled)
            {
                try
                { File.Delete(dp.downloadedFilePath); }
                catch (Exception e2)
                { } // Do nothing. We only want to try and delete the file.

                // Remove from pendingToDownload
                DownloadProgress temp = null;
                pendingToDownload.TryRemove(pr, out temp);

                updateRequest.newHash = dp.hash;
                updateRequest.status = "canceled";

                updateWorker.RunWorkerAsync(updateRequest);
                return null;
            }

            // Download neither canceled nor failed

            String newHash = Indexer.GenerateHash(dp.downloadedFilePath);

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
                    dp.isHashMismatch = true;
                    dp.isFailed = true;
                    dp.isFailedUnrecoverably = true;
                    worker.ReportProgress(100, dp);

                    try
                    { File.Delete(dp.downloadedFilePath); }
                    catch (Exception e2)
                    { } // Do nothing. We only want to try and delete the file.
                }

                updateRequest.newHash = newHash;
                updateRequest.status = "done";

                updateWorker.RunWorkerAsync(updateRequest);

                return newHash;
            }

            if (dp.type == "firstleg")
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

            updateRequest.newHash = newHash;
            updateRequest.status = "done";
                        
            updateWorker.RunWorkerAsync(updateRequest);

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
