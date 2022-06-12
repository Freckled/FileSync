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

        public static void GetFile(IPEndPoint endPoint, long fileLength)
        {
            
            try
            {
                // Create a Socket that will use Tcp protocol
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                //_socket.Connect(remoteEndPoint);
                socket.Listen(10);

                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = socket.Accept();

                ///receive file

                try
                {
                    using (NetworkStream networkStream = new NetworkStream(_dataSocket))
                    using (FileStream fileStream = File.Open("D:\\FileWatcherTo\\test.txt", FileMode.OpenOrCreate))
                    {

                        while (fileStream.Length < fileLength)
                        {
                            networkStream.CopyTo(fileStream);
                        }
                    }
                    _dataSocket.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;

                }
            }
            catch (Exception e)
            {
                
            }
        }

        public static void SendFile(IPEndPoint endPoint, string fileName)
        {
            //string fileName = "D:\\FileWatcher\\test.txt";
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endPoint);
            string fileLoc = Config.serverDir + fileName;

            // Create the preBuffer data.
            try
            {
                string string1 = String.Format("This is text data that precedes the file.{0}", Environment.NewLine);
                byte[] preBuf = Encoding.ASCII.GetBytes(string1);

                // Create the postBuffer data.
                string string2 = String.Format("This is text data that will follow the file.{0}", Environment.NewLine);
                byte[] postBuf = Encoding.ASCII.GetBytes(string2);

                //Send file fileName with buffers and default flags to the remote device.
                Console.WriteLine("Sending {0} with buffers to the host.{1}", fileLoc, Environment.NewLine);
                //_socket.SendFile(fileName, preBuf, postBuf, TransmitFileOptions.UseDefaultWorkerThread);
                socket.SendFile(fileLoc);
                Console.WriteLine("File Transfer started");

                // Release the socket.
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (SocketException se)
            {
                
            }            
        }

        public static void GetFiles(IPEndPoint endPoint, List<KeyValuePair<string, string>> list)
        {
            Connection conn = new Connection(endPoint);
            CommandHandler cmd = new CommandHandler();

            for (int i = 0; i < list.Count; i++)
            {                
                string resp = conn.sendCommand("get " + list[i].Key);
                Response response = cmd.getResponse(resp);
                response.runAction(endPoint);
            }

        }

    }
}
