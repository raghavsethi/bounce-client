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
        // These are what the server thinks the client has
        public static Hashtable fileIndex;      //File hash to LocalFile

        // This always remains up-to-date. If the values in this have not changed
        // then the file is guaranteed to be in either fileIndex or updatedIndex

        public static Hashtable modifiedIndex;  //File name to DateTime

        public static Hashtable hashIndex;      //File name to file hash

        // These are the latest indices which have not been synced
        public static Hashtable updatedIndex;   //File hash to LocalFile
        public static Hashtable removedFiles;   //File hash to LocalFile
        public static Hashtable addedFiles;     //File hash to LocalFile

        public static void serializeHashTables()
        {
            serializeFile(fileIndex, "_files.dat");
            serializeFile(modifiedIndex, "_modified.dat");
            serializeFile(hashIndex, "_hashes.dat");

            serializeFile(updatedIndex, "_updated.dat");
            serializeFile(addedFiles, "_added.dat");
            serializeFile(removedFiles, "_removed.dat");
        }

        public static void deserializeHashTables()
        {
            fileIndex = deserializeFile("_files.dat");
            modifiedIndex = deserializeFile("_modified.dat");
            hashIndex = deserializeFile("_hashes.dat");

            updatedIndex = deserializeFile("_updated.dat");
            addedFiles = deserializeFile("_added.dat");
            removedFiles = deserializeFile("_removed.dat");
        }

        public static void successfulSync()
        {
            fileIndex = (Hashtable)updatedIndex.Clone();
            removedFiles = new Hashtable();
            addedFiles = new Hashtable();
            serializeHashTables();
        }

        public static void buildIndex(CheckedListBox.ObjectCollection sharedFoldersList)
        {
            Utils.writeLog("buildIndex: Started indexing...");

            int numFoldersIndexed = 0;

            DateTime timeOfLastSave = DateTime.Now; //Keeps track of when to persist stuff in the middle of indexing

            List<string> folders = new List<string>();
            foreach (string sharedFolderPath in sharedFoldersList)
                folders.Add(sharedFolderPath);

            /* INDEXING
             * --------
             * 
             * The idea is to maintain add/remove lists accurately until a successful sync
             * occurs. At this point we will nullify add/remove list and update fileIndex.
             * 
             * Add/remove lists:
             * It turns out that determining removed files is super-fast, because there is
             * no hash computation involved. The most accurate time to get the list of
             * removed files is after the computationally-expensive add list is computed.
             * 
             * So what we do is:
             * 1. Compare against updatedIndex to determine added files - if new, add to
             *    updatedIndex and addedFiles. If old, do nothing.
             * 2. After all adds we have an updatedIndex with some removed files as well
             * 3. Every so often, write everything to disk so that we can pick up where we
             *    left off.
             * 4. So then we compute removed files by checking against updatedIndex. If
             *    removed, remove from updatedIndex and add to removedFiles
             * 5. Attempt to sync, if successful nullify add/remove and fileIndex=updated
             *    FileIndex
             * 
            */

            while (folders.Count > 0)
            {
                // Persist data from time to time so that we can eventually index very large sets
                if ((DateTime.Now - timeOfLastSave).TotalMinutes > 3)
                {
                    Utils.writeLog("buildIndex : Time exceeded 3 minutes, writing indices to disk..");
                    serializeHashTables();
                    timeOfLastSave = DateTime.Now;
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
                    Utils.writeLog("ERROR! buildIndex: UnauthorizedAccessException while indexing " + di.FullName);
                    continue;
                }

                // Add all the sub folders to the processing queue
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
                            // If file has not been modified, it's already in updatedIndex
                            if ((DateTime)modifiedIndex[fi.FullName] == fi.LastWriteTime)
                            {
                                //Utils.writeLog("buildIndex: Old file not modified : " + fi.FullName);
                                continue;
                            }
                            else
                            {
                                Utils.writeLog("buildIndex: Old file modified : " + fi.FullName);
                            }
                        }
                        else
                        {
                            Utils.writeLog("buildIndex: New file seen : " + fi.FullName);
                        }

                        // Get file details, including hash and keywords
                        LocalFile currentFile = getFileInfo(fi.FullName);

                        if (currentFile == null)
                        {
                            Utils.writeLog("buildIndex: Didn't process file :" + fi.FullName);
                            continue;
                        }

                        hashIndex[fi.FullName] = currentFile.hash;
                        modifiedIndex[fi.FullName] = fi.LastWriteTime;

                        addedFiles[currentFile.hash] = currentFile;
                        updatedIndex[currentFile.hash] = currentFile;
                    }

                    folders.Remove(folders[0]);
                    numFoldersIndexed++;
                }
            }

            // Addition is complete, now check for removed files..
            Utils.writeLog("buildIndex: File addition complete..");

            removedFiles = (Hashtable)fileIndex.Clone();
            
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
                    Utils.writeLog("ERROR! buildIndex: UnauthorizedAccessException while indexing " + di.FullName);
                    continue;
                }

                // Add all the sub folders to the processing queue
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
                        // If file hash was found in hashIndex, remove from removedFiles
                        if (hashIndex.ContainsKey(fi.FullName) && updatedIndex.ContainsKey(hashIndex[fi.FullName]))
                        {
                            removedFiles.Remove(hashIndex[fi.FullName]);
                        }
                    }
                    folders.Remove(folders[0]);
                }
            }

            // At this point removedFiles contains list of files in the index that don't
            // correspond to any files on disk..

            // Make sure removed files are gone from the appropriate places
            foreach(String key in removedFiles.Keys)
            {
                String path = ((LocalFile)removedFiles[key]).location;
                Utils.writeLog("buildIndex: Removed file : " + path);
                
                if (addedFiles.ContainsKey(key))
                    addedFiles.Remove(key);

                if (updatedIndex.ContainsKey(key))
                {
                    Utils.writeLog("buildIndex: Found out-of-date key in updatedIndex");
                    updatedIndex.Remove(key);
                }

                if (modifiedIndex.ContainsKey(path))
                {
                    modifiedIndex.Remove(path);
                }
            }

            Utils.writeLog("buildIndex: " + removedFiles.Keys.Count + " files removed");
            Utils.writeLog("buildIndex: " + addedFiles.Keys.Count + " files added");

            serializeHashTables();
            Utils.writeLog("buildIndex: Completed indexing of " + numFoldersIndexed + " folders.");
        }
         
        public static LocalFile getFileInfo(string filePath)
        {
            LocalFile currentFile = new LocalFile();

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

            if (currentFile.name.ToLower().Contains("thumbs.db") ||
                currentFile.name.ToLower().Contains("albumart") ||
                currentFile.name.ToLower().Contains("desktop.ini") || 
                currentFile.name.ToLower().Contains("folder.jpg"))
            {
                Utils.writeLog("getFileInfo: Ignored file " + filePath);
                return null;
            }

            if ((currentFile.hash = GenerateHash(filePath)) == null)
            {
                Utils.writeLog("getFileInfo: Unable to generate hash of file " + filePath);
                return null;
            }
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
                Utils.writeLog("GenerateHash: Output: " + output);
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
                Stream s = File.Open(Utils.getAppDataPath(file), FileMode.Open, FileAccess.Read);
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
            Stream s = File.Open(Utils.getAppDataPath(file), FileMode.Create, FileAccess.ReadWrite);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(s, h);
            s.Close();
        }
    }
}
