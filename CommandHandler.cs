﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Schema;

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

            Console.WriteLine("Command received: {0}",_command);

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
                    this.executeGet(_command);
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
                    this.executeClose(_command);
                    break;

                case "LS":
                    this.executeDir(_command);
                    break;

                default:
                    //throw new Exception("Command " + _command + " is not supported."); //WHY????
                    Connection.sendCommandNoReply(socket, "Command" + _command + " is not supported.");
                    break;
            }

            //return response code. Like 200, 205 etc...
        }

        private void executeClose(string _command)
        {
            Console.WriteLine("Server orders close. Shutting down connections.");
            FileWatcher.Watch();

        }


        private void executeDir(string _command)
        {

            List<KeyValuePair<string, string>> localfiles = FileHelper.listFilesWithDateTime(Config.rootDir);
            string dirList = "";
            //generate string from list
            foreach (KeyValuePair<string, string> file in localfiles)
            {
                //dirList = dirList + file.Key + " " + file.Value + Config.linebreak; //old remain until new tested
                dirList = dirList + file.Key + Config.unitSeperator + file.Value + Config.fileSeperator;

            }
            
            
            dirList = "200" + " " + dirList;
            
            //TODO leave in or just send empty dirlist??
            if (dirList.Equals("")){ dirList = "400 empty_dir"; }
            //string dirList = "filenumber1.txt 2/19/2023 3456kb"+Config.linebreak+ "filenumber2.pdf 2/17/2023 365kb"+Config.linebreak+"filenumber3.mp4 2/14/2023 2975kb"+Config.endTextChar;
            //byte[] msg = Encoding.UTF8.GetBytes(dirList);
            //socket.Send(msg);

            Connection.sendCommandNoReply(socket, dirList);


        }

        private void executePort(string _command)
        {
            string[] arguments = _command.Split(" ");
            int port = int.Parse(arguments[1]);

            //TOD change adres. IPV6Any
            dataEndpoint = new IPEndPoint(((IPEndPoint)socket.RemoteEndPoint).Address, port);
            //dataEndpoint = new IPEndPoint(((IPEndPoint)socket.RemoteEndPoint).Address, Config.dataPort);
            if (!dataSocket.Connected)
            {
                dataSocket.Connect(dataEndpoint);
            }
        }

        private void executePut(string _command)
        {
            
            
            //string[] arguments = command.Split(":");
            //string fileName = arguments[1];
            //long filesize = long.Parse(arguments[2]);
            //string fileHeader = arguments[1];
            
            string fileHeader = Transformer.RemoveCommand(_command);
            FileHeader fh = new FileHeader();
            fh.setFileHeader(fileHeader);
            string fileName = fh.getName();
            long filesize = fh.getSize();
            DateTime dateModified = fh.getDateModified();

            ////Check if datasocket is connected
            //if (dataSocket.Connected)
            //{
            //    string fileLoc = (Config.rootDir + fileName);
            //    FileHandler.receiveFile(dataSocket, fileLoc, filesize);
            //}

            if (dataEndpoint == null)
            {
                throw new Exception("Method executePut threw an error. No data end point is set.");
            }

            Thread t = ActionThread(() => {
                Connection.sendCommandNoReply(socket, "200 Ready_to_receive");
                if (!dataSocket.Connected) { 
                    dataSocket.Connect(dataEndpoint); 
                    Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote" + dataEndpoint.ToString());
                }

                string fileLoc = (Config.rootDir + fileName);
                FileHandler.receiveFile(dataSocket, fileLoc, filesize, dateModified);
                //dataSocket.Close();
            });

            //send confirmation or request file again??             
        }


        private void executeGet(string _command)
        {
            //string[] arguments = _command.Split(" ");
            //string fileName = arguments[1];
            //long filesize = long.Parse(arguments[2]);
            //string fileName = arguments[1];
            string fileName = Transformer.RemoveResponseCode(_command).Trim();
            string filePath = Config.rootDir + fileName;
            
            FileHeader fh = new FileHeader();

            Thread t = ActionThread(() => {
                if (!File.Exists(filePath))
                {
                    Connection.sendCommandNoReply(socket, "300 file_not_found");
                    return;
                }
                else
                {
                    string fileHeader = fh.getFileHeader(filePath);
                    Connection.sendCommandNoReply(socket, "200 " + fileHeader);
                }

                if (dataEndpoint == null)
                {
                    throw new Exception("Method executePut threw an error. No data end point is set.");
                }

                //Check if datasocket is connected
                if (!dataSocket.Connected)
                {
                    dataSocket.Connect(dataEndpoint);
                    Console.WriteLine(dataSocket.LocalEndPoint.ToString() + " is Connected to remote" + dataEndpoint.ToString());
                }

                string fileLoc = (filePath);
                FileHandler.SendFile(dataSocket, fileLoc);

                //dataSocket.Close();
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
