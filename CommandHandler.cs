﻿using System;
using System.IO;
using System.Net;


namespace FileSync
{
    public class CommandHandler
    {

        private string _root;
        private string _currentDirectory;
        IPEndPoint _dataEndpoint;
        Response _response;

        public CommandHandler()
        {
            _root = Config.clientDir; //@"D:\test\";//Directory.GetCurrentDirectory();
        }

        public Response getResponse(string command)
        {
            string responseString = null;
            string fileName;

            string[] commandCode = command.Split(' ');

            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = command.Length > 1 ? command.Substring(commandCode[0].Length) : null;
            arguments = arguments.Trim();

            if (string.IsNullOrWhiteSpace(arguments))
                arguments = null;

            if (responseString == null)
            {
                switch (cmd)
                {
                    case "GET":
                        fileName = arguments;
                        string fileLoc = Config.serverDir;
                        long fileSizeBytes = new FileInfo(fileLoc + fileName).Length;
                        //response = "sending " + fileName; //Retrieve(_root + "test.txt");
                        responseString = "SEND " + fileName + " " + fileSizeBytes; //Retrieve(_root + "test.txt");

                        _response = new Response(responseString, "sending file", ActionType.GETFILE, ActionType.SENDFILE, fileName, fileSizeBytes);
                      

                        break;

                    case "PORT":
                        //response = Port(arguments);
                        responseString = Port(arguments);
                        _response = new Response(responseString, "", ActionType.NONE, ActionType.NONE);
                        break;


                    //case "SEND":
                    //    fileName = arguments;
                    //    responseString = "reveiving file";
                    //    _response = new Response(responseString,"", ActionType.GETFILE,ActionType.NONE fileName);
                    //    break;

                    //case "PUT":
                    //    response = "";
                    //    break;

                    //case "DELETE":
                    //    response = "";
                    //    break;

                    //case "SIZE":
                    //    response = "";
                    //    break;

                    //case "PASV":
                    //    response = Passive();
                    //    break;

                    //case "CLOSE":
                    //    response = "";
                    //    break;

                    //case "REIN":
                    //    response = "";
                    //    break;

                    //case "REST":
                    //    response = "";
                    //    break;

                    //case "LIST":
                    //    response = "";
                    //    break;


                    //case "TEST":
                    //    responseString = "Test check";
                    //    _response = new Response(responseString, ActionType.NONE);
                    //    break;


                    default:
                        responseString = "502 Command not implemented"; ;
                        _response = new Response(responseString, responseString, ActionType.NONE, ActionType.NONE);
                        break;
                }
            }

            return _response;
        }


        private string Port(string hostPort)
        {
            //_dataConnectionType = DataConnectionType.Active;
            
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

            _dataEndpoint = new IPEndPoint(new IPAddress(ipAddress), BitConverter.ToInt16(port, 0));
            Console.WriteLine("200 Data Connection Established");
            return "200 Data Connection Established";
        }



        private string[] List()
        {
            String[] listing = Directory.GetFiles(Directory.GetCurrentDirectory());
            return listing;
        }


    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
