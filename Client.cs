using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace FileSync
{
    class Client
    {
        private string _serverIP;
        private int _serverPort;
        private int _listenerPort;

        public Client()
        {

        }

        [Obsolete]
        public async Task startAsync(string serverIP = "127.0.0.1", int serverPort = 3456, int listenerPort = 2345)
        {
            _serverIP = serverIP;
            _serverPort = serverPort;
            _listenerPort = listenerPort;

        connection:
            try
            {
                TcpClient client = new TcpClient(_serverIP, _serverPort);
                IPAddress ipAddress = IPAddress.Parse(_serverIP);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, _serverPort);
                if (client.Client.Connected == false) { client.Connect(ipEndPoint); };

                TcpListener dataListener = new TcpListener(_listenerPort); // 9,41 = 2345
                                                                           //dataListener.Start();           


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
                        request(stream, message);
                        handler.receiveFile(dataListener);
                    }

                    string response = request(stream, message);

                    //Console.WriteLine(response);

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
