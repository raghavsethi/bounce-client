using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace BouncedClient
{

    static class Configuration
    {
        private static string m_username = "";
        private static long m_numFilesShared = 0;
        private static List<string> m_sharedFolders = null;
        private static string m_server = "";
        private static int m_GBShared = 5;
        private static string m_downloadFolder = "";
        
        private static string m_indexHash = "";

        public static string username
        {
            get { return m_username; }
            set { m_username = value; }
        }

        public static string downloadFolder
        {
            get { return m_downloadFolder; }
            set { m_downloadFolder = value; }
        }

        public static int GBShared
        {
            get { return m_GBShared; }
            set { m_GBShared = value; }
        }

        public static long numFilesShared
        {
            get { return m_numFilesShared; }
            set { m_numFilesShared = value; }
        }

        public static List<string> sharedFolders
        {
            get { return m_sharedFolders; }
            set { m_sharedFolders = value; }
        }

        public static string server
        {
            get { return m_server; }
            set { m_server = value; }
        }

        public static string indexHash
        {
            get { return m_indexHash; }
            set { m_indexHash = value; }
        }

        public static bool loadConfiguration()
        {
            TextReader tr;
            try
            {
                 tr = new StreamReader(Utils.getAppDataPath("config.dat"));
            }
            catch (Exception e)
            {
                TextWriter tw = new StreamWriter(Utils.getAppDataPath("config.dat"), false);
                tw.Close();
                tr = new StreamReader(Utils.getAppDataPath("config.dat"));
            }

            m_sharedFolders = new List<string>();

            string currentLine;

            // First run behaviour
            if ((m_username = tr.ReadLine()) == null)
            {
                tr.Close();
                return false;
            }

            m_numFilesShared = Convert.ToInt64(tr.ReadLine());
            m_GBShared = Convert.ToInt32(tr.ReadLine());
            m_downloadFolder = tr.ReadLine();
            m_indexHash = tr.ReadLine();
            m_server = tr.ReadLine();

            //Reading list of shared folders.
            while ((currentLine = tr.ReadLine()) != null)
            {
                m_sharedFolders.Add(currentLine);
            }
            tr.Close();
            return true;
        }

        public static void saveConfiguration()
        {
            TextWriter tw = new StreamWriter(Utils.getAppDataPath("config.dat"), false);
            tw.WriteLine(m_username);
            tw.WriteLine(m_numFilesShared);
            tw.WriteLine(m_GBShared);
            tw.WriteLine(m_downloadFolder);
            tw.WriteLine(m_indexHash);
            tw.WriteLine(m_server);
            foreach (string sharedFolder in m_sharedFolders)
            {
                tw.WriteLine(sharedFolder);
            }
            tw.Close();
        }


    }
 }

