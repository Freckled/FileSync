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

        /// <summary>
        /// Listens on local endpoint for connection to receive pre-determined file.
        /// </summary>
        /// <param name="endPoint">the local IPEndPoint</param>
        /// <param name="fileLength">length of the file to recveive in bytes</param>
        /// <param name="fileName">name of the file to receive</param>
        /// <param name="modDate">date last modified of file on remote host</param>
        /// <returns></returns>
        public static void GetFile(IPEndPoint endPoint, string fileName, long fileLength, DateTime? modDate)
        {
            string filePath = Global.rootDir + fileName;
            DateTime fileModDate = modDate ?? DateTime.Now;

            try
            {
                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = FSSocket.Listen(Config.dataPort);

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
                    //socket.Close();
                    File.SetLastWriteTime(filePath, fileModDate);
                    Console.WriteLine("filetransfer complete : " + fileName + " " + fileLength);
                }
                catch (Exception ex)
                {
                    //If filetransfer aborted before complete, delete incomplete file. 
                    long fileSizeLocal = new FileInfo(filePath).Length;
                    if (fileSizeLocal != fileLength)
                    {
                        File.Delete(filePath);
                        Console.WriteLine("File transfer failed");
                    }
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File transfer failed, connection could not be established");
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Sends a file to the remote IPEndPoint.
        /// </summary>
        /// <param name="endPoint">the local IPEndPoint</param>
        /// <param name="fileName">name of the file to receive</param>
        /// <returns></returns>
        public static void SendFile(IPEndPoint endPoint, string fileName)
        {
            Socket socket = FSSocket.Connect(Config.dataPort);
            string fileLoc = Global.rootDir + fileName;

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
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

                Console.WriteLine("File Transfer failed");
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Receives a predetermined amount of files from the remote Host.
        /// </summary>
        /// <param name="remoteEndPoint">the remote IPEndPoint</param>
        /// <param name="list">A Dictionary <filename, filesize> of files to receive</param>
        /// <returns></returns>
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

        /// <summary>
        /// Receives a predetermined amount of files from the remote Host.
        /// </summary>
        /// <param name="remoteEndPoint">the remote IPEndPoint</param>
        /// <param name="list">A Dictionary <filename, filesize> of files to send</param>
        /// <returns></returns>
        public static void SendFiles(IPEndPoint remoteEndPoint, Dictionary<string, string> list)
        {

            Connection conn = new Connection(remoteEndPoint);
            IPAddress remoteIP = remoteEndPoint.Address;

            CommandHandler cmd = new CommandHandler();
            IPEndPoint dataEndPoint = new IPEndPoint(remoteIP, Config.dataPort);

            foreach (KeyValuePair<string, string> entry in list)
            {
                string filePath = Global.rootDir + entry.Key;
                long fileSize = new FileInfo(filePath).Length;
                string resp = conn.sendCommand("send " + " " + entry.Key + " " + fileSize + " " + entry.Value);
                //Response response = cmd.getResponse(resp);
                FileHandler.SendFile(dataEndPoint, entry.Key);
            }

        }

    }
}
