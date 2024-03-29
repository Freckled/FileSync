﻿using System.Globalization;
using System.IO;
using System.Net;


namespace FileSync
{
    public class Config
    {
        public static readonly string rootDir = Directory.GetCurrentDirectory() + "\\Home\\";//TODO change name        
        public static readonly int serverPort = 42069;
        public static readonly int dataPort = 42068;
        public static readonly IPAddress serverIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]; //TODO change to serverIP if known
        public static readonly CultureInfo cultureInfo = new CultureInfo("en-GB");
        public static readonly string dateTimeFormat = "s";
        public static readonly string linebreak = "\n";
        public static readonly string endTransmissionChar = "\u0004";
        public static readonly string endTextChar = "\u0003";        
        public static readonly string fileSeperator = "\u001C";
        public static readonly string unitSeperator = "\u001F";
        public static readonly string checkSumAlgo = "MD5";
    }
}
