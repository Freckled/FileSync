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
        
        //TODO Error handling
        //receive files based on pre-determined size
        public static void receiveFile(Socket socket, string filePath, long size)
        {
            using (socket)
            {
                socket.Listen();
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    byte[] buffer = new byte[8192];
                    int read;
                    int bytesSoFar = 0; //Use this to keep track of how many bytes have been read

                    do
                    {
                        read = socket.Receive(buffer);
                        fs.Write(buffer, 0, read);
                        bytesSoFar += read;

                    } while (bytesSoFar < size);
                }
            }
        }

        //Send the specified file over the specified socket
        //TODO Error handling (The process cannor acces the file because it is being used by another process)
        public static bool SendFile(Socket socket, string filePath)
        {
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read); ;
            long totalBytes = file.Length, bytesSoFar = 0;
            socket.SendTimeout = 1000000; //timeout in milliseconds
            try
            {
                byte[] filechunk = new byte[4096];
                int numBytes;
                while ((numBytes = file.Read(filechunk, 0, 4096)) > 0)
                {
                    if (socket.Send(filechunk, numBytes, SocketFlags.None) != numBytes)
                    {
                        throw new Exception("Error in sending the file");
                    }
                    bytesSoFar += numBytes;
                    Byte progress = (byte)(bytesSoFar * 100 / totalBytes);
                    if (progress > lastStatus && progress != 100)
                    {
                        //Console.WriteLine(".");
                        Console.Write(".");
                        lastStatus = progress;
                    }
                }
                //socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File send complete");
                //socket.Close();
                file.Close();
            }
            return true;
        }


        public static void sendFiles(Socket controlSocket, Socket dataSocket, Dictionary<string, string> fileList)
        {
            FileHeader fh = new FileHeader();
            foreach (KeyValuePair<string, string> file in fileList)
            {
                string filePath = Config.rootDir + file.Key;
                //create fileheader
                string fileHeader = fh.getFileHeader(filePath); ;//Filehandlder.filehead(filepath)

                //send fileheader over socket.
                //string[] response = Connection.sendCommand(controlSocket, fileHeader).Split(" ");
                //int responseCode = int.Parse(response[0]);

                //TODO change to wait for response (Connection.sendCommandReply) before sending file (needs change in PUT commandhandler item to send response code)
                //response = Connection.sendCommand(controlSocket, "PUT" + " " + fh.getName() + " " + fh.getSize()).Split(" ");
                string response = Connection.sendCommand(controlSocket, "PUT" + " " + fileHeader);
                if (response != null) { 
                    string[] responseArr = response.Split(" ");
                    int responseCode = int.Parse(responseArr[0]);

                    if (ResponseCode.isValid(responseCode))
                    {
                        dataSocket.Connect(dataSocket.RemoteEndPoint);
                        FileHandler.SendFile(dataSocket, filePath);
                    }
                }
            }
        }

        public static void getFiles(Socket controlSocket, Socket dataSocket, Dictionary<string, string> fileList)
        {
            FileHeader fh = new FileHeader();
            foreach (KeyValuePair<string, string> file in fileList)
            {
                //openDataStream;
                
                //get fileheader
                string response = Connection.sendCommand(controlSocket, "GET" + " " + file.Key);
                //parse fileheader
                
                if(ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    string fileHeader = Transformer.RemoveResponseCode(response);
                    fh.setFileHeader(fileHeader);

                    string filePath = Config.rootDir + fh.getName(); //TODO change placeholder
                    long size = fh.getSize(); //TODO change placeholder
                    dataSocket.Listen();
                    dataSocket.Accept();
                    FileHandler.receiveFile(dataSocket, filePath, size);
                }
                else
                {
                    Console.WriteLine(response);
                }


            }

        }


    }
}
