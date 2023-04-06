using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace FileSync
{
    public class FileHandler
    {

        //TODO Error handling
        //receive files based on pre-determined size
        public static void receiveFile(Socket socket, string filePath, long size, DateTime dateTimeModified)//TODO add, date last Modified --modDT
        {
            if (size != 0)
            {
                try
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally {
                    
                    if (File.Exists(filePath))
                    {
                        FileHelper.SetModifiedDateTime(filePath, dateTimeModified); //TODO enable after datetime format is fixed
                        //TODO CheckSumCheck here
                        Console.WriteLine("File transfer of {0} complete", filePath);
                    }                    
                }
            }
            else
            {
                using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
                {
                    fs.Write((byte[])null);
                }
                FileHelper.SetModifiedDateTime(filePath, dateTimeModified); //TODO enable after datetime format is fixed                               
            }
        }

        //Send the specified file over the specified socket
        //TODO Error handling (The process cannor acces the file because it is being used by another process)
        public static bool SendFile(Socket socket, string filePath)
        {
            int lastStatus = 0;
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); ;
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
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File {0} sent", filePath);
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

                string response = Connection.sendCommand(controlSocket, "PUT" + " " + fileHeader);
                if (response != null && !response.Equals("")) { 
                    int responseCode = Transformer.GetResponseCode(response);

                    if (ResponseCode.isValid(responseCode))
                    {
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

                if (ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    string fileHeader = Transformer.RemoveResponseCode(response);
                    fh.setFileHeader(fileHeader);

                    string filePath = Config.rootDir + fh.getName(); //TODO change placeholder
                    long size = fh.getSize(); //TODO change placeholder
                    DateTime dateModified = fh.getDateModified();
                    FileHandler.receiveFile(dataSocket, filePath, size, dateModified);
                }
                else
                {
                    Console.WriteLine(response);
                }
            }            
        }

        public static bool DeleteFile(string fileName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Config.rootDir + fileName);
                if (fileInfo.Exists)
                {
                    File.Delete(Config.rootDir + fileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File deletetion complete");
            }
            return true;
        }

        public static bool RenameFile(string oldName, string newName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(Config.rootDir + oldName);
                if (fileInfo.Exists)
                {
                    // Move file with a new name. Hence renamed.  
                    fileInfo.MoveTo(Config.rootDir + newName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message.ToString());
                return false;
            }
            finally
            {
                Console.WriteLine("File rename complete");
            }
            return true;
        }

        public static void synchFiles(Socket controlSocket, Socket dataSocket)
        {
            //---synch--
            //get local DIR
            var LocalfileList = FileHelper.DictFilesWithDateTime(Config.rootDir);

            //get remote DIR
            string response = Connection.sendCommand(controlSocket, "LS");
            int responseCode = Transformer.GetResponseCode(response);


            if (ResponseCode.isValid(responseCode))
            {

                string[] files = Transformer.RemoveResponseCode(response).Trim().Split(Config.fileSeperator);


                Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

                foreach (string file in files)
                {
                    if (!file.Equals(""))
                    {
                        var fileSplit = file.Trim().Split(Config.unitSeperator);
                        remoteFileList.Add(fileSplit[0], fileSplit[1]);
                    }
                }


                Dictionary<string, string> putFiles = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.LOCAL);
                Dictionary<string, string> getFiles = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.REMOTE);
                if (putFiles.Count > 0 | getFiles.Count > 0)
                {
                    //TODO check if we want to connect on PORT command or on GET command.

                    Thread t = ActionThread(() =>
                    {
                        //dataSocket.Listen();
                        //Console.WriteLine("datasocket listening on {0}", dataSocket.LocalEndPoint.ToString());
                        //Socket _dataSocket = dataSocket.Accept();

                        //Console.WriteLine("datasocket connected. Remote :" + _dataSocket.RemoteEndPoint.ToString()); //TODO keep?

                        if (getFiles.Count > 0) { FileHandler.getFiles(controlSocket, dataSocket, getFiles); }
                        Console.WriteLine("putfilelist count :" + putFiles.Count);
                        if (putFiles.Count > 0)
                        {
                            FileHandler.sendFiles(controlSocket, dataSocket, putFiles);
                        }
                        Connection.Close(controlSocket);
                        //Connection.sendCommandNoReply(controlSocket, "CLOSE" + Config.endTransmissionChar);

                    });
                    //Connection.sendCommandNoReply(controlSocket, "OPEN " + ((IPEndPoint)dataSocket.LocalEndPoint).Port); //TODO wait for response?
                }
                else
                {
                    Connection.Close(controlSocket);
                }
            }
        }

        private static Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }
    }
}
