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
    public class Test
    {


        public void Connection() {
            Socket sock = FSSocket.Connect(2503);
            string message = "list";
            string response = SendMessage(sock, message);
            ResponseHandler(sock, response);
        }

        public void SynchFiles()
        {
            Socket sock = FSSocket.Connect(2503);
            string command = "GET";
            string fileName = "1234.txt";
            string message = command + " " + fileName;
            string response = SendMessage(sock, message);                               
            ResponseHandler(sock, response);            

        }


        private void ResponseHandler(Socket socket, string response)
        {

            string message = null;
            string fileName;

            string[] commandCode = response.Split(' ');

            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = response.Length > 1 ? response.Substring(commandCode[0].Length) : null;
            arguments = arguments.Trim();


            if (response == null)
            {
                switch (response)
                {
                    case "GET":
                        fileName = arguments;
                        string filePath = Global.rootDir + fileName;
                        long fileSize = new FileInfo(filePath).Length;
                        string fileModDate = FileHelper.GetModifiedDateTime(filePath).ToString(Config.cultureInfo);
                        message = "SEND " + fileName + " " + fileSize + " " + fileModDate;
                        SendMessage(socket, message);

                        if (Global.client)
                        {
                            Socket sock = FSSocket.Connect(11305);
                            SendFile(sock, fileName);
                        }
                        else
                        {
                            Socket sock = FSSocket.Listen(11305);
                            SendFile(sock, fileName);
                        }
                        break;

                    case "SEND":
                        var argSplit = arguments.Split(' ');
                        fileName = argSplit[0];
                        fileSize = (long)Convert.ToDouble(argSplit[1]);
                        var modDate = DateTime.Parse(argSplit[2] + " " + argSplit[3], Config.cultureInfo);
                        Console.WriteLine("reveiving file : " +  fileName);
                        ReceiveFile(socket, fileName, fileSize, modDate);
                        break;

                    case "LIST":

                        break;
                }
            }
            
            
            
            Console.WriteLine(response);
        }

        public string SendMessage(Socket socket, string message)
        {
            string response = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            byte[] msg = Encoding.ASCII.GetBytes(message);

            // Send the data through the socket.
            socket.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = socket.Receive(bytes);
            response += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Text received : {0}", response);
            return response;
        }
        
        public bool SendFile(Socket sock, string filePath)
        {
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open); ;
            long totalBytes = file.Length, bytesSoFar = 0;
            sock.SendTimeout = 1000000; //timeout in milliseconds
            try
            {
                //sock.Connect(endpoint);
                byte[] filechunk = new byte[4096];
                int numBytes;
                while ((numBytes = file.Read(filechunk, 0, 4096)) > 0)
                {
                    if (sock.Send(filechunk, numBytes, SocketFlags.None) != numBytes)
                    {
                        throw new Exception("Error in sending the file");
                    }
                    bytesSoFar += numBytes;
                    Byte progress = (byte)(bytesSoFar * 100 / totalBytes);
                    if (progress > lastStatus && progress != 100)
                    {
                        Console.WriteLine("File sending progress:{0}", lastStatus);
                        lastStatus = progress;
                    }
                }
                sock.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                sock.Close();
                file.Close();
            }
            return true;
        }



        public bool SendFile(IPAddress deviceAddr, string filePath)
        {
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open); ;
            long totalBytes = file.Length, bytesSoFar = 0;
            IPEndPoint endpoint = new IPEndPoint(deviceAddr, 9100);
            Socket sock = new Socket(deviceAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sock.SendTimeout = 1000000; //timeout in milliseconds
            try
            {
                sock.Connect(endpoint);
                byte[] filechunk = new byte[4096];
                int numBytes;
                while ((numBytes = file.Read(filechunk, 0, 4096)) > 0)
                {
                    if (sock.Send(filechunk, numBytes, SocketFlags.None) != numBytes)
                    {
                        throw new Exception("Error in sending the file");
                    }
                    bytesSoFar += numBytes;
                    Byte progress = (byte)(bytesSoFar * 100 / totalBytes);
                    if (progress > lastStatus && progress != 100)
                    {
                        Console.WriteLine("File sending progress:{0}", lastStatus);
                        lastStatus = progress;
                    }
                }
                sock.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                sock.Close();
                file.Close();
            }
            return true;
        }

        public void ReceiveFile(Socket socket, string fileName, long fileLength, DateTime? modDate) 
        {
            string filePath = Global.rootDir + fileName;
            DateTime fileModDate = modDate ?? DateTime.Now;

            try
            {
                Console.WriteLine("Waiting for filetransfer...");
                //Socket _dataSocket = FSSocket.Listen(Config.dataPort);

                try
                {
                    //Receive file
                    using (NetworkStream networkStream = new NetworkStream(socket))
                    using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate))
                    {

                        while (fileStream.Length < fileLength)
                        {
                            networkStream.CopyTo(fileStream);
                        }
                    }
                    socket.Close();
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
    }
}
