﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FileSync
{
    public class CommandHandler
    {

        private string _root;
        private string _currentDirectory;
        IPEndPoint dataEndpoint;
        Socket socket;
        Socket dataSocket;
        IPAddress iPAddress;

        public enum Device
        {
            CLIENT,
            SERVER
        }

        public CommandHandler(Socket _socket, Socket _dataSocket)
        {
            _root = Config.rootDir;
            socket = _socket;
            dataSocket = _dataSocket;
        }

        public void processCommand(string _command, Device _device)
        {
            //Return Response code. 
            if (_command == "")
            {
                return;
            }

            Console.WriteLine(_command);

            string[] commandCode = _command.Split(' ');

            //Makes sure String is CAPS
            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = _command.Length > 1 ? _command.Substring(commandCode[0].Length) : null;
            arguments = arguments.Trim();

            if (string.IsNullOrWhiteSpace(arguments))
            { 
                arguments = null;
            }

            switch (cmd)
            {
                case "GET":
                    break;

                case "PUT":
                    this.executePut(_command);
                    break;

                case "DELETE":
                    break;

                case "SIZE":
                    break;

                case "PORT":
                    this.executePort(_command);
                    //Misschien dataEndPoint zetten tijdens PORT command. Komt van Server. 
                    break;

                case "CLOSE":
                    break;

                case "DIR":
                    this.executeDir(_command);
                    break;

                default:
                    throw new Exception("Command " + _command + " is not supported.");
            }

            //return response code. Like 200, 205 etc...
        }

        private void executeDir(string _command)
        {

            List<KeyValuePair<string, string>> localfiles = FileHelper.listFilesWithDateTime(Config.rootDir);
            string dirList = "";
            //generate string from list
            foreach (KeyValuePair<string, string> file in localfiles)
            {
                dirList = dirList + file.Key + " " + file.Value + Config.linebreak;

            }

            //string dirList = "filenumber1.txt 2/19/2023 3456kb"+Config.linebreak+ "filenumber2.pdf 2/17/2023 365kb"+Config.linebreak+"filenumber3.mp4 2/14/2023 2975kb"+Config.endTextChar;
            byte[] msg = Encoding.UTF8.GetBytes(dirList);
            socket.Send(msg);
        }

        private void executePort(string _command)
        {
            string[] arguments = _command.Split(" ");
            int port = int.Parse(arguments[1]);

            //TOD change adres. IPV6Any
            dataEndpoint = new IPEndPoint(((IPEndPoint)socket.RemoteEndPoint).Address, port);                 
            //dataEndpoint = new IPEndPoint(((IPEndPoint)socket.RemoteEndPoint).Address, Config.dataPort);           
        }

        private void executePut(string _command)
        {
            string[] arguments = _command.Split(" ");
            string fileName = arguments[1];
            long filesize = long.Parse(arguments[2]);

            //Check if datasocket is connected
            if (dataSocket.Connected)
            {
                string fileLoc = (Config.rootDir + fileName);
                FileHandler.receiveFile(dataSocket, fileLoc, filesize);
            }

            if (dataEndpoint == null)
            {
                throw new Exception("Method executePut threw an error. No data end point is set.");
            }

            Thread t = ActionThread(() => {
                dataSocket.Connect(dataEndpoint);
                Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote" + dataEndpoint.ToString());

                string fileLoc = (Config.rootDir + fileName);
                FileHandler.receiveFile(dataSocket, fileLoc, filesize);
                dataSocket.Close();
            });

            //send confirmation or request file again??             
        }

        private Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }

        private string Port(string hostPort)
        {         
            string[] ipAndPort = hostPort.Trim().Split(',');

            byte[] ipAddress = new byte[4];
            byte[] port = new byte[2];

            for (int i = 0; i < 4; i++)
            {
                ipAddress[i] = Convert.ToByte(ipAndPort[i]);
            }

            for (int i = 4; i < 6; i++)
            {
                port[i - 4] = Convert.ToByte(ipAndPort[i]);
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(port);

            dataEndpoint = new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt16(port, 0));
            Console.WriteLine("200 Data Connection Established");
            return "200 Data Connection Established";
        }

    }
}
