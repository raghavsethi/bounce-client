using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BouncedClient
{
    class Transfers
    {
        public static ConcurrentDictionary<PendingResponse, DownloadProgress> pendingToDownload;
        public static List<DownloadProgress> currentDownloads;
        public static List<DownloadRequest> outstandingDownloadRequests;

        public static string download(BackgroundWorker worker, PendingResponse pr, DownloadProgress dr)
        {
            TcpClient tcpClient = new TcpClient();

            TcpClient tcpclnt = new TcpClient();
            tcpclnt.NoDelay = true;
            NetworkStream clientStream;
            
            try
            {
                tcpclnt.Connect(pr.uploaderIP, 8002);

                clientStream = tcpclnt.GetStream();
                Utils.writeLog("download: Sending download instruction to peer " + pr.uploaderIP + "...");

                //Format: fileHash | transfer ID | transfer-type

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

            //Begin file transfer
            FileStream strLocal = null;

            Transfers.currentDownloads.Add(dr);


            worker.ReportProgress(0, dr);

            long bytesDownloaded = 0;
            int bytesSize;

            DateTime startTime = DateTime.Now;
            DateTime currentTime;
            TimeSpan duration = new TimeSpan();
            DateTime refresh = DateTime.Now;
            float tempTransferRate = 0;

            byte[] downBuffer = new byte[4096];

            strLocal = new FileStream(Configuration.downloadFolder + "\\" + pr.fileName, 
                FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            
            try
            {
                //Performing file transfer.
                while ((bytesSize = clientStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    //In case user cancels download.
                    if (pendingToDownload[pr]==null || worker.CancellationPending)
                    {
                        Utils.writeLog("Download canceled by user");
                        dr.status = "Canceled";
                        worker.ReportProgress((int)(100 * bytesDownloaded / pr.fileSize), dr);
                        clientStream.Close();
                        strLocal.Close();
                        return null;
                    }

                    bytesDownloaded = bytesDownloaded + bytesSize;
                    strLocal.Write(downBuffer, 0, bytesSize);

                    DateTime temp = DateTime.Now;
                    double msElapsedSinceRefresh = (temp - refresh).TotalMilliseconds;
                    if (msElapsedSinceRefresh > 1000)
                    {
                        refresh = DateTime.Now;
                        dr.transferRate =  (double)(1.0 * bytesDownloaded / (1024 * duration.Seconds));
                        dr.completed = bytesDownloaded;
                        dr.status = "Downloading";
                        int percentComplete = (int) (100 * bytesDownloaded / pr.fileSize);
                        worker.ReportProgress(percentComplete, dr);
                    }

                    currentTime = DateTime.Now;
                    duration = currentTime - startTime;

                }
            
                dr.completed = bytesDownloaded;
                dr.transferRate = tempTransferRate;
                dr.status = "Checking file..";
                worker.ReportProgress(100, dr);
                strLocal.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("download: Error:" + e.ToString());
                dr.transferRate = 0;
                dr.status = "Download failed.";
                worker.ReportProgress(0, dr);
                return null;
            }
            finally
            {
                clientStream.Close();
            }

            Utils.writeLog("download: Successfully downloaded " + pr.fileName + 
                "(" + dr.fileSize + ") Type:" + dr.type );

            if (dr.type == "secondleg" || dr.type == "direct")
            {
                String hash = Indexer.GenerateHash(Configuration.downloadFolder + "\\" + pr.fileName);
                if (hash == dr.hash)
                {
                    Utils.writeLog("download: Hash verified");
                    dr.status = "Completed";
                    worker.ReportProgress(100, dr);
                }
                else
                {
                    Utils.writeLog("download: Hash verification failed");
                    dr.status = "Failed integrity check";
                    worker.ReportProgress(100, dr);
                    //TODO: Delete the file
                    return null;
                }
            }
            else
            {
                dr.status = "Completed";
                worker.ReportProgress(100, dr);
            }

            return Configuration.downloadFolder + "\\" + pr.fileName;
        }
    }
}
