using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class Config
    {
        public static readonly string clientDirectory = @"D:\Test";
        public static readonly string serverDirectory = @"D:\Test\drop";
        public static readonly string clientIp = "127.0.0.1";
        public static readonly int clientPort = 2503;
        public static readonly string serverIp = "192.168.1.144";
        public static readonly int serverPort = 2503;
        public static readonly string host = "ftp://127.0.0.1:2503/C:/FileWatcherTo";
        public static string clientDir = @"d:\FileWatcher\";
        public static string serverDir = "D:/FileWatcherTo/";
        public static readonly int dataPort = 11305;
    }
}
