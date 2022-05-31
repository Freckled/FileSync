using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace FileSync
{
    class Server
    {
        public int start(int port = 2503)
        {
            try
            {
                StartListener(port).Wait();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        private static async Task StartListener(int port)
        {
            var tcpListener = TcpListener.Create(port);
            tcpListener.Start();
            
                for (; ; )
            {
                Console.WriteLine("[Server] waiting for client(s)...");
                using (var tcpClient = await tcpListener.AcceptTcpClientAsync())
                {
                    try
                    {
                        Console.WriteLine("[Server] Client has connected");
                        using (var stream = tcpClient.GetStream())
                        using (var reader = new StreamReader(stream))
                        using (var writer = new StreamWriter(stream) { AutoFlush = true })
                        {
                            CommandHandler cmdHandler = new CommandHandler(stream);
                            byte[] buffer = new byte[4096];
                            Console.WriteLine("[Server] Reading from client");
                            

                            var request = await reader.ReadLineAsync();
                            //Console.WriteLine(request);

                            //insert stuff to do; aka commands etc. Aparte klasse voor afhandelen commandos maken?
                            string response = await Task.Run(()=> cmdHandler.getResponseAsync(request));
                          
                            //string.Format(string.Format("[Server] Client wrote '{0}'", response));
                            await writer.WriteLineAsync(response);
                            //for (int i = 0; i < 5; i++)
                            //{
                            //    await writer.WriteLineAsync("I am the server! HAHAHA!");
                            //    Console.WriteLine("[Server] Response has been written");
                            //    await Task.Delay(TimeSpan.FromSeconds(1));
                            //}
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[Server] client connection lost");
                    }
                }
            }
        }
    }
}
