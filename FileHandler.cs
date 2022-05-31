using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    internal class FileHandler
    {
        private string _file;
        //private TcpClient _client;
        private TcpListener _listener;

        public void sendFile(TcpClient client, string file)
        {
            _file = file;
            //_client = client;

                try
                {
                    //TcpClient tcpClient = new TcpClient("127.0.0.1", 2345);
                    Console.WriteLine("Connected. Sending file.");

                    StreamWriter sWriter = new StreamWriter(client.GetStream());
                    byte[] bytes = File.ReadAllBytes(_file);

                    sWriter.WriteLine(bytes.Length.ToString());
                    sWriter.Flush();

                    sWriter.WriteLine(_file);
                    sWriter.Flush();

                    Console.WriteLine("Sending file");
                    client.Client.SendFile(_file);

                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

                Console.Read();

        }

        public void receiveFile(TcpListener listener)
        {
            // Listen on port 1234    
            //TcpListener tcpListener = new TcpListener(IPAddress.Any, 2345);
            
            
            listener.Start();
            Console.WriteLine("Server started");

            //Infinite loop to connect to new clients    
            while (true)
            {
                // Accept a TcpClient    
                TcpClient tcpClient = listener.AcceptTcpClient();

                Console.WriteLine("Connected to client");

                StreamReader reader = new StreamReader(tcpClient.GetStream());

                // The first message from the client is the file size    
                string cmdFileSize = reader.ReadLine();

                // The first message from the client is the filename    
                string cmdFileName = reader.ReadLine();

                int length = Convert.ToInt32(cmdFileSize);
                byte[] buffer = new byte[length];
                int received = 0;
                int read = 0;
                int size = 1024;
                int remaining = 0;

                // Read bytes from the client using the length sent from the client    
                while (received < length)
                {
                    remaining = length - received;
                    if (remaining < size)
                    {
                        size = remaining;
                    }

                    read = tcpClient.GetStream().Read(buffer, received, size);
                    received += read;
                }

                // Save the file using the filename sent by the client    
                using (FileStream fStream = new FileStream(Path.GetFileName(cmdFileName), FileMode.Create))
                {
                    fStream.Write(buffer, 0, buffer.Length);
                    fStream.Flush();
                    fStream.Close();
                }

                Console.WriteLine("File received and saved in " + Environment.CurrentDirectory);
            }


        }


    }
}
