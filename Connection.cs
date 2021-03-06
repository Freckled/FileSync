using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace FileSync
{
    public class Connection
    {
        private IPAddress ipAddress;
        private IPEndPoint remoteEndPoint;
        private IPEndPoint dataEndpoint;
        private IPEndPoint localEndPoint;

        public Connection(string remoteIP, int remotePort)
        {
            ipAddress = IPAddress.Parse(remoteIP);
            remoteEndPoint = new IPEndPoint(ipAddress, remotePort);
            dataEndpoint = new IPEndPoint(ipAddress, Config.dataPort);
            localEndPoint = new IPEndPoint(IPAddress.Parse(Config.serverIp), Config.serverPort);
        }

        public Connection(IPEndPoint endPoint)
        {
            remoteEndPoint = endPoint;
            ipAddress = endPoint.Address;
        }

        public void ServerStart()
        {
            //Start new command handler to handle incoming commands
            CommandHandler cmdHandler = new CommandHandler();

        reboot:
            try
            {
                // Create a Socket that will use Tcp protocol
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, (int)1);
                if (listener.LocalEndPoint == null)
                {
                    listener.Bind(localEndPoint);
                }
                //create a loop so it keeps listening
                while (true)
                {                             
                    listener.Listen(100);
                    Console.Clear();
                    Console.WriteLine("Server starterd, waiting for a connection...");
                    Console.WriteLine("Listening on :{0}", remoteEndPoint.ToString());
                    Socket clientSocket = listener.Accept();

                    //note the client IP
                    string clientIP = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();

                    Console.WriteLine("Connected to " + clientSocket.RemoteEndPoint.ToString());

                    // Incoming data from the client.
                    string data = null;
                    byte[] bytes = null;

                    bytes = new byte[1024];
                    int bytesRec = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    Console.WriteLine("Command received: {0}", data);
                    Response response = cmdHandler.getResponse(data);

                    byte[] msg = Encoding.ASCII.GetBytes(response.getResponseString());
                    clientSocket.Send(msg);

                    Console.WriteLine("Reply sent : {0}", response.getResponseString());
                    
                    response.runAction(dataEndpoint);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("restarting server...");
                goto reboot;
            }
        }

        public string sendCommand(string command)
        {
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, (int)1);
            socket.Connect(remoteEndPoint);
            Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());

            string response = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            byte[] msg = Encoding.ASCII.GetBytes(command);

            // Send the data through the socket.
            socket.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = socket.Receive(bytes);
            response += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Text received : {0}", response);
            return response;
        }

    }
}
