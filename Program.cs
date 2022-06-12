using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileSync
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Mode; 1-Server, 2-Client, 3-DirTest, 4-?:");
            string _serverIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();// "192.168.1.144";//"84.241.204.248";x;
            string _clientIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();  //"192.168.1.144";
            string message = Console.ReadLine();

            switch (message)
            {
                case "1":

                    Connection server = new Connection(_serverIP, Config.serverPort);
                    server.ServerStart();
                    break;

                case "2":

                    Connection client = new Connection(_serverIP, Config.serverPort);
                    //string resp = client.sendCommand("get test.txt");
                    string resp = client.sendCommand("List");

                    CommandHandler cmd = new CommandHandler();
                    Response response = cmd.getResponse(resp);

                    //IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_serverIP), Config.dataPort);
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(_serverIP), Config.serverPort);
                    response.runAction(endPoint);
                    break;

                case "3":
                    Connection client2 = new Connection(_serverIP, Config.serverPort);
                    string resp2 = client2.sendCommand("get test.txt");
                    //string resp2 = client2.sendCommand("List");

                    CommandHandler cmd2 = new CommandHandler();
                    Response response2 = cmd2.getResponse(resp2);

                    IPEndPoint endPoint2 = new IPEndPoint(IPAddress.Parse(_serverIP), Config.dataPort);
                    //IPEndPoint endPoint2 = new IPEndPoint(IPAddress.Parse(_serverIP), Config.serverPort);
                    response2.runAction(endPoint2);

                    break;

                case "4":

                    break;

                case "5":

                    break;




            }

        }
    }
}
