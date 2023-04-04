using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FileSync
{
    class FileWatcher
    {

        //Monitors the dir for any changes in files (Change, Delete, Create, Rename)
        public static void Watch()
        {
            using var watcher = new FileSystemWatcher(Config.rootDir);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            //Console.WriteLine("Press enter to exit.");
            Console.WriteLine("Monitoring root folder. Press enter to exit.");
            Console.ReadLine();
        }

        //If a file in a dir changes (and is saved)
        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            Console.WriteLine($"Changed: {e.FullPath}");
            //-------------Setup server connection--------------
            //Socket controlSocket = Connection.createSocket();
            //Socket dataSocket = Connection.createSocket();

            //if (!controlSocket.Connected) { 
            //    controlSocket.Connect(Global.remoteEP);
            //}
            //-------------Setup server connection--------------
            Socket[] sockets = Connection.ServerConnect(Global.remoteIP);
            Socket controlSocket = sockets[0];
            Socket dataSocket = sockets[1];
            //-------------Setup server connection--------------
            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                FileHeader fh = new FileHeader();
                string filePath = Config.rootDir + e.Name;
                string fileHeader = fh.getFileHeader(filePath);

                string response = Connection.sendCommand(controlSocket, "PUT" + " " + fileHeader);

                if (ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    //get datasocket and connect to it.
                    if (!dataSocket.Connected)
                    {
                        dataSocket.Connect(Global.remoteDataEP);
                    }
                    FileHandler.SendFile(dataSocket, filePath);
                    //TODO Check if move file to folder counts as created.
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
            Connection.Close(dataSocket);
        }

        //If a file in a dir is created
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);

            //-------------Setup server connection--------------
            //Socket controlSocket = Connection.createSocket();
            //Socket dataSocket = Connection.createSocket();

            //if (!controlSocket.Connected) { 
            //    controlSocket.Connect(Global.remoteEP);
            //}
            //-------------Setup server connection--------------
            Socket[] sockets = Connection.ServerConnect(Global.remoteIP);
            Socket controlSocket = sockets[0];
            Socket dataSocket = sockets[1];
            //-------------Setup server connection--------------

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                FileHeader fh = new FileHeader();
                string filePath = Config.rootDir + e.Name;
                string fileHeader = fh.getFileHeader(filePath);

                string response = Connection.sendCommand(controlSocket, "PUT" + " " + fileHeader);

                if (ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    //get datasocket and connect to it.
                    if (!dataSocket.Connected)
                    {
                        dataSocket.Connect(Global.remoteDataEP);
                    }
                    FileHandler.SendFile(dataSocket, filePath);
                    //TODO Check if move file to folder counts as created.
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
            Connection.Close(dataSocket);
        }

        //If a file in a dir is deleted
        //TODO Send/initiate command to delete file on remote from here
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");

            //-------------Setup server connection--------------
            //Socket controlSocket = Connection.createSocket();
            //Socket dataSocket = Connection.createSocket();

            //if (!controlSocket.Connected) { 
            //    controlSocket.Connect(Global.remoteEP);
            //}
            //-------------Setup server connection--------------
            Socket[] sockets = Connection.ServerConnect(Global.remoteIP);
            Socket controlSocket = sockets[0];
            Socket dataSocket = sockets[1];
            //-------------Setup server connection--------------

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(controlSocket, "DELETE " + e.Name);

                if (ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    return;
                }
                else
                {
                    numberOfRetries++;
                }    
            }
            Connection.Close(controlSocket);
            Connection.Close(dataSocket);
        }

        //If a file in a dir is renamed
        //TODO Send/initiate command to rename file on remote from here
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");

            //-------------Setup server connection--------------
            //Socket controlSocket = Connection.createSocket();
            //Socket dataSocket = Connection.createSocket();

            //if (!controlSocket.Connected) { 
            //    controlSocket.Connect(Global.remoteEP);
            //}
            //-------------Setup server connection--------------
            Socket[] sockets = Connection.ServerConnect(Global.remoteIP);
            Socket controlSocket = sockets[0];
            Socket dataSocket = sockets[1];
            //-------------Setup server connection--------------

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(controlSocket, "RENAME " + e.OldName + " " + e.Name);

                if (ResponseCode.isValid(Transformer.GetResponseCode(response)))
                {
                    FileHandler.RenameFile(e.OldName, e.Name);
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
            Connection.Close(dataSocket);
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}