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

                    ServerSocket server = new ServerSocket();
                    server.start(Config.clientPort);

                    break;
                case "2":
                    ClientSocket client = new ClientSocket("192.168.1.144", Config.serverPort, Config.clientPort);
                    client.connect();

                    FileTransfer ft = new FileTransfer();
                    ft.getFile("192.168.1.144", Config.dataPort);

                    string response = client.sendCommand("get");
                    Console.WriteLine(response);
                    break;
            }

        }
    }
}
