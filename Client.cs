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
<<<<<<< HEAD
            _ipAdress = Config.serverIP;// TODO change to "server IP"
            _rep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
=======
            _ipAdress = Config.serverIP;// TODO change to "server IP"                 
            
>>>>>>> noel_1
        }

        //constructor with the option to give an IP adress as string to connect to.
        public Client(string ip)
        {
            _ipAdress = IPAddress.Parse(ip);
<<<<<<< HEAD
            _rep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
=======
            //_ipAdress = _ipAdress.MapToIPv6();
>>>>>>> noel_1
        }

        //start the client
        public void start()
        {
        try
            {
<<<<<<< HEAD
                Thread mainThread = Thread.CurrentThread;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
=======
                _socket = Connection.createSocket();
                _rep = new IPEndPoint(_ipAdress, Config.serverPort);
                
                //Thread mainThread = Thread.CurrentThread;
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
>>>>>>> noel_1
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
<<<<<<< HEAD
                byte[] data = Connection.ReceiveAll2(socket);
                command = Encoding.UTF8.GetString(data, 0, data.Length);
                Socket dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
=======
                byte[] data = Connection.ReceiveAll(socket);
                command = Encoding.UTF8.GetString(data, 0, data.Length);
                Socket dataSocket = Connection.createSocket();// new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

>>>>>>> noel_1

                if (commandHandler == null)
                {
                    commandHandler = new CommandHandler(socket, dataSocket);
                }

                commandHandler.processCommand(command, CommandHandler.Device.CLIENT);

                Console.WriteLine(command);
            }

<<<<<<< HEAD
            Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            _dataSocket.Bind(ep);
=======
            //Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            //_dataSocket.Bind(ep);
>>>>>>> noel_1

            //let the client know where to connect to and be ready to accept connection
        }

    }
}
