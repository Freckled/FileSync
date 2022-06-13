using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public static class FileHelper
    {
        public static string[] GetFilesFromDir(string dir)
        {
            string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            return filePaths;
        }

        //public static string[] GetFilesFromDirNames(string dir)
        //{
        //    string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
        //    foreach (string fileName in filePaths)
        //    {
        //        List<string> FileList = new List<string>();
        //        FileList.Add(Path.GetFileName(fileName));
        //    }
        //    return filePaths;
        //}

        public static string GetFileFromDirByName(string name, string dir)
        {
            dir = (dir != "") ? dir : Config.clientDirectory;

            string[] filePaths = Directory.GetFiles(dir, name);
            return filePaths.GetValue(0).ToString();
        }

        public static DateTime GetModifiedDateTime(string file)
        {
            return System.IO.File.GetLastWriteTime(file);
        }

        public static Boolean IsClientNew(string clientFile, string serverFile)
        {
            if (!File.Exists(clientFile))
            {
                return false;
            }

            var clientDateTimeModified = GetModifiedDateTime(clientFile);
            var serverDateTimeModified = GetModifiedDateTime(serverFile);
            return (clientDateTimeModified > serverDateTimeModified) ? true : false;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string[] arrayOfFilesWithDate(string name, string dir)
        {
            string seperator = ";";
            List<String> filesList = new List<String>();

            String[] serverFilesArray;

            foreach (var file in GetFilesFromDir(Config.clientDirectory))
            {
                string fileNameWithDateTime = file + seperator + GetModifiedDateTime(file).ToString();
                filesList.Add(fileNameWithDateTime);
            }

            return serverFilesArray = filesList.ToArray();
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
        /// compares two dirlists and returns files on the remote that arent in local or are newer on the remote
        /// </summary>
        /// <returns>List<string></returns>
        public static List<string> CompareFileList(List<KeyValuePair<string, string>> localFileList, List<KeyValuePair<string, string>> remoteFileList)
        {
            List<string> tmpList = new List<string>();

            //set Datime to GB

            for (int i = 0; i < remoteFileList.Count; i++)
            {
                if (localFileList.All(x => x.Key.Contains(remoteFileList[i].Key)))
                {
                    var dateTimeL = DateTime.Parse(localFileList.First(c => c.Key == remoteFileList[i].Key).Value, Config.cultureInfo);
                    var dateTimeR = DateTime.Parse(remoteFileList[i].Value, Config.cultureInfo);

                    if (dateTimeR > dateTimeL)
                    {
                        tmpList.Add(remoteFileList[i].Key);
                    }
                }
                else
                {
                    tmpList.Add(remoteFileList[i].Key);
                }
            }
            return tmpList;
        }


        public static List<KeyValuePair<string, string>> CompareFileList2(List<KeyValuePair<string, string>> localFileList, List<KeyValuePair<string, string>> remoteFileList)
        {
            List<KeyValuePair<string, string>> tmpList = new List<KeyValuePair<string, string>>();
            if (localFileList.Count == 0)
            {
                tmpList = remoteFileList;
                return tmpList;
            }
            //set Datime to GB
            var cultureInfo = new CultureInfo("en-GB");
            for (int i = 0; i < remoteFileList.Count; i++)
            {
                if (localFileList.All(x => x.Key.Contains(remoteFileList[i].Key)))
                {
                    var dateTimeL = DateTime.Parse(localFileList.First(c => c.Key == remoteFileList[i].Key).Value, cultureInfo);
                    var dateTimeR = DateTime.Parse(remoteFileList[i].Value, cultureInfo);

                    if (dateTimeR > dateTimeL)
                    {
                        tmpList.Add(new KeyValuePair<string, string>(remoteFileList[i].Key, remoteFileList[i].Value));

                    }
                }
                else
                {
                    tmpList.Add(new KeyValuePair<string, string>(remoteFileList[i].Key, remoteFileList[i].Value));
                }
            }
            return tmpList;
        }




        /// <summary>
        /// Send list to server client
        /// </summary>
        /// <returns></returns>
        //public static List<KeyValuePair<string, string>> listFilesWithDateTimeClient()
        //{
        //    var serverList = new List<KeyValuePair<string, string>>();
        //    int i = 0;

        //    foreach (var file in GetFilesFromDir(Config.clientDir))
        //    {
        //        serverList.Insert(i, new KeyValuePair<string, string>(file, GetModifiedDateTime(file).ToString()));
        //        i++;
        //    }

        //    return serverList;
        //}
        //public static List<KeyValuePair<string, string>> getListWithFilesToChange()
        //{
        //    var finalList = new List<KeyValuePair<string, string>>();
        //    var listClient = listFilesWithDateTimeClient();
        //    var listServer = listFilesWithDateTimeServer();
        //    int i = 0;
        //    foreach (KeyValuePair<string, string> file in listClient)
        //    {
        //        string fileName = file.Key;
        //        string dateTime = file.Value;

        //        string fileserver = listServer.Find(kvp => kvp.Key == fileName).Value;
        //        if (fileserver == null)
        //        {
        //            finalList.Insert(i, new KeyValuePair<string, string>(fileName, dateTime));
        //            i++;
        //        }
        //    }

        //    return finalList;
        //}
    }
}
