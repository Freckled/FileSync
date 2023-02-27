using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSync
{
    class Client
    {

        private Socket _socket;
        private IPAddress _ipAdress;
        private IPEndPoint _rep;

        public Client()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            _ipAdress = host.AddressList[0];// TODO change to "server IP"
            _rep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        }

        public void start()
        {
        try
            {
                Thread mainThread = Thread.CurrentThread;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _socket.Connect(_rep);
                serverConnection(_socket);                                       
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Thread.Sleep(1000);
               
            }
        }

        private Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }


        private void serverConnection(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " is Connected to remote " + socket.RemoteEndPoint.ToString());
            byte[] msg = null;
            string command = null;
            IPEndPoint dataEndpoint = null;
            //receive code connection established
            //--?

            //receive request for DIR List
            while (socket.Connected)
            {
                byte[] data = ReceiveAll(socket);
                command = Encoding.ASCII.GetString(data, 0, data.Length);
                Socket dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

                Console.WriteLine(command);

                if (command.Equals("DIR"))
                {
                    string dirlist = "filenumber1.txt 2/19/2023 3456kb /n filenumber2.pdf 2/17/2023 365kb /n filenumber3.mp4 2/14/2023 2975kb";
                    msg = Encoding.ASCII.GetBytes(dirlist);
                    socket.Send(msg);
                }

                if (command.Contains("PORT"))
                {
                    string[] arguments = command.Split(" ");
                    int port = int.Parse(arguments[1]);
                  
                    dataEndpoint = new IPEndPoint(_ipAdress, port);
                    //Thread t = ActionThread(() => {
                    //    dataSocket.Connect(dataEndpoint);
                    //    Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote" + dataEndpoint.ToString());
                    //});
                    //t.Start();                  

                }

                if (command.Contains("PUT"))
                {
                    string[] arguments = command.Split(" ");
                    string fileName = arguments[1];
                    long filesize = long.Parse(arguments[2]);
                                                            
                    Thread t = ActionThread(() => {
                        dataSocket.Connect(dataEndpoint);
                        Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote" + dataEndpoint.ToString());

                        string fileLoc = ("D:/Filesync/Client/" + fileName);
                        //byte[] data = ReceiveLargeFile(dataSocket, filesize);
                        //SaveByteArrayToFileWithFileStream(data, fileLoc);
                        receiveFile(dataSocket, fileLoc, filesize);
                        
                        dataSocket.Close();
                    });
                    //t.Start();                  

                }


            }

                       
            //check response code 
            //--?

            //open data socket
            Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            _dataSocket.Bind(ep);

            //let the client know where to connect to and be ready to accept connection


        }

        private byte[] ReceiveAll(Socket socket)
        {
            var buffer = new List<byte>();

            //Do while zodat hij niet hangt op available. Blocked nu op receive en gaat dan door. 
            do
            {
                var currByte = new Byte[1];
                var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

                if (byteCounter.Equals(1))
                {
                    buffer.Add(currByte[0]);
                }
            }
            while (socket.Available > 0);

            return buffer.ToArray();
        }


        private static byte[] ReceiveLargeFile(Socket socket, int lenght)
        {
            // send first the length of total bytes of the data to server
            // create byte array with the length that you've send to the server.
            byte[] data = new byte[lenght];


            int size = lenght; // lenght to reveive
            var total = 0; // total bytes to received
            var dataleft = size; // bytes that havend been received 

            // 1. check if the total bytes that are received < than the size you've send before to the server.
            // 2. if true read the bytes that have not been receive jet
            while (total < size)
            {
                // receive bytes in byte array data[]
                // from position of total received and if the case data that havend been received.
                var recv = socket.Receive(data, total, dataleft, SocketFlags.None);
                if (recv == 0) // if received data = 0 than stop reseaving
                {
                    data = null;
                    break;
                }
                total += recv;  // total bytes read + bytes that are received
                dataleft -= recv; // bytes that havend been received
            }
            return data; // return byte array and do what you have to do whith the bytes.
        }




        private void receiveFile(Socket socket, string filePath, long size)
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

        private void saveFileAsync(MemoryStream inputStream, string filepath)
        {
            using (inputStream)
            {
                using Stream streamToWriteTo = File.Open(filepath, FileMode.Create);

                inputStream.Position = 0;
                inputStream.CopyToAsync(streamToWriteTo);
            }
        }






        public static void SaveByteArrayToFileWithFileStream(byte[] data, string filePath)
        {
            using var stream = File.Create(filePath);
            stream.Write(data, 0, data.Length);
        }


    }
}
