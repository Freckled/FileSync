using System;
using System.Net.Sockets;

namespace FileSync
{
    class Program
    {
        enum Mode
        {
            server,
            client,
            ftserver,
            ftclient
        }

        static void Main(string[] args)
        {

            FileHandler handler = new FileHandler();
            string file2Send = @"d:\FileWather\test.txt";
            Console.WriteLine("Mode; 1-Server, 2-Client, 3-FileWait, 4-FileSend:");
            string message = Console.ReadLine();


            switch (message)
            {
                case "1":

                    ServerSocket server = new ServerSocket();
                    server.start(Config.clientPort);
                    break;

                case "2":
                    ClientSocket client = new ClientSocket("192.168.1.144", Config.serverPort, Config.clientPort);
                    client.connect();

                    //TODO klaarstaan voor de file receive.  aka; nieuwe socket openen en luisteren.
                    //TODO bestand daadwerkelijk ontvangen. 
                    string response = client.sendCommand("get");

                    Console.WriteLine(response);
                    break;

                //case "3":

                //    SyncSocket fileSocket = new SyncSocket("192.168.1.144", 11305);
                //    fileSocket.connectToRemote();
                //    var fileResponse = fileSocket.getFile();
                //    Console.WriteLine(fileResponse);
                //    break;

                //case "4":

                //    SyncSocket fileSocketSend = new SyncSocket("192.168.1.144", 11305);
                //    fileSocketSend.connectToRemote();
                //    fileSocketSend.sendFile();
                    
                //    break;


            }

        }
    }
}
