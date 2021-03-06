using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public enum outPutNewest
    {
        REMOTE,
        LOCAL
    }
    public static class FileHelper
    {
        public static string[] GetFilesFromDir(string dir)
        {
            string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            return filePaths;
        }

        public static DateTime GetModifiedDateTime(string file)
        {
            return System.IO.File.GetLastWriteTime(file);
        }

        /// <summary>
        /// Send list to server client
        /// </summary>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> listFilesWithDateTime(string dir)
        {
            var serverList = new List<KeyValuePair<string, string>>();
            int i = 0;

            foreach (var file in GetFilesFromDir(dir))
            {
                serverList.Insert(i, new KeyValuePair<string, string>(Path.GetFileName(file), GetModifiedDateTime(file).ToString(Config.cultureInfo)));
                i++;
            }

            return serverList;
        }

        /// <summary>
        /// Compares directory listing of remote host and local host.
        /// returns dictionary <filename, datetime modified> of files that are newer on host
        /// </summary>
        ///<returns>Dictionary <string,string> </string></returns>
        public static Dictionary<string, string> CompareDir(Dictionary<string, string> localFileList, Dictionary<string, string> remoteFileList, outPutNewest output = outPutNewest.REMOTE)
        {
            Dictionary<string, string> list1;
            Dictionary<string, string> list2;

            if (output == outPutNewest.REMOTE)
            {
                list1 = localFileList;
                list2 = remoteFileList;
            }
            else
            {
                list2 = localFileList;
                list1 = remoteFileList;
            }
                        
            Dictionary<string, string> tmpDict = new Dictionary<string, string>();

            if (list1.Count == 0 || list1 == null)
            {
                return list2;
            }

            foreach (KeyValuePair<string, string> entry in list2)
            {
                if (list1.ContainsKey(entry.Key))
                {
                    var dateTimeL = DateTime.Parse(list1[entry.Key], Config.cultureInfo);
                    var dateTimeR = DateTime.Parse(entry.Value, Config.cultureInfo);

                    if (dateTimeR > dateTimeL)
                    {
                        tmpDict.Add(entry.Key, entry.Value);
                    }
                }
                else
                {
                    tmpDict.Add(entry.Key, entry.Value);
                }
            }
            return tmpDict;
        }

        public static Dictionary<string, string> DictFilesWithDateTime(string dir)
        {
            var serverList = new Dictionary<string, string>();

            foreach (var file in GetFilesFromDir(dir))
            {
                serverList.Add(Path.GetFileName(file), GetModifiedDateTime(file).ToString(Config.cultureInfo));
            }

            return serverList;
        }
    }
}
