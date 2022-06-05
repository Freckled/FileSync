using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class CommandHandler2
    {

        private enum DataConnectionType
        {
            Passive,
            Active,
        }

        private TcpClient _controlClient;
        private TcpClient _dataClient;
        private TcpListener _passiveListener;
        private string _root;

        private NetworkStream _controlStream;
        private NetworkStream _dataConnection;

        private string _currentDirectory;
        private IPEndPoint _dataEndpoint;
        private IPEndPoint _remoteEndPoint;
        private DataConnectionType _dataConnectionType;


        public CommandHandler2()
        {
            _root = Config.clientDir; //@"D:\test\";//Directory.GetCurrentDirectory();
        }

        public string getResponse(string command)
        {
            string response = null;

            string[] commandCode = command.Split(' ');

            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = command.Length > 1 ? command.Substring(commandCode[0].Length) : null;

            if (string.IsNullOrWhiteSpace(arguments))
                arguments = null;

            if (response == null)
            {
                switch (cmd)
                {
                    //case "OPEN":
                    //    response = "";
                    //    break;

                    //case "LS":
                    //    response = "";
                    //    break;

                    //case "LIST":
                    //    response = List();
                    //    break;

                    //case "TYPE":
                    //    response = "";
                    //    break;

                    case "GET":
                        response = "sending file..."; //Retrieve(_root + "test.txt");
                        break;

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

                    case "PORT":
                        response = Port(arguments);
                        break;

                    //case "CLOSE":
                    //    response = "";
                    //    break;

                    //case "REIN":
                    //    response = "";
                    //    break;

                    //case "REST":
                    //    response = "";
                    //    break;

                    case "TEST":
                        response = "Test check";
                        break;


                    default:
                        response = "502 Command not implemented";
                        break;
                }
            }

            return response;
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


        //private string Passive()
        //{
        //    _dataConnectionType = DataConnectionType.Passive;

        //    IPAddress localIp = ((IPEndPoint)_controlClient.Client.LocalEndPoint).Address;

        //    _passiveListener = new TcpListener(localIp, 0);
        //    _passiveListener.Start();

        //    IPEndPoint passiveListenerEndpoint = (IPEndPoint)_passiveListener.LocalEndpoint;

        //    byte[] address = passiveListenerEndpoint.Address.GetAddressBytes();
        //    short port = (short)passiveListenerEndpoint.Port;

        //    byte[] portArray = BitConverter.GetBytes(port);

        //    if (BitConverter.IsLittleEndian)
        //        Array.Reverse(portArray);

        //    return string.Format("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5})",
        //        address[0], address[1], address[2], address[3], portArray[0], portArray[1]);
        //}

        private string[] List()
        {
            String[] listing = Directory.GetFiles(Directory.GetCurrentDirectory());
            return listing;
        }


       private static long CopyStreamAscii(Stream input, Stream output, int bufferSize = 4096)
        {
            char[] buffer = new char[bufferSize];
            int count = 0;
            long total = 0;

            using (StreamReader rdr = new StreamReader(input))
            {
                using (StreamWriter wtr = new StreamWriter(output, Encoding.ASCII))
                {
                    while ((count = rdr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        wtr.Write(buffer, 0, count);
                        total += count;
                    }
                }
            }

            return total;
        }
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////


namespace FileSync
{
    public class CommandHandler
    {

        private enum DataConnectionType
        {
            Passive,
            Active,
        }

        private TcpClient _controlClient;
        private TcpClient _dataClient;
        private TcpListener _passiveListener;
        private string _root;

        private NetworkStream _controlStream;
        private NetworkStream _dataConnection;

        private string _currentDirectory;
        private IPEndPoint _dataEndpoint;
        private IPEndPoint _remoteEndPoint;
        private DataConnectionType _dataConnectionType;
        private StreamReader _controlReader;
        private StreamWriter _controlWriter;


        public CommandHandler(NetworkStream controlStream)
        {
            _controlStream = controlStream;
            _controlReader = new StreamReader(controlStream);
            _controlWriter = new StreamWriter(controlStream);
            _root = Config.clientDir; //@"D:\test\";//Directory.GetCurrentDirectory();
        }

        public async Task<string> getResponseAsync(string command)
        {
            string response = null;

            string[] commandCode = command.Split(' ');

            string cmd = commandCode[0].ToUpperInvariant();
            string arguments = command.Length > 1 ? command.Substring(commandCode[0].Length) : null;

            if (string.IsNullOrWhiteSpace(arguments))
                arguments = null;

            if (response == null)
            {
                switch (cmd)
                {
                    //case "OPEN":
                    //    response = "";
                    //    break;

                    //case "LS":
                    //    response = "";
                    //    break;

                    //case "LIST":
                    //    response = List();
                    //    break;

                    //case "TYPE":
                    //    response = "";
                    //    break;

                    case "GET":
                        response = "sending file..."; //Retrieve(_root + "test.txt");
                        break;

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

                    case "PORT":
                        response = Port(arguments);
                        break;

                    //case "CLOSE":
                    //    response = "";
                    //    break;

                    //case "REIN":
                    //    response = "";
                    //    break;

                    //case "REST":
                    //    response = "";
                    //    break;

                    case "TEST":
                        response = "Test check";
                        break;


                    default:
                        response = "502 Command not implemented";
                        break;
                }
            }

            return response;
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


        //private string Passive()
        //{
        //    _dataConnectionType = DataConnectionType.Passive;

        //    IPAddress localIp = ((IPEndPoint)_controlClient.Client.LocalEndPoint).Address;

        //    _passiveListener = new TcpListener(localIp, 0);
        //    _passiveListener.Start();

        //    IPEndPoint passiveListenerEndpoint = (IPEndPoint)_passiveListener.LocalEndpoint;

        //    byte[] address = passiveListenerEndpoint.Address.GetAddressBytes();
        //    short port = (short)passiveListenerEndpoint.Port;

        //    byte[] portArray = BitConverter.GetBytes(port);

        //    if (BitConverter.IsLittleEndian)
        //        Array.Reverse(portArray);

        //    return string.Format("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5})",
        //        address[0], address[1], address[2], address[3], portArray[0], portArray[1]);
        //}

        private string[] List()
        {
            String[] listing = Directory.GetFiles(Directory.GetCurrentDirectory());
            return listing;
        }


        private string Retrieve(string pathname)
        {
            //for testing purposes
            if (_dataEndpoint == null)
            {
                Port("127,0,0,1,9,41");
            }

            if (File.Exists(pathname))
            {
                _dataClient = new TcpClient();
                _dataClient.BeginConnect(_dataEndpoint.Address, _dataEndpoint.Port, DoRetrieve, pathname);

                return string.Format("150 Opening {0} mode data transfer for RETR", _dataConnectionType);
            }

            return "550 File Not Found";
        }


        private void DoRetrieve(IAsyncResult result)
        {

            _dataClient.EndConnect(result);

            string pathname = (string)result.AsyncState;

            using (NetworkStream dataStream = _dataClient.GetStream())
            {
                using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
                {
                    CopyStreamAscii(fs, dataStream);
                    _dataClient.Close();
                    _dataClient = null;
                    _controlWriter.WriteLine("226 Closing data connection, file transfer successful");
                    _controlWriter.Flush();
                }
            }
        }

        private static long CopyStreamAscii(Stream input, Stream output, int bufferSize = 4096)
        {
            char[] buffer = new char[bufferSize];
            int count = 0;
            long total = 0;

            using (StreamReader rdr = new StreamReader(input))
            {
                using (StreamWriter wtr = new StreamWriter(output, Encoding.ASCII))
                {
                    while ((count = rdr.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        wtr.Write(buffer, 0, count);
                        total += count;
                    }
                }
            }

            return total;
        }
    }
}

