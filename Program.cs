using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FileSync
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Mode; 1-Server, 2-Client, 3-FileWait, 4-FileSend:");
            string message = Console.ReadLine();

            switch (message)
            {
                case "1":
                    SyncSocket server = new SyncSocket("192.168.1.144", Config.serverPort);
                    server.ServerStart();
                    break;


                case "2":
                    SyncSocket client = new SyncSocket("192.168.1.144", Config.clientPort);
                    SyncSocket fileSocket2 = new SyncSocket("192.168.1.144", 11305);

                    string response = client.sendCommand("get");
                    Console.WriteLine(response);

                    var fileResponse2 = await fileSocket2.getFileAsync();
                    Console.WriteLine(fileResponse2);
                    break;

                case "3":
                    SyncSocket fileSocket = new SyncSocket("192.168.1.144", 11305);
                    var fileResponse = await fileSocket.getFileAsync();
                    Console.WriteLine(fileResponse);
                    break;

                case "4":
                    SyncSocket fileSocketSend = new SyncSocket("192.168.1.144", 11305);
                    await fileSocketSend.sendFileAsync("D:\\FileWatcher\\test.txt");

                    break;


            }

        }
    }
}
