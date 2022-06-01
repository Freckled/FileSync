using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace FileSync
{
    class ClientSocket
    {
        private IPAddress _serverIP;
        private int _serverPort;
        private int _listenerPort;
        private string _dataPortCommand;

        NetworkStream _stream;

        Socket _clientSocket;


        public ClientSocket(string serverIP = "127.0.0.1", int serverPort = 3456, int listenerPort = 2345)
        {
            _serverIP = IPAddress.Parse(serverIP);
            _serverPort = serverPort;
            _listenerPort = listenerPort;
        }

        public void connect()
        {

            try
            {
                IPAddress ipAddress = IPAddress.Parse("192.168.1.144");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2345);

                try
                {

                    // Connect to Remote EndPoint -- needs to be created each time (see error below)
                    Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                    _clientSocket = sender;
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                    // Release the socket.
                    //sender.Shutdown(SocketShutdown.Both);


                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public string sendCommand(string command)
        {
            string response = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            byte[] msg = Encoding.ASCII.GetBytes(command);

            // Send the data through the socket.
            _clientSocket.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = _clientSocket.Receive(bytes);
            response += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Text received : {0}", response);
            return response;
        }

    }
}

/*
System.InvalidOperationException: 'Once the socket has been disconnected, you can only reconnect 
again asynchronously, and only to a different EndPoint.  BeginConnect must be called on a thread that won't 
exit until the operation has been completed.'
*/