using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class Global
    {
        public static string rootDir { get; set; }
        public static string remoteIP { get; set; }
    }
    class Program
    {
        /// <summary>
        /// Main entrypoint of the app.
        /// </summary>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
        start:
            Console.WriteLine("Mode; 1-Server, 2-client, 3-client [input server IP], 4-Exit");
            string _serverIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            string _clientIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();  
            string message = Console.ReadLine();

            Global.remoteIP = _serverIP;

            switch (message)
            {
                case "1":
                    Global.rootDir = Config.serverDir;
                    Connection server = new Connection(_serverIP, Config.serverPort);
                    server.ServerStart();
                    break;

                case "2":
                    //get newest files from server.                    
                    Global.rootDir = Config.clientDir;
                    SyncFiles(_serverIP);
                    Console.WriteLine("Files synchronized");
                    MonitorChanges();
                    break;

                case "3":
                    Console.WriteLine("input server IP");
                    Global.remoteIP = Console.ReadLine();
                    break;

                case "4":
                    System.Environment.Exit(0);
                    break;

               default:
                    goto start;
                    break;

            }
            Console.WriteLine("--------------------------------------");
            goto start;
        }


        /// <summary>
        /// Synchronize the files with those on the remote host. Will request any newer files it doesnt have and send
        /// files it has that are newer than those on the remote host. 
        /// </summary>
        /// <param name="_serverIP">the string containing the Server Ipv4 IPAddress</param>
        /// <param name="fileName">name of the file to receive</param>
        /// <returns></returns>
        public static void SyncFiles(string _serverIP)
        {

            Connection client = new Connection(_serverIP, Config.serverPort);
            //ask for a fileList
            string resp = client.sendCommand("List");

            CommandHandler cmd = new CommandHandler();
            Response response = cmd.getResponse(resp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_serverIP), Config.serverPort);
            response.runAction(endPoint);

            //compose list from response
            string[] commandCode = resp.Split(' ');
            string arguments = resp.Length > 1 ? resp.Substring(commandCode[0].Length) : null;
            arguments = arguments.Trim();

            var LocalfileList = FileHelper.DictFilesWithDateTime(Global.rootDir);
            var argSplitFiles = arguments.Split(' ');

            Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

            foreach (string file in argSplitFiles)
            {
                var fileSplit = file.Split("|");
                remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
            }

            var dirListSend = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.LOCAL);
            FileHandler.SendFiles(endPoint, dirListSend);
        }

        /// <summary>
        /// Calls the Filewatcher to monitor the folder for any changes to files (local)
        /// </summary>
        /// <returns></returns>
        public static void MonitorChanges()
        {
            FileWatcher.Watch();
        }

    }
}
