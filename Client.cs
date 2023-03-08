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

        //start the client
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

        //Handle server connection
        private void serverConnection(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " is Connected to remote " + socket.RemoteEndPoint.ToString());
            string command = null;


            CommandHandler commandHandler = null;

            while (socket.Connected)
            {
                byte[] data = ReceiveAll(socket);
                command = Encoding.ASCII.GetString(data, 0, data.Length);
                Socket dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

                if (commandHandler == null)
                {
                    commandHandler = new CommandHandler(socket, dataSocket);
                }

                commandHandler.processCommand(command, CommandHandler.Device.CLIENT);

                Console.WriteLine(command);
            }

            Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            _dataSocket.Bind(ep);

            //let the client know where to connect to and be ready to accept connection
        }

        //used for comms channel, might replace later
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
