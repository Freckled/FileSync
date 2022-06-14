using System;
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

        public static void GetFile(IPEndPoint endPoint, string fileName, long fileLength, DateTime? modDate)
        {
            string filePath = Global.rootDir + fileName;
            DateTime fileModDate = modDate ?? DateTime.Now;

            try
            {
                // Create a Socket that will use Tcp protocol
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, (int)1);
                socket.Bind(endPoint);
                socket.Listen(10);

                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = socket.Accept();

                try
                {
                    //Receive file
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
                    File.SetLastWriteTime(filePath, fileModDate);
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
            string fileLoc = Global.rootDir + fileName;

            try
            {
                //Send file fileName to the remote device.
                Console.WriteLine("Sending file : {0} {1}", fileLoc, Environment.NewLine);
                socket.SendFile(fileLoc);
                Console.WriteLine("File Transfer started");

                // Release the socket.
                socket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("File Transfer failed");
                Console.WriteLine(e.ToString());
            }
        }
        public static void GetFiles(IPEndPoint remoteEndPoint, Dictionary<string, string> list)
        {
            Connection conn = new Connection(remoteEndPoint);
            IPAddress remoteIP = remoteEndPoint.Address;

            CommandHandler cmd = new CommandHandler();
            IPEndPoint dataEndPoint = new IPEndPoint(remoteIP, Config.dataPort);

            foreach (KeyValuePair<string, string> entry in list)
            {
                string resp = conn.sendCommand("get " + entry.Key);
                Response response = cmd.getResponse(resp);
                response.runAction(dataEndPoint);
            }

        }


        public static void SendFiles(IPEndPoint remoteEndPoint, Dictionary<string, string> list)
        {

            Connection conn = new Connection(remoteEndPoint);
            IPAddress remoteIP = remoteEndPoint.Address;

            CommandHandler cmd = new CommandHandler();
            IPEndPoint dataEndPoint = new IPEndPoint(remoteIP, Config.dataPort);

            foreach (KeyValuePair<string, string> entry in list)
            {
                string resp = conn.sendCommand("send " + entry.Key + entry.Value);
                //Response response = cmd.getResponse(resp);
                FileHandler.SendFile(dataEndPoint, entry.Key);
            }

        }

    }
}
