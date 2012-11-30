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
        public static List<DownloadRequest> outstandingDownloadRequests;

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
                        
                        int percentComplete = (int) (100 * bytesDownloaded / pr.fileSize);

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
                dp.status = "Download failed.";
                worker.ReportProgress(0, dp);
                return null;
            }
            finally
            {
                clientStream.Close();
            }

            Utils.writeLog("download: Successfully downloaded " + pr.fileName + 
                "(" + dp.fileSize + ") Type:" + dp.type );

            String hash = Indexer.GenerateHash(dp.downloadedFilePath);
            if (dp.type == "secondleg" || dp.type == "direct")
            {
                if (hash == dp.hash)
                {
                    Utils.writeLog("download: Hash verified");
                    dp.status = "Completed";
                    worker.ReportProgress(100, dp);
                }
                else
                {
                    Utils.writeLog("download: Hash verification failed");
                    dp.status = "Failed integrity check";
                    worker.ReportProgress(100, dp);

                    try
                    {
                        File.Delete(dp.downloadedFilePath);
                    }
                    catch (Exception e)
                    {
                        // Do nothing. We only want to try and delete the file.
                    }

                    return null;
                }
            }
            else
            {
                dp.status = "Completed";
                worker.ReportProgress(100, dp);
            }

            return hash;
        }
    }
}
