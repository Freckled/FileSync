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
            Console.WriteLine(  "███████╗██╗██╗     ███████╗███████╗██╗   ██╗███╗   ██╗ ██████╗\r\n" +
                                "██╔════╝██║██║     ██╔════╝██╔════╝╚██╗ ██╔╝████╗  ██║██╔════╝\r\n" +
                                "█████╗  ██║██║     █████╗  ███████╗ ╚████╔╝ ██╔██╗ ██║██║     \r\n" +
                                "██╔══╝  ██║██║     ██╔══╝  ╚════██║  ╚██╔╝  ██║╚██╗██║██║     \r\n" +
                                "██║     ██║███████╗███████╗███████║   ██║   ██║ ╚████║╚██████╗\r\n" +
                                "╚═╝     ╚═╝╚══════╝╚══════╝╚══════╝   ╚═╝   ╚═╝  ╚═══╝ ╚═════╝\r\n");            
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
                
                Console.WriteLine("Choose your Mode;\n1-Server\n2-client\n3-client [input server IP]\n4-client [input server IP & PORT]\n5-Exit");

                IPAddress IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                Console.Write("Selection: ");
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
                        client.start();

                        break;

                    case "3":
                        Console.Write("Input IP Adress of server: ");
                        string input = Console.ReadLine();
                        Client clientIP = new Client(input);
                        clientIP.start();
                        break;

                    case "4":
                        Console.Write("Input IP Adress of server: ");
                        string inputIP2 = Console.ReadLine();
                        Console.Write("Input PORT of server: ");
                        string port = Console.ReadLine();
                        Client clientIP2 = new Client(inputIP2, port);                        
                        clientIP2.start();
                        break;



                    case "5":
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
        }
    }
}
