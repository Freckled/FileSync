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
    public class FileHandler
    {
        private IPEndPoint _endPoint;
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public FileHandler(IPEndPoint endpoint)
        {
            _endPoint = endpoint;            
        }

        public string SendFile(string fileName)
        {
            string fileLoc = Config.serverDir + fileName;
            _socket.Connect(_endPoint);
            
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
                _socket.SendFile(fileLoc);

                // Release the socket.
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                Console.WriteLine("File Transfer started");
                return "File transfer started";


            }
            catch (SocketException se)
            {
                return "File transfer failed";
            }

        }

        public string GetFile(string fileName, long fileLength)
        {
            try
            {
                // Create a Socket that will use Tcp protocol IP & PORT in config class
                IPAddress ipAddress = IPAddress.Parse(Config.serverIp);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Config.dataPort);
                string fileLocation = Config.serverDir + fileName;


                _socket.Bind(localEndPoint);
                //_socket.Connect(remoteEndPoint);
                _socket.Listen(10);

                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = _socket.Accept();

                ///receive file

                try
                {
                    using (NetworkStream networkStream = new NetworkStream(_dataSocket))
                    using (FileStream fileStream = File.Open(fileLocation, FileMode.OpenOrCreate))
                    {

                        while (fileStream.Length < fileLength)
                        {
                            networkStream.CopyTo(fileStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw;

                }

                Console.WriteLine("transfer complete");
                return "Filetransfer complete";
            }
            catch (Exception e)
            {
                return "Error transferring files";
            }
        }







    }
}
