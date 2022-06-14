using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class Global
    {
        public static string rootDir { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
        start:
            Console.WriteLine("Mode; 1-Server, 2-GetNewerFiles, 3-GetFile, 4-SendNewerFiles, 5-Exit");
            string _serverIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();
            string _clientIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();  
            string message = Console.ReadLine();

            switch (message)
            {
                case "1":
                    Global.rootDir = Config.serverDir;
                    Connection server = new Connection(_serverIP, Config.serverPort);
                    server.ServerStart();
                    break;

                case "2":
                    Global.rootDir = Config.clientDir;
                    Connection client = new Connection(_serverIP, Config.serverPort);
                    string resp = client.sendCommand("List");

                    CommandHandler cmd = new CommandHandler();
                    Response response = cmd.getResponse(resp);

                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_serverIP), Config.serverPort);
                    response.runAction(endPoint);
                    break;

                case "3":
                    Global.rootDir = Config.clientDir;
                    Connection client2 = new Connection(_serverIP, Config.serverPort);
                    string resp2 = client2.sendCommand("get test.txt");

                    CommandHandler cmd2 = new CommandHandler();
                    Response response2 = cmd2.getResponse(resp2);

                    IPEndPoint endPoint2 = new IPEndPoint(IPAddress.Parse(_serverIP), Config.dataPort);
                    response2.runAction(endPoint2);
                    break;

                case "4":
                    Global.rootDir = Config.clientDir;
                    Connection client4 = new Connection(_serverIP, Config.serverPort);
                    string resp4 = client4.sendCommand("List");

                    CommandHandler cmd4 = new CommandHandler();
                    Response response4 = cmd4.getResponse(resp4);

                    IPEndPoint endPoint4 = new IPEndPoint(IPAddress.Parse(_serverIP), Config.serverPort);
                    response4.runAction(endPoint4);

                    string resp4b = client4.sendCommand("");
                    Response response4b = cmd4.getResponse(resp4b);
                    client4.sendCommand(response4b.getResponseString());
                    response4b.runAction(endPoint4);
                    break;

                case "5":
                    System.Environment.Exit(0);
                    break;

               default:
                    goto start;
                    break;

            }
            Console.WriteLine("--------------------------------------");
            goto start;

        }
    }
}
