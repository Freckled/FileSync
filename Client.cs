using System;
using System.Collections.Generic;
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
            Console.WriteLine("Connected to " + socket.RemoteEndPoint.ToString());
            byte[] msg = null;
            string command = null;
            //receive code connection established
            //--?

            //receive request for DIR List
            while (socket.Connected)
            {
                byte[] data = ReceiveAll(socket);
                command = Encoding.ASCII.GetString(data, 0, data.Length);

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
                  
                    Socket dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint dataEndpoint = new IPEndPoint(_ipAdress, port);
                    Thread t = ActionThread(() => {
                        dataSocket.Connect(dataEndpoint);
                        Console.WriteLine("Connected to " + dataEndpoint.ToString());
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

    }
}
