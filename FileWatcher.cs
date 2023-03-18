using System;
using System.IO;
using System.Net;

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
               // Program.SyncFiles(Global.remoteIP);
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        //If a file in a dir is created
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        //If a file in a dir is deleted
        //TODO Send/initiate command to delete file on remote from here
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"Deleted: {e.FullPath}");
            IPEndPoint endPoint = new IPEndPoint(Global.remoteIP, Config.serverPort);

            //Old command, leaving it here for now TODO delete later
            //Connection con = new Connection(endPoint);
            //con.sendCommand("delete " + e.Name);

            //Check of socket nog open is

        }

        //If a file in a dir is renamed
        //TODO Send/initiate command to rename file on remote from here
        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");

            IPEndPoint endPoint = new IPEndPoint(Global.remoteIP, Config.serverPort);

            //Old command, leaving it here for now TODO delete later
            //Connection con = new Connection(endPoint);
            //con.sendCommand("rename " + e.OldName + " " + e.Name);

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