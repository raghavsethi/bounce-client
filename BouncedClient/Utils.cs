using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncedClient
{
    class Utils
    {
        private static System.Object logLock = new System.Object();

        public static string GetMACAddress()
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    //break;
                }
            }
            return macAddresses;
        }

        public static void writeLog(string message)
        {
            lock (logLock)
            {
                TextWriter tw;
                try
                {
                    tw = new StreamWriter("log.txt", true);
                }
                catch (IOException e)
                {
                    MessageBox.Show("Unhandleable IOException occurred.\nCould not write event details to log file.\nYou should be able to continue normally.");
                    return;
                }
                //tw.WriteLine("{0} : {1}", DateTime.Now.ToString("dd/MM/yyyy h:MM:ss tt"), message);
                tw.WriteLine("{0} : {1}", DateTime.Now.ToLongTimeString(), message);
                tw.Close();
            }
        }

        public static void clearLog()
        {
            TextWriter tw;
            tw = new StreamWriter("log.txt", false);
            tw.Write("");
            tw.Close();
        }

        public static string getHumanSize(long bytes)
        {
            string human = bytes.ToString() + "B";
            if (bytes > 1024)
            {
                human = (bytes / 1024.0).ToString("F2") + "KB";
            }
            if (bytes > (1024 * 1024))
            {
                human = (bytes / (1024 * 1024.0)).ToString("F2") + "MB";
            }
            if (bytes > (1024 * 1024 * 1024))
            {
                human = (bytes / (1024 * 1024 * 1024.0)).ToString("F2") + "GB";
            }
            return human;
        }
    }
}
