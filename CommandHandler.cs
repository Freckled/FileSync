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
            _root = Config.clientDir;
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
                        string filePath = Global.rootDir + fileName;
                        long fileSize = new FileInfo(filePath).Length;
                        string fileModDate = FileHelper.GetModifiedDateTime(filePath).ToString(Config.cultureInfo);
                        message = "SEND " + fileName + " " + fileSize + " " + fileModDate;
                        _response = new Response(message, ActionType.SENDFILE, fileName);
                        break;

                    case "PORT":
                        message = Port(arguments);
                        _response = new Response(message, ActionType.NONE);
                        break;


                    case "SEND":
                        var argSplit = arguments.Split(' ');
                        fileName = argSplit[0];
                        fileSize = (long)Convert.ToDouble(argSplit[1]);
                        var ModDate = DateTime.Parse(argSplit[2] + " " + argSplit[3], Config.cultureInfo);
                        message = "reveiving file";
                        _response = new Response(message, ActionType.GETFILE, fileName, fileSize, ModDate);
                        break;

                    case "LIST":
                        message = "DIRLIST";
                        var files = FileHelper.listFilesWithDateTime(Global.rootDir);
                        foreach(var file in files)
                        {
                            message += " " + file.Key + "|" + file.Value.Replace(" ","|");
                        }                        
                        _response = new Response(message, ActionType.NONE);
                        break;

                    case "DIRLIST":
                        message = "Listing"; //if lists are equal change to nothing
                        var LocalfileList = FileHelper.DictFilesWithDateTime(Global.rootDir);
                        var argSplitFiles = arguments.Split(' ');

                        Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

                        foreach (string file in argSplitFiles)
                        {
                            var fileSplit = file.Split("|");
                            remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
                        }

                        var files2Get = FileHelper.CompareDir(LocalfileList, remoteFileList, outPutNewest.REMOTE);
                        _response = new Response(message, ActionType.GETFILES, files2Get);
                        break;

                    case "ASKLIST":
                        message = "LIST";
                        _response = new Response(message, ActionType.NONE);
                        break;

                    case "TEST":
                        message = "Test check";
                        _response = new Response(message, ActionType.NONE);
                        break;

                    case "DELETE":
                        message = "deleting file";
                        fileName = arguments;
                        FileInfo fiDel = new FileInfo(Global.rootDir + fileName);
                        if (fiDel.Exists)
                        {
                            // Move file with a new name. Hence renamed.  
                            File.Delete(Global.rootDir + fileName);
                        }                        
                        _response = new Response(message, ActionType.NONE);
                        break;

                    case "RENAME":
                        string [] argsplit = arguments.Split(' ');
                        string oldFileName = argsplit[0];
                        string newFileName = argsplit[1];
                        message = "renaming file "+ oldFileName + " to "+ newFileName;
                        FileInfo fi = new FileInfo(Global.rootDir + oldFileName);
                        if (fi.Exists)
                        {
                            // Move file with a new name. Hence renamed.  
                            fi.MoveTo(Global.rootDir + newFileName);
                        }
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

    }
}
