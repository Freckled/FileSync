using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class Config
    {
        public static readonly string clientDirectory = @"C:\FileWatcher";
        public static readonly string serverDirectory = @"C:\FileWatcherTo";
        public static readonly string clientIp = "127.0.0.1";
        public static readonly int clientPort = 2503;
        public static readonly string serverIp = "127.0.0.1";
        public static readonly int serverPort = 2503;
        public static readonly string host = "ftp://127.0.0.1:2503/C:/FileWatcherTo";
        public static string clientDir = @"c:\FileWatcher\";
        public static string serverDir = "C:/FileWatcherTo/";
    }
}
