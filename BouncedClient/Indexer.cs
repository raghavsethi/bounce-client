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

        public static void serializeHashTable()
        {
            Stream s = File.Open("files.dat", FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, fileIndex);
            s.Close();

            s = File.Open("modified.dat", FileMode.Create, FileAccess.ReadWrite);
            b = new BinaryFormatter();
            b.Serialize(s, modifiedIndex);
            s.Close();

            s = File.Open("hashes.dat", FileMode.Create, FileAccess.ReadWrite);
            b = new BinaryFormatter();
            b.Serialize(s, hashIndex);
            s.Close();   
        }

        public static void deserializeHashTable()
        {
            try
            {
                Stream s = File.Open("files.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter b = new BinaryFormatter();
                fileIndex = (Hashtable)b.Deserialize(s);
                s.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("deserializeHashTable: Error reading fileIndex : " + e);
                fileIndex = new Hashtable();
            }

            try
            {
                Stream s = File.Open("modified.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter b = new BinaryFormatter();
                modifiedIndex = (Hashtable)b.Deserialize(s);
                s.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("deserializeHashTable: Error reading modifiedIndex : " + e);
                modifiedIndex = new Hashtable();
            }

            try
            {
                Stream s = File.Open("hashes.dat", FileMode.Open, FileAccess.Read);
                BinaryFormatter b = new BinaryFormatter();
                hashIndex = (Hashtable)b.Deserialize(s);
                s.Close();
            }
            catch (Exception e)
            {
                Utils.writeLog("deserializeHashTable: Error reading hashIndex : " + e);
                hashIndex = new Hashtable();
            }

            Utils.writeLog("deserializeIndex: Loaded " + fileIndex.Keys.Count + " files from disk");
            Boolean consistent = false;
            if (fileIndex.Keys.Count == hashIndex.Keys.Count && hashIndex.Keys.Count==modifiedIndex.Keys.Count)
                consistent = true;

            Utils.writeLog("deserializeIndex: Consistency is " + consistent);

        }

        public static void buildIndex(CheckedListBox.ObjectCollection sharedFoldersList)
        {
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

            List<string> folders = new List<string>();
            foreach (string sharedFolderPath in sharedFoldersList)
                folders.Add(sharedFolderPath);

            while (folders.Count > 0)
            {
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
                            //If file has not been modified, copy old data and remove from old table
                            if ((DateTime)modifiedIndex[fi.FullName] == fi.LastWriteTime)
                            {

                                //Utils.writeLog("buildIndex: Old file not modified :" + fi.FullName);

                                string hash = (string)hashIndex[fi.FullName];
                                updatedFileIndex[hash] = fileIndex[hash];
                                fileIndex.Remove(hash);
                                continue;
                            }
                            Utils.writeLog("buildIndex: Old file modified :" + fi.FullName);
                            //If it has been modified, it will be in both fileIndex and updatedFileIndex
                            //All files in fileIndex will be removed.
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

            serializeHashTable();
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
    }
}
