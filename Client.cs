using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace FileSync
{
    class Client
    {
        public void start()
        {
        connection:
            try
            {
                TcpClient client = new TcpClient("127.0.0.1", 2503);
                string messageToSend = "My name is Neo";
                int byteCount = Encoding.ASCII.GetByteCount(messageToSend + 1);
                byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);

                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
                Console.WriteLine("sending data to server...");

                StreamReader sr = new StreamReader(stream);
                string response = sr.ReadLine();
                Console.WriteLine(response);

                stream.Close();
                client.Close();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                goto connection;
            }
        }
    }
}
