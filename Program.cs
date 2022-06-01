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
            Console.WriteLine("Mode; 1-Server, 2-Client:");
            string message = Console.ReadLine();
            

            switch (message)
            {
                case "1":
                    //Server server = new Server();
                    //server.start();

                    //int serverstart = server.start();
                    //if (serverstart == -1)
                    //{
                    //    Console.WriteLine("Unable to start [Server]");
                    //}
                    ServerSocket server = new ServerSocket();
                    server.start();

                    break;
                case "2":
                    //Client client = new Client(Config.serverIp, Config.serverPort);
                    //client.startAsync();
                    ClientSocket client = new ClientSocket("192.168.1.144", Config.serverPort, Config.clientPort);
                    client.connect();
                    string response = client.sendCommand("get");
                    Console.WriteLine(response);
                    break;
            }

        }
    }
}
