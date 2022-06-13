using System;
using System.Collections.Generic;
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
            string message = null;
            string fileName;

            string[] commandCode = command.Split(' ');

            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = command.Length > 1 ? command.Substring(commandCode[0].Length) : null;
            arguments = arguments.Trim();

            if (string.IsNullOrWhiteSpace(arguments))
                arguments = null;

            if (message == null)
            {
                switch (cmd)
                {
                    case "GET":
                        fileName = arguments;
                        string filePath = Config.serverDir + fileName;
                        long fileSize = new FileInfo(filePath).Length;
                        string fileModDate = FileHelper.GetModifiedDateTime(filePath).ToString(Config.cultureInfo);
                        //response = "sending " + fileName; //Retrieve(_root + "test.txt");
                        message = "SEND " + fileName + " " + fileSize + " " + fileModDate; //Retrieve(_root + "test.txt");

                        _response = new Response(message, ActionType.SENDFILE, fileName);

                        break;

                    case "PORT":
                        //response = Port(arguments);
                        message = Port(arguments);
                        _response = new Response(message, ActionType.NONE);
                        break;


                    case "SEND":
                        var argSplit = arguments.Split(' ');
                        fileName = argSplit[0]; //arguments;
                        fileSize = (long)Convert.ToDouble(argSplit[1]);
                        var ModDate = DateTime.Parse(argSplit[2] + " " + argSplit[3], Config.cultureInfo);
                        message = "reveiving file";
                        _response = new Response(message, ActionType.GETFILE, fileName, fileSize, ModDate);
                        break;

                    case "LIST":
                        message = "DIRLIST";
                        var files = FileHelper.listFilesWithDateTime(Config.serverDir);
                        foreach(var file in files)
                        {
                            message += " " + file.Key + "|" + file.Value.Replace(" ","|");
                        }                        
                        _response = new Response(message, ActionType.NONE);
                        break;

                    case "DIRLIST":
                        message = "test"; //if lists are equal change to nothing
                        var LocalfileList = FileHelper.DictFilesWithDateTime(Config.clientDir);
                        //var LocalfileList = FileHelper.listFilesWithDateTime(Config.clientDir);
                        var argSplitFiles = arguments.Split(' ');
                        //List<KeyValuePair<string, string>> remoteFileList = new List<KeyValuePair<string, string>>(); 
                        Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

                        foreach (string file in argSplitFiles)
                        {
                            var fileSplit = file.Split("|");
                            remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
                        }

                        //List<string> fileListToGet = FileHelper.CompareFileList(LocalfileList, remoteFileList);
                        var files2Get = FileHelper.CompareDir(LocalfileList, remoteFileList);
                        //var files2Get = FileHelper.CompareFileList2(LocalfileList, remoteFileList);
                        _response = new Response(message, ActionType.GETFILES, files2Get);
                        break;

                    case "ASKLIST":
                        message = "LIST";
                        _response = new Response(message, ActionType.SENDMESSAGE);
                        break;

                    case "TEST":
                        message = "Test check";
                        _response = new Response(message, ActionType.NONE);
                        break;


                    default:
                        message = "502 Command not implemented"; ;
                        _response = new Response(message, ActionType.NONE);
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
