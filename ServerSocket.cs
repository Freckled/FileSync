using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;

namespace FileSync
{
    class ServerSocket
    {
        private Socket _client;

        public void start()
        {

            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPAddress ipAddress = (Dns.Resolve(IPAddress.Any.ToString())).AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 2345);
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
                while (true) { 
                
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

            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();

        }
    }
}

