using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSync
{
    public class Server
    {
        private Socket _socket;
        private IPAddress _ipAdress;
        private IPEndPoint _ep;
        
        public Server()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            _ipAdress = host.AddressList[0];
            _ep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
        }

        public void start()
        {
            reboot:
            try
            {
                _socket.Bind(_ep);
                //create a loop so it keeps listening
                while (true)
                {
                    Thread mainThread = Thread.CurrentThread;
                    
                    _socket.Listen(Config.serverPort);
                    Socket client = _socket.Accept();
                    
                    Thread t = ActionThread(() => {
                        clientConnection(client);
                    });

                    //t.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("restarting server...");
                Thread.Sleep(1000);
                goto reboot;
            }
        }

        private Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }


        private void clientConnection(Socket socket)
        {
            Console.WriteLine("Connected to " + socket.RemoteEndPoint.ToString());
            byte[] msg = null;
            //send code connection established
            //--?

            //ask for DIR List
            msg = Encoding.ASCII.GetBytes("DIR");
            socket.Send(msg);
            byte[] data = ReceiveAll(socket);
            //check response code 
            //--?

            //compare DIR list to own
            //--?

            //TODO issue, port 0 doesnt select a random port but sets it to 0.
            //open data socket
            Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            _dataSocket.Bind(ep);

            //let the client know where to connect to and be ready to accept connection
            msg = Encoding.ASCII.GetBytes("PORT " + ep.Port);
            socket.Send(msg);
            _dataSocket.Listen(ep.Port);
            Socket dataSocket = _dataSocket.Accept();

            //get new file(s)
            //put new file(s)
            Thread t = ActionThread(() => {
                //for each loop through dir files
                    //socket.send("get/put " + filename)
                    //
                //end

                //datasocket.close
                //socket.close
            });
            t.Start();
                     
        }

        private byte[] ReceiveAll(Socket socket)
        {
            var buffer = new List<byte>();

            //Do while zodat hij niet hangt op available. Blocked nu op receive en gaat dan door. 
            try { 
                do
                {
                    var currByte = new Byte[1];
                    var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

                    if (byteCounter.Equals(1))
                    {
                        buffer.Add(currByte[0]);
                    }
                }
                while (socket.Available > 0);
            }catch(SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            return buffer.ToArray();
        }



    }
}
