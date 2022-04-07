using System;

namespace FileSync
{
    class Program
    {
        static void Main(string[] args)
        {
           // Client client = new Client();
           // client.start();

            FileWatcher watcher = new FileWatcher();
            FileWatcher.Watch();
        }
    }
}
