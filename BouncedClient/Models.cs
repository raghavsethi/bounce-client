using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncedClient
{
    class Models
    {
    }

    public class StatusResponse
    {
        public String status { get; set; }
        public String text { get; set; }

        
        public override String ToString()
        {
            return status + " : " + text;
        }
    }

    public class PendingResponse
    {
        public long transferID { get; set; }
        public String uploader { get; set; }
        public String downloader { get; set; }
        public String fileHash { get; set; }
        public String fileName { get; set; }
        public String symKey { get; set; }
        public String type { get; set; }

        public String uploaderIP { get; set; }

        public class EqualityComparer : IEqualityComparer<PendingResponse>
        {

            public bool Equals(PendingResponse x, PendingResponse y)
            {
                return x.fileHash == y.fileHash && x.uploader == y.uploader;
            }

            public int GetHashCode(PendingResponse x)
            {
                return x.fileHash.GetHashCode() * x.uploader.GetHashCode();
            }

        }

    }

    [Serializable()]
    public class LocalFile
    {
        public String hash;
        public String name;
        public String type;
        public String location;
        public String parentFolder;
        public String size;

        public String[] keywords;

        public void computeKeywords()
        {
            List<String> kw = new List<string>();
            
            char[] sep = {' ', '.', '-', '_'};

            kw.AddRange(name.Split(sep));
            kw.AddRange(parentFolder.Split(sep));
            kw.Add(type);

            keywords = new String[kw.Count];

            for(int i=0; i<kw.Count; i++) {
                keywords[i]=kw[i].ToLower();
            }


        }
        //TODO: Add dictionary for metadata
    }

    public class SearchResult
    {
        public String name { get; set; }
        public String hash { get; set; }
        public String mac { get; set; }
        public String nick { get; set; }
        public long size { get; set; }
        public String type { get; set; }
        public bool online { get; set; }
        

        public override String ToString()
        {
            return name + " : " + type;
        }
    }

    public class DownloadRequest
    {
        public string mac;
        public string hash;
        public string fileName;
        public string type;
        public long fileSize;
    }

    public class DownloadProgress
    {
        public string mac;
        public string hash;
        public string fileName;
        public string type;
        public long fileSize;
        public long completed;
        public string downloaderIp;
        public string symKey;
        public string status;
    }

}
