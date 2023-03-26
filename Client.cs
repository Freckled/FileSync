using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private int? _port;

        public Client()
        {            
            _ipAdress = Config.serverIP;// TODO change to "server IP"                 
            
        }

        //constructor with the option to give an IP adress as string to connect to.
        public Client(string ip)
        {
            _ipAdress = IPAddress.Parse(ip);            
        }

        public Client(string ip, string port)
        {
            _ipAdress = IPAddress.Parse(ip);
            _port = int.Parse(port);            
        }

        //start the client
        public void start()
        {
        try
            {
                int port = _port ?? Config.serverPort;
                _socket = Connection.createSocket();
                _rep = new IPEndPoint(_ipAdress, port);
                
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                
                _socket.Connect(_rep);
                serverConnection(_socket);                                       
            }catch(SocketException e)
            {
                Console.WriteLine("No server listening on specified address : {0}", _rep.ToString());
                Thread.Sleep(2000);
                Program.restart();

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
            Socket dataSocket = Connection.createSocket();// new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            while (socket.Connected)
            {                
                command = Transformer.ParseByteArrString(Connection.ReceiveAll(socket));
                string[] responsecode = command.Split(" ");
                                

                if (commandHandler == null)
                {
                    commandHandler = new CommandHandler(socket, dataSocket);
                }

                commandHandler.processCommand(command, CommandHandler.Device.CLIENT);
            }
        }

    }
}
