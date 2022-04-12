using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FileSync
{
    class Client
    {
        private string _serverIP;

        public Client()
        {

        }

        [Obsolete]
        public async Task startAsync(string serverIP = "127.0.0.1", int serverPort = 3456, int listenerPort = 2345)
        {


        connection:
            try
            {
                TcpClient client = new TcpClient(serverIP, serverPort);

                TcpListener dataListener = new TcpListener(listenerPort); // 9,41 = 2345
                dataListener.Start();

                //for (; ; )
                //{
                //    Console.WriteLine("[Server] waiting for client(s)...");
                //    using (var dataClient = await dataListener.AcceptTcpClientAsync())
                //    {
                //        try
                //        {
                //            Console.WriteLine("[Server] Client has connected");
                //            using (var dataStream = dataClient.GetStream())
                //            using (var reader = new StreamReader(dataStream))
                //            using (var writer = new StreamWriter(dataStream) { AutoFlush = true })
                //            {

                //                byte[] buffer = new byte[4096];

                //                var request = await reader.ReadLineAsync();
                //                //Console.WriteLine(request);

                //                //insert stuff to do; aka commands etc. Aparte klasse voor afhandelen commandos maken?
                //                //string response = await Task.Run(() => cmdHandler.getResponseAsync(request));

                //                //string.Format(string.Format("[Server] Client wrote '{0}'", response));
                //                //await writer.WriteLineAsync(response);
                //                //for (int i = 0; i < 5; i++)
                //                //{
                //                //    await writer.WriteLineAsync("I am the server! HAHAHA!");
                //                //    Console.WriteLine("[Server] Response has been written");
                //                //    await Task.Delay(TimeSpan.FromSeconds(1));
                //                //}
                //            }
                //        }
                //        catch (Exception)
                //        {
                //            Console.WriteLine("[Server] client connection lost");
                //        }
                //    }
                //}

                NetworkStream stream = client.GetStream();


                while (true)
                {

                    Console.WriteLine("Message:");
                    string message = Console.ReadLine();

                    if (message == "exit")
                    {
                        stream.Close();
                        client.Close();
                    }

                    if (message == "get")
                    {
                        FileHandler handler = new FileHandler();
                        handler.receiveFile(dataListener);
                    }

                    string response = request(stream, message);
                    Console.WriteLine(response);

                    //if (response == "200 Data Connection Established") { request(stream, "get"); }
                    //Console.ReadKey();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                goto connection;
            }
        }

        public string request(NetworkStream stream, string message)
        {
            int byteCount = Encoding.ASCII.GetByteCount(message + 1);
            byte[] sendData = Encoding.ASCII.GetBytes(message);

            ////////
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(message);
            sw.Flush();
            ////////
            //stream.Write(sendData, 0, sendData.Length);
            Console.WriteLine("sending data to server...");


            StreamReader sr = new StreamReader(stream);
            string response = sr.ReadLine();

            return response;

        }

    }
}
