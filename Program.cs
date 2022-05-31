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
                    Server server = new Server();
                    server.start();

                    int serverstart = server.start();
                    if (serverstart == -1)
                    {
                        Console.WriteLine("Unable to start [Server]");
                    }
                    break;
                case "2":
                    Client client = new Client();
                    client.startAsync(Config.serverIp, Config.serverPort);
                    break;

                case "ftget":
                    TcpListener dataListener = TcpListener.Create(Config.dataPort);
                    dataListener.Start();
                    handler.receiveFile(dataListener);
                    break;

                case "ftsend":
                    TcpClient dataClient = new TcpClient(Config.serverIp, Config.dataPort);
                    handler.sendFile(dataClient, file2Send);
                    break;

                case "fts":
                    FileTransfer ft = new FileTransfer();
                    ft.sendFile(file2Send);
                    break;

                case "ftr":
                    FileTransfer ft2 = new FileTransfer();
                    ft2.getFile();
                    break;
            }

        }
    }
}
