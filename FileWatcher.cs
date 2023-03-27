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
        public static void Watch(Socket _socket = null)
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
               // Program.SyncFiles(Global.remoteIP);
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");

            Socket controlSocket = Connection.createSocket();
            
            if (!controlSocket.Connected) { 
                controlSocket.Connect(Global.remoteEP);
            }

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(controlSocket, "PUT " + e.Name);
                Int32.TryParse(response, out int responseCode);

                if (ResponseCode.isValid(responseCode))
                {
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
        }

        //If a file in a dir is created
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);

            Socket controlSocket = Connection.createSocket();

            if (!controlSocket.Connected)
            {
                controlSocket.Connect(Global.remoteEP);
            }

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(DisposeSocketFileWatcher.current(), "PUT " + e.Name);
                Int32.TryParse(response, out int responseCode);

                if (ResponseCode.isValid(responseCode))
                {
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
        }

        //If a file in a dir is deleted
        //TODO Send/initiate command to delete file on remote from here
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");

            Socket controlSocket = Connection.createSocket();

            if (!controlSocket.Connected)
            {
                controlSocket.Connect(Global.remoteEP);
            }

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(DisposeSocketFileWatcher.current(), "DELETE " + e.Name);
                Int32.TryParse(response, out int responseCode);

                if (ResponseCode.isValid(responseCode))
                {
                    return;
                }
                else
                {
                    numberOfRetries++;
                }    
            }
            Connection.Close(controlSocket);
        }

        //If a file in a dir is renamed
        //TODO Send/initiate command to rename file on remote from here
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");

            Socket controlSocket = Connection.createSocket();

            if (!controlSocket.Connected)
            {
                controlSocket.Connect(Global.remoteEP);
            }

            var tries = 0;
            var numberOfRetries = 3;
            while (tries <= numberOfRetries)
            {
                string response = Connection.sendCommand(DisposeSocketFileWatcher.current(), "RENAME " + e.OldName + " " + e.Name);
                Int32.TryParse(response, out int responseCode);

                if (ResponseCode.isValid(responseCode))
                {
                    return;
                }
                else
                {
                    numberOfRetries++;
                }
            }
            Connection.Close(controlSocket);
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