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
            Console.WriteLine("Mode?:");
            string message = Console.ReadLine();

            switch (message)
            {
                case "server":
                    Server server = new Server();
                    server.start();

                    int serverstart = server.start();
                    if (serverstart == -1)
                    {
                        Console.WriteLine("Unable to start [Server]");
                    }
                    break;
                case "client":
                    Client client = new Client();
                    client.startAsync(Config.serverIp, Config.serverPort);
                    break;

                case "ftserver":
                    TcpListener dataListener = TcpListener.Create(Config.dataPort);
                    dataListener.Start();
                    handler.receiveFile(dataListener);
                    break;

                case "ftclient":
                    TcpClient dataClient = new TcpClient(Config.serverIp, Config.dataPort);
                    string file2Send = @"d:\test\d2.mp4";
                    handler.sendFile(dataClient, file2Send);
                    break; 

            }

        }
    }
}
