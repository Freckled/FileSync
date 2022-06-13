﻿using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace FileSync
{
    public class FileHandler
    {

        public static void GetFile(IPEndPoint endPoint, string fileName, long fileLength)
        {
            string filePath = Config.clientDir + fileName;

            try
            {
                // Create a Socket that will use Tcp protocol
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, (int)1);
                socket.Bind(endPoint);
                //_socket.Connect(remoteEndPoint);
                socket.Listen(10);

                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = socket.Accept();

                ///receive file

                try
                {
                    using (NetworkStream networkStream = new NetworkStream(_dataSocket))
                    using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate))
                    {

                        while (fileStream.Length < fileLength)
                        {
                            networkStream.CopyTo(fileStream);
                        }
                    }
                    _dataSocket.Close();
                    socket.Close();
                    Console.WriteLine("filetransfer complete " + fileName + " " + fileLength);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public static void SendFile(IPEndPoint endPoint, string fileName)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            string fileLoc = Config.serverDir + fileName;
        
            try
            {
                //Send file fileName to the remote device.
                Console.WriteLine("Sending file : {0} {1}", fileLoc, Environment.NewLine);
                
                socket.SendFile(fileLoc);
                Console.WriteLine("File Transfer started");

                // Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("File Transfer failed");
                Console.WriteLine(e.ToString());
            }            
        }

        public static void GetFiles(IPEndPoint remoteEndPoint, List<KeyValuePair<string, string>> list)
        {
            Connection conn = new Connection(remoteEndPoint);
            IPAddress remoteIP = remoteEndPoint.Address;

            CommandHandler cmd = new CommandHandler();
            IPEndPoint dataEndPoint = new IPEndPoint(remoteIP, Config.dataPort);


            for (int i = 0; i < list.Count; i++)
            {                
                string resp = conn.sendCommand("get " + list[i].Key);
                Response response = cmd.getResponse(resp);                
                response.runAction(dataEndPoint);
            }

        }

    }
}
