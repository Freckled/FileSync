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
            string _serverIP = "192.168.1.144";//"84.241.204.248";x;
            string _clientIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString();  //"192.168.1.144";
            string message = Console.ReadLine();

            switch (message)
            {
                case "1":
                    SyncSocket server = new SyncSocket(_serverIP, Config.serverPort);
                    server.ServerStart();
                    break;

                case "2":
                    //TODO klaarstaan voor de file receive.  aka; nieuwe socket openen en luisteren.
                    //TODO bestand daadwerkelijk ontvangen. 
                    
                    
                    SyncSocket client = new SyncSocket(_serverIP, Config.clientPort);
                    SyncSocket fileSocket2 = new SyncSocket(_clientIP, 11305);

                    string file2get = "D:\\FileWatcher\\test.txt";
                    string response = client.sendCommand("get " + file2get);
                    
                    Console.WriteLine(response);

                    Response response2 = new Response(response, ActionType.GETFILE);
                    response2.runAction(fileSocket2);

                    //var fileResponse2 = await fileSocket2.getFileAsync();
                    //Console.WriteLine(response);
                    break;

                case "3":
                    FileHelper fh = new FileHelper();
                    Console.WriteLine(fh.GetFilesFromDir("d:\\FileWatcher"));

                    break;

                case "4":

                    break;

                case "5":

                    break;




            }

        }
    }
}
