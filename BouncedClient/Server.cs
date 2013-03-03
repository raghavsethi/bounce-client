using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncedClient
{
    class Server
    {
        public static TcpListener tcpListener;
        public static ASCIIEncoding encoder;
        public static int currentUploadsCount;

        public static System.Object uploadCountLock = new System.Object();

        public static void serverWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Utils.writeLog("serverWorker_DoWork: Started");

            // Handles uploads to peers.
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8002);
                tcpListener.Start();
            }
            catch (Exception ex)
            {
                Utils.writeLog("serverWorker_DoWork: " + ex);
                return;
            }

            Utils.writeLog("serverWorker_DoWork: Listening for peers..");

            while (true)
            {
                // Waits until a client has connected to the server.
                TcpClient client = tcpListener.AcceptTcpClient();
                //client.NoDelay = true;
                Utils.writeLog("serverWorker_DoWork: New peer connected");
                
                // Creates a thread to handle the client.
                BackgroundWorker uploadWorker = new BackgroundWorker();
                uploadWorker.DoWork += uploadWorker_DoWork;
                //uploadWorker.ProgressChanged += uploadWorker_ProgressChanged;
                uploadWorker.RunWorkerAsync(client);
            }
        }

        static void uploadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void uploadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TcpClient tcpClient = e.Argument as TcpClient;
            if (tcpClient == null)
                return;

            tcpClient.NoDelay = true;
            NetworkStream clientStream = tcpClient.GetStream();

            if (encoder == null)
                encoder = new ASCIIEncoding();

            byte[] message = new byte[4096];
            int bytesRead;
            bytesRead = 0;
            
            try
            {
                // Blocks until a client sends a message
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                Utils.writeLog("uploadWorker_DoWork: Socket error");
                return;
            }

            if (bytesRead == 0)
            {
                Utils.writeLog("uploadWorker_DoWork: Client disconnected");
                return;
            }

            // If code gets here, message was successfully received.
            String uploadParameters = encoder.GetString(message, 0, bytesRead);

            // Format: fileHash | transfer ID | transfer-type

            //TODO: Add part which gets key and actually encrypts the transfer
            char[] sep = { '|' };
            String[] temp = uploadParameters.Split(sep);
            String fileHash = temp[0];
            String transferId = temp[1];
            String transferType = temp[2]; //direct, firstleg, secondleg

            upload(fileHash, clientStream, transferType);
            clientStream.Close();
        }

        public static bool upload(string fileHash, NetworkStream fileUploadStream, String transferType)
        {
            Utils.writeLog("upload: Started");

            lock (Server.uploadCountLock)
            {
                Server.currentUploadsCount++;
            }

            bool successfulTransfer = false;
            byte[] byteSend = new byte[4096];

            string filePath = "";
            string fileName = "";

            // Get file path from hash.
            if (transferType == "direct" || transferType == "firstleg")
            {
                LocalFile lf = (LocalFile)Indexer.fileIndex[fileHash];
                
                // Got request for a file that we don't have.
                if (lf == null)
                {
                    Utils.writeLog("upload: Upload failed. Got request for a file not present in index. Hash:" + fileHash);
                    return false;
                }

                filePath = lf.location;
            }
            else
            {
                filePath = Utils.getAppDataPath(@"\Bounces\" + fileHash + ".bounced");
            }

            FileStream fileLocalStream;

            try
            {
                fileLocalStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Utils.writeLog("upload: " + e.ToString());
                return false;
            }

            BinaryReader binaryFileReader = new BinaryReader(fileLocalStream);
            FileInfo fileInfo = new FileInfo(filePath);

            Utils.writeLog("upload: Started reading file from disk : " + filePath);
            fileName = fileInfo.Name;

            long bytesUploaded = 0;

            try
            {
                int bytesSize = 0;

                // Send the file.
                while ((bytesSize = fileLocalStream.Read(byteSend, 0, byteSend.Length)) > 0)
                {
                    //Thread.Sleep(100);
                    bytesUploaded = bytesUploaded + bytesSize;
                    fileUploadStream.Write(byteSend, 0, bytesSize);
                }

                Utils.writeLog("upload : Sent file : " + fileName);
                successfulTransfer = true;
            }
            catch (Exception e)
            {
                Utils.writeLog("Upload module reported error : " + e.ToString());
                successfulTransfer = false;
            }
            finally
            {
                fileLocalStream.Close();
            }

            lock (Server.uploadCountLock)
            {
                Server.currentUploadsCount--;
            }

            return successfulTransfer;
        }
    }
}
