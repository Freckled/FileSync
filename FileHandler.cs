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
        public static bool SendFile(Socket socket, string filePath)
        {
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open); ;
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
                        Console.WriteLine(".");
                        lastStatus = progress;
                    }
                }
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File send complete");
                socket.Close();
                file.Close();
            }
            return true;
        }

        public static bool DeleteFile(Socket socket, string filePath)
        {
            FileStream file = new FileStream(filePath, FileMode.Open); ;
            try
            {
                FileInfo fileInfo = new FileInfo(Global.rootDir + file.Name);
                if (fileInfo.Exists)
                {
                    File.Delete(Global.rootDir + file.Name);
                }
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File deletetion complete");
                socket.Close();
                file.Close();
            }
            return true;
        }

        public static bool RenameFile(Socket socket, string filePath)
        {
            string newName;
            string oldName;
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open); ;
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
                        Console.WriteLine(".");
                        lastStatus = progress;
                    }
                }
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File send complete");
                socket.Close();
                file.Close();
            }
            return true;
        }

    }
}
