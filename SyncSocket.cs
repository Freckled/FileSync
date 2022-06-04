using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace FileSync
{
    internal class SyncSocket
    {
        private Socket _socket;

        public void ServerStart(int port)
        {

            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPAddress ipAddress = (Dns.Resolve(IPAddress.Any.ToString())).AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            CommandHandler2 cmdHandler = new CommandHandler2();
            try
            {

                // Create a Socket that will use Tcp protocol
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                listener.Bind(localEndPoint);
                // Specify how many requests a Socket can listen before it gives Server busy response.
                // We will listen 10 requests at a time



                //create a loop so it keeps listening
                while (true)
                {

                    listener.Listen(10);

                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();

                    Console.WriteLine("Connected to " + handler.RemoteEndPoint.ToString());

                    // Incoming data from the client.
                    string data = null;
                    byte[] bytes = null;

                    bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    Console.WriteLine("Text received : {0}", data);
                    string response = cmdHandler.getResponse(data);

                    byte[] msg = Encoding.ASCII.GetBytes(response);
                    handler.Send(msg);

                }

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //Console.WriteLine("\n Press any key to continue...");
            //Console.ReadKey();

        }

        public void connectTo(string remoteIP, int remotePort)
        {

            try
            {
                IPAddress ipAddress = IPAddress.Parse(remoteIP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);
                
                try
                {

                    // Connect to Remote EndPoint -- needs to be created each time (see error below)
                    _socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(remoteEP);


                    Console.WriteLine("Socket connected to {0}",
                    _socket.RemoteEndPoint.ToString());

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
            _socket.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = _socket.Receive(bytes);
            response += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Text received : {0}", response);
            return response;
        }

        public void sendFile()
        {

        }

        public void getFile()
        {

            byte[] Rec_bytes = new byte[100];
            int messageLength = _socket.Receive(Rec_bytes);
            Console.WriteLine("Received...");
            for (int i = 0; i < messageLength; i++)
                Console.Write(Convert.ToChar(Rec_bytes[i]));
        }
                    

    }
}
