﻿using BounceClient.Properties;
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

        public static string getMACAddress()
        {
            string macAddresses = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                /*
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
                */

                // Made the change to get a fixed MAC address for each computer 
                // regardless of current connection. Doesn't seem to work perfectly yet.

                Utils.writeLog("MAC: " + nic.GetPhysicalAddress().ToString() + " Type: " + nic.NetworkInterfaceType);

                if (!nic.NetworkInterfaceType.Equals(NetworkInterfaceType.Loopback))
                {
                    macAddresses = nic.GetPhysicalAddress().ToString();
                    //Utils.writeLog("GetMACAddress: MAC address is " + macAddresses);
                    break;
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
                    tw = new StreamWriter(getAppDataPath("log.txt"), true);
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
            tw = new StreamWriter(getAppDataPath("log.txt"), false);
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

        public static string getHumanSpeed(int kbps)
        {
            string human = kbps.ToString() + " KB/s";
            if (kbps > 1024)
            {
                human = (kbps / 1024.0).ToString("F2") + " MB/s";
            }
            if (kbps > (1024 * 1024))
            {
                human = (kbps / (1024 * 1024.0)).ToString("F2") + " GB/s";
            }
            return human;
        }

        public static string getHumanTime(double seconds)
        {
            if (seconds < 60)
            {
                return seconds.ToString("F0") + "s";
            }
            if (seconds < 3600)
            {
                return (seconds / 60).ToString("F0") + "m " + (seconds % 60).ToString("F0") + "s";
            }
            if (seconds < 24 * 3600)
            {
                return (seconds / 3600).ToString("F0") + "hrs " + ((seconds % 3600)/60).ToString("F0") + "m";
            }

            return "Unknown";
        }

        public static string getAppDataPath(String filename)
        {
            String appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appDataPath = Path.Combine(appDataPath, "Bounce");
            appDataPath = Path.GetFullPath(appDataPath + "..\\" + filename); ;
            return appDataPath;
        }

        public static string getVersion()
        {
            String versionID = "Debug";

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                versionID = ad.CurrentVersion.ToString();
            }

            return versionID;
        }

        public static void writeHasherToDisk()
        {
            FileStream md5write = File.OpenWrite(Utils.getAppDataPath("md5sums.exe"));
            md5write.Write(Resources.md5sums, 0, Resources.md5sums.Length);
            md5write.Close();
        }

    }
}
