using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class FileHelper
    {
        public string[] GetFilesFromDir(string dir)
        {
            string[] filePaths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            return filePaths;
        }

        public string GetFileFromDirByName(string name, string dir)
        {
            dir = (dir != "") ? dir : Config.clientDirectory;

            string[] filePaths = Directory.GetFiles(dir, name);
            return filePaths.GetValue(0).ToString();
        }

        public DateTime GetModifiedDateTime(string file)
        {
            return System.IO.File.GetLastWriteTime(file);
        }

        public Boolean IsClientNew(string clientFile, string serverFile)
        {
            if (!File.Exists(clientFile))
            {
                return false;
            }

            var clientDateTimeModified = this.GetModifiedDateTime(clientFile);
            var serverDateTimeModified = this.GetModifiedDateTime(serverFile);
            return (clientDateTimeModified > serverDateTimeModified) ? true : false;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public string[] arrayOfFilesWithDate(string name, string dir)
        {
            string seperator = ";";
            List<String> filesList = new List<String>();

            String[] serverFilesArray;

            foreach (var file in this.GetFilesFromDir(Config.clientDirectory))
            {
                string fileNameWithDateTime = file + seperator + this.GetModifiedDateTime(file).ToString();
                filesList.Add(fileNameWithDateTime);
            }

            return serverFilesArray = filesList.ToArray();
        }

        /// <summary>
        /// Send list to server client
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> listFilesWithDateTimeServer()
        {
            var serverList = new List<KeyValuePair<string, string>>();
            int i = 0;

            foreach (var file in this.GetFilesFromDir(Config.serverDirectory))
            {
                serverList.Insert(i, new KeyValuePair<string, string>(file, this.GetModifiedDateTime(file).ToString()));
                i++;
            }

            return serverList;
        }

        /// <summary>
        /// Send list to server client
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> listFilesWithDateTimeClient()
        {
            var serverList = new List<KeyValuePair<string, string>>();
            int i = 0;

            foreach (var file in this.GetFilesFromDir(Config.clientDirectory))
            {
                serverList.Insert(i, new KeyValuePair<string, string>(file, this.GetModifiedDateTime(file).ToString()));
                i++;
            }

            return serverList;
        }
        public List<KeyValuePair<string, string>> getListWithFilesToChange()
        {
            var finalList = new List<KeyValuePair<string, string>>();
            var listClient = this.listFilesWithDateTimeClient();
            var listServer = this.listFilesWithDateTimeServer();
            int i = 0;
            foreach (KeyValuePair<string, string> file in listClient)
            {
                string fileName = file.Key;
                string dateTime = file.Value;

                string fileserver = listServer.Find(kvp => kvp.Key == fileName).Value;
                if (fileserver == null)
                {
                    finalList.Insert(i, new KeyValuePair<string, string>(fileName, dateTime));
                    i++;
                }
            }

            return finalList;
        }
    }
}
