using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BouncedClient
{
    class Indexer
    {
        public static Hashtable fileIndex;
        public static Hashtable modifiedIndex;
        public static Hashtable hashIndex;
        public static Hashtable removedFiles;
        public static Hashtable addedFiles;
        //TODO: Add a hashtable containing file paths and last modified times to improve perf

        public static void serializeHashTables()
        {
            serializeFile(fileIndex, "files.dat");
            serializeFile(modifiedIndex, "modified.dat");
            serializeFile(hashIndex, "hashes.dat");
            serializeFile(addedFiles, "added.dat");
            serializeFile(removedFiles, "removed.dat");
        }

        public static void deserializeHashTables()
        {
            fileIndex = deserializeFile("files.dat");
            modifiedIndex = deserializeFile("modified.dat");
            hashIndex = deserializeFile("hashes.dat");
            
            Utils.writeLog("deserializeIndex: Loaded " + fileIndex.Keys.Count + " files from disk");
            Boolean consistent = false;
            if (fileIndex.Keys.Count == hashIndex.Keys.Count && hashIndex.Keys.Count==modifiedIndex.Keys.Count)
                consistent = true;

            Utils.writeLog("deserializeIndex: Consistency is " + consistent);
        }

        public static void buildIndex(CheckedListBox.ObjectCollection sharedFoldersList)
        {
            #region initvars

            //TODO: Optimize Hashtable constructor for efficiency.
            if (fileIndex == null)
                fileIndex = new Hashtable();

            if (modifiedIndex == null)
                modifiedIndex = new Hashtable();

            if (hashIndex == null)
                hashIndex = new Hashtable();

            if (addedFiles == null)
                addedFiles = new Hashtable();

            Hashtable updatedFileIndex = new Hashtable();

            Utils.writeLog("buildIndex: Started indexing...");

            int numFoldersIndexed = 0;

            DateTime timeOfLastSave = DateTime.Now; //Keeps track of when to persist stuff in the middle of indexing

            #endregion

            List<string> folders = new List<string>();
            foreach (string sharedFolderPath in sharedFoldersList)
                folders.Add(sharedFolderPath);

            /* INDEXING
             * --------
             * 
             * The idea is to maintain add/remove lists accurately until a successful sync
             * occurs. At this point we will nullify add/remove list and update fileIndex.
             * 
             * How this will be done:
             * It turns out that determining removed files is super-fast, because there is
             * no hash computation involved. The most accurate time to get the list of
             * removed files is after the computationally-expensive add list is computed.
             * 
            */

            while (folders.Count > 0)
            {
                // We want to persist data from time to time so that we can eventually index very large sets
                if ((DateTime.Now - timeOfLastSave).TotalMinutes > 5)
                {
                    Utils.writeLog("buildIndex : Time exceeded 5 minutes, writing indices to disk..");
                    serializeHashTables();
                }

                DirectoryInfo di = new DirectoryInfo(folders[0]);
                FileInfo[] fileInfoArr = null;
                DirectoryInfo[] directoryInfoArr = null;
                try
                {
                    fileInfoArr = di.GetFiles();
                    directoryInfoArr = di.GetDirectories();
                }
                catch (UnauthorizedAccessException u)
                {
                    Utils.writeLog("ERROR! buildIndex: UnauthorizedAccessException while indexing: " + di.FullName);
                }

                if (directoryInfoArr != null)
                {
                    foreach (DirectoryInfo currentDirectory in directoryInfoArr)
                    {
                        folders.Add(currentDirectory.FullName);
                    }
                }

                if (fileInfoArr != null)
                {
                    foreach (FileInfo fi in fileInfoArr)
                    {
                        if (modifiedIndex.ContainsKey(fi.FullName))
                        {
                            // If file has not been modified, copy old data and remove from old table
                            if ((DateTime)modifiedIndex[fi.FullName] == fi.LastWriteTime)
                            {

                                //Utils.writeLog("buildIndex: Old file not modified :" + fi.FullName);

                                string hash = (string)hashIndex[fi.FullName];
                                updatedFileIndex[hash] = fileIndex[hash];
                                fileIndex.Remove(hash);
                                continue;
                            }
                            Utils.writeLog("buildIndex: Old file modified :" + fi.FullName);
                            // If it has been modified, it will be in both fileIndex and updatedFileIndex
                            // All files in fileIndex will be removed.
                        }

                        LocalFile currentFile = getFileInfo(fi.FullName);

                        if (currentFile == null)
                        {
                            Utils.writeLog("buildIndex: Could not get hash for file :" + fi.FullName);
                            continue;
                        }

                        hashIndex[fi.FullName] = currentFile.hash;
                        modifiedIndex[fi.FullName] = fi.LastWriteTime;
                        addedFiles[currentFile.hash] = currentFile;
                        updatedFileIndex[currentFile.hash] = currentFile;

                    }

                    folders.Remove(folders[0]);
                    numFoldersIndexed++;
                }

            }

            removedFiles = fileIndex;
            fileIndex = updatedFileIndex;

            Utils.writeLog("buildIndex: " + removedFiles.Keys.Count + " files removed");
            Utils.writeLog("buildIndex: " + addedFiles.Keys.Count + " files added");

            serializeHashTables();
            Utils.writeLog("buildIndex: Completed indexing");
        }
         
        public static LocalFile getFileInfo(string filePath)
        {
            LocalFile currentFile = new LocalFile();

            if ((currentFile.hash = GenerateHash(filePath)) == null)
                return null;

            try
            {
                int indexOfDot = filePath.LastIndexOf('.');
                currentFile.type = filePath.Substring(indexOfDot);
            }
            catch (Exception e)
            {
                currentFile.type = "";
            }

            try
            {
                int indexOfSlash = filePath.LastIndexOf('\\');
                currentFile.name = filePath.Substring(indexOfSlash+1);
            }
            catch (Exception e)
            {
                currentFile.name = filePath;
            }

            currentFile.size = new FileInfo(filePath).Length.ToString();
            
            try
            {
                int indexOfLastSlash = filePath.LastIndexOf('\\');
                currentFile.parentFolder = filePath.Substring(0,indexOfLastSlash);
                int indexOfSecondSlash = currentFile.parentFolder.LastIndexOf('\\');
                currentFile.parentFolder = currentFile.parentFolder.Substring(indexOfSecondSlash+1);
            }
            catch (Exception e)
            {
                currentFile.name = filePath;
            }

            currentFile.location = filePath;

            /*
            List<string> propertyHeaders = new List<string>();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            Shell32.Shell shell = new Shell32.Shell();
            Shell32.Folder locationFolder;

            locationFolder = shell.NameSpace(filePath);

            String s = "";

            for (int i = 0; i < short.MaxValue; i++)
            {
                string header = locationFolder.GetDetailsOf(null, i);
                if (String.IsNullOrEmpty(header))
                    break;
                propertyHeaders.Add(header);
            }

            foreach (Shell32.FolderItem item in locationFolder.Items())
            {
                for (int i = 0; i < propertyHeaders.Count; i++)
                {
                    properties.Add(propertyHeaders[i], locationFolder.GetDetailsOf(item, i));
                    s = s + propertyHeaders[i] + " : " + locationFolder.GetDetailsOf(item, i) + "\n";
                }
            }
            MessageBox.Show(s);
            */

            currentFile.computeKeywords();

            return currentFile;
        }

        public static string GenerateHash(string file)
        {
            var p = new Process();

            p.StartInfo.FileName = Application.StartupPath + "\\md5sums.exe";
            p.StartInfo.Arguments = " -e -u \"" + file + "\"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            try
            {
                return output.Split(' ')[0].Substring(1).ToUpper();
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }

        public static string getAddedJson()
        {
            return JsonConvert.SerializeObject(addedFiles, Formatting.Indented);
        }

        public static string getRemovedJson()
        {
            return JsonConvert.SerializeObject(removedFiles, Formatting.Indented);
        }

        public static Hashtable deserializeFile(String file)
        {
            Hashtable temp = null;
            try
            {
                Stream s = File.Open(file, FileMode.Open, FileAccess.Read);
                BinaryFormatter b = new BinaryFormatter();
                temp = (Hashtable)b.Deserialize(s);
                s.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("deserializeHashTable: Error reading "+file+" : " + e);
                temp = new Hashtable();
            }
            return temp;
        }

        public static void serializeFile(Hashtable h, String file)
        {
            Stream s = File.Open(file, FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, h);
            s.Close();
        }
    }
}
