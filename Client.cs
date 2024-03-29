﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FileSync
{
    class Client
    {

        private Socket _socket;
        private IPAddress _ipAdress;
        private IPEndPoint _rep;
        private int? _port;
        public Socket serverControlSocket;

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
                Global.remoteIP = _ipAdress;
                Socket[] sockets = Connection.ServerConnect(_ipAdress);
                _socket = sockets[0];
                Socket _dataSocket = sockets[1];

                serverConnection(_socket, _dataSocket);

            }catch(SocketException e)
            {
                if (e.ErrorCode == 10057)
                {
                }
                else
                {
                    Console.WriteLine("No server listening on specified address : {0}", _ipAdress.ToString());
                    Thread.Sleep(2000);
                    Program.restart();
                }


            }         
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Thread.Sleep(1000);
            }

            FileWatcher.Watch();

        }


        //Handle server connection
        private void serverConnection(Socket socket, Socket dataSocket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " is Connected to remote " + socket.RemoteEndPoint.ToString());
            Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote " + dataSocket.RemoteEndPoint.ToString());
            string command = null;


            CommandHandler commandHandler = null;
            Connection.sendCommandNoReply(socket, "SYNC");

            while (socket.Connected)
            {

                Console.WriteLine("Waiting for response from remote..");
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
