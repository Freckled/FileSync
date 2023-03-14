﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSync
{
    public class Server
    {
        private Socket _socket;
        private IPAddress _ipAdress;
        private IPEndPoint _ep;
        
        public Server()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            _ipAdress = IPAddress.IPv6Any; //host.AddressList[0];
            _ep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = Connection.createSocket();

        }

        //start server
        public void start()
        {
            reboot:
            try
            {
                _socket.Bind(_ep);
                //create a loop so it keeps listening
                while (true)
                {
                    
                    Thread mainThread = Thread.CurrentThread;
                    
                    _socket.Listen(Config.serverPort);
                    Socket client = _socket.Accept();
                    
                    Thread t = ActionThread(() => {
                        clientConnection(client);
                    });

                    //t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("restarting server...");
                Thread.Sleep(1000);
                goto reboot;
            }
        }

        //creates a new thread, puts the action in there and starts it.
        private Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }

        //handles connection with the client (todo, replace commandHandler part of communications)
        private void clientConnection(Socket controlSocket)
        {

            //Assign data socket
            Socket _dataSocket = Connection.createSocket();
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            //IPEndPoint ep = new IPEndPoint(_ipAdress, Config.dataPort);
            _dataSocket.Bind(ep);


            //TESTING synch files
            synchFiles(controlSocket, _dataSocket);
                     
        }


        private void synchFiles(Socket controlSocket, Socket dataSocket)
        {
            //---synch--
            //get local DIR
            var LocalfileList = FileHelper.DictFilesWithDateTime(Config.rootDir);

            //get remote DIR
            string response =  Connection.sendCommand(controlSocket, "DIR" + Config.endTextChar);


            string[] files = response.Split(Config.linebreak);
            Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

            foreach (string file in files)
                {
                    if (!file.Equals(""))
                    {
                        var fileSplit = file.Trim().Split(" ");
                        remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
                    }
                }

            
            //--------TODO turn off after testing
            remoteFileList = FileHelper.DictFilesWithDateTime(Config.testDir);
            //--------TODO turn off after testing

            Dictionary<string, string> putFiles = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.LOCAL);
            Dictionary<string, string> getFiles = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.REMOTE);

            //Connection.sendCommand(controlSocket, "PORT" + " " + ((IPEndPoint)dataSocket.LocalEndPoint).Port);
            Connection.sendCommandNoReply(controlSocket, "PORT " + ((IPEndPoint)dataSocket.LocalEndPoint).Port);

            //TODO check if we want to connect on PORT command or on GET command.
            Thread t = ActionThread(() =>
            {                
                dataSocket.Listen();

                FileHandler.getFiles(controlSocket, dataSocket, getFiles);               
                FileHandler.sendFiles(controlSocket, dataSocket, putFiles);

                dataSocket.Close();
            });

            


        }

    }
}
