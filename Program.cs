using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class Global
    {
        public static string rootDir { get; set; }
        public static IPAddress remoteIP { get; set; }
        public static IPAddress localIP { get; set; }
        public static bool client { get; set; } 
    }
    class Program
    {
        /// <summary>
        /// Main entrypoint of the app.
        /// </summary>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            StartupActions();
            Console.WriteLine("Welcome to FileSync");

            
            //If commandline is used choose commandline           
            if (args.Length > 0) { 
                switch (args[0])
                {
                    case "server":
                        Server server = new Server();
                        server.start();
                        break;
                    case "client":
                        string input = args[1];
                        Client clientIP = new Client(input);
                        clientIP.start();
                        break;

                    default:
                        break;
                }
            }


        start:
            try {
                
                Console.WriteLine("Mode; 1-Server, 2-client, 3-client [input server IP], 4-Exit");
                //Console.WriteLine(Config.rootDir);
                //get local IPv4
                //string IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
                IPAddress IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                string message = Console.ReadLine();

                Global.remoteIP = IP;
                Global.localIP = IP;

                switch (message)
                {
                    case "1":
                        Server server = new Server();
                        server.start();
                        break;

                    case "2":
                        Client client = new Client();
                        Config.setClientTestDir(); //TODO remove for production
                        client.start();
                    
                        //TODO integrate into client (&server?)
                        //FileWatcher.Watch(); 
                        ////MonitorChanges();
                        break;

                    case "3":
                        string input = Console.ReadLine();
                        Client clientIP = new Client(input);
                        Config.setClientTestDir(); //TODO remove for production
                        clientIP.start();
                        break;

                    case "4":
                        Environment.Exit(0);
                        break;


                    case "test":                                                      

                        break;

                   default:
                        Console.Write("invalid input, try again");
                        goto start;
                        break;

            }
            Console.WriteLine("--------------------------------------");
            
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                goto start;
            }
            
        }


        //-----------------------------------KEEP FOR NOW, TODO REWRITE with new command handler and send command etc----------------------------------------
        /// <summary>
        /// Synchronize the files with those on the remote host. Will request any newer files it doesnt have and send
        /// files it has that are newer than those on the remote host. 
        /// </summary>
        /// <param name="_serverIP">the string containing the Server Ipv4 IPAddress</param>
        /// <param name="fileName">name of the file to receive</param>
        /// <returns></returns>
        //public static void SyncFiles(IPAddress _serverIP)
        //{

        //    Connection client = new Connection(_serverIP, Config.serverPort);
        //    //ask for a fileList
        //    string resp = client.sendCommand("List");

        //    CommandHandler cmd = new CommandHandler();
        //    Response response = cmd.getResponse(resp);

        //    IPEndPoint endPoint = new IPEndPoint(_serverIP, Config.serverPort);
        //    response.runAction(endPoint);

        //    //compose list from response
        //    string[] commandCode = resp.Split(' ');
        //    string arguments = resp.Length > 1 ? resp.Substring(commandCode[0].Length) : null;
        //    arguments = arguments.Trim();

        //    var LocalfileList = FileHelper.DictFilesWithDateTime(Global.rootDir);
        //    var argSplitFiles = arguments.Split(' ');

        //    Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

        //    foreach (string file in argSplitFiles)
        //    {
        //        var fileSplit = file.Split("|");
        //        remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
        //    }

        //    var dirListSend = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.LOCAL);
        //    FileHandler.SendFiles(endPoint, dirListSend);
        //}
        //-----------------------------------KEEP FOR NOW, TODO REWRITE with new command handler and send command etc----------------------------------------

        /// <summary>
        /// Calls the Filewatcher to monitor the folder for any changes to files(local)
        /// </summary>
        /// <returns></returns>
        public static void MonitorChanges()
        {
            FileWatcher.Watch();
        }

        private static void StartupActions()
        {
            //startup actions
            // If File directory does not exist, create it
            try { 
                if (!Directory.Exists(Config.rootDir))
                {
                    Directory.CreateDirectory(Config.rootDir);
                }
            }
            catch(Exception e) {
                Console.WriteLine(e.ToString());
            }

            //TODO test, remove later
            if (!Directory.Exists(Config.testDir))
            {
                Directory.CreateDirectory(Config.testDir);
            }
        }


    }
}
