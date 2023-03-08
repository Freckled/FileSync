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

        public Connection(IPAddress ipAddress, int remotePort)
        {
            remoteEndPoint = new IPEndPoint(ipAddress, remotePort);
            dataEndpoint = new IPEndPoint(ipAddress, Config.dataPort);
            //localEndPoint = new IPEndPoint(IPAddress.Parse(Config.serverIp), Config.serverPort); 
            localEndPoint = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), Config.serverPort);
        }

        public Connection(IPEndPoint endPoint)
        {
            remoteEndPoint = endPoint;
            ipAddress = endPoint.Address;
        }

        //public void ServerStart()
        //{
        //    //Start new command handler to handle incoming commands
        //    CommandHandler cmdHandler = new CommandHandler();
            
        //reboot:
        //    try
        //    {
                               
        //        //create a loop so it keeps listening
        //        while (true)
        //        {
        //            Thread mainThread = Thread.CurrentThread;
                                        
        //            Socket clientSocket = FSSocket.Listen(Config.serverPort);

        //            Thread t = ClientConnection(() => {
                       
        //                //-----------------------------------------------------------------------------------------
        //                //note the client IP
        //                string clientIP = ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString();

        //                Console.WriteLine("Connected to " + clientSocket.RemoteEndPoint.ToString());

        //                // Incoming data from the client.
        //                string data = null;
        //                byte[] bytes = null;

        //                bytes = new byte[1024];
        //                int bytesRec = clientSocket.Receive(bytes);
        //                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

        //                Console.WriteLine("Command received: {0}", data);
        //                Response response = cmdHandler.getResponse(data);

        //                byte[] msg = Encoding.ASCII.GetBytes(response.getResponseString());
        //                clientSocket.Send(msg);

        //                Console.WriteLine("Reply sent : {0}", response.getResponseString());
                        
        //                response.runAction(dataEndpoint);
        //                    //-----------------------------------------------------------------------------------------

        //                });

        //            t.Start();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        Console.WriteLine("restarting server...");
        //        Thread.Sleep(1000);
        //        goto reboot;
        //    }
        //}

        public static Thread ClientConnection(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }


        public string sendCommand(string command)
        {
            Socket socket = FSSocket.Connect(Config.serverPort);
            
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

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
