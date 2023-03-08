using System;
using System.Collections.Generic;
using System.IO;
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

        //start server
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

        //creates a new thread, puts the action in there and starts it.
        private Thread ActionThread(Action action)
        {
            Thread thread = new Thread(() => { action(); });
            thread.Start();
            return thread;
        }

        //handles connection with the client (todo, replace commandHandler part of communications)
        private void clientConnection(Socket socket)
        {
            Console.WriteLine(socket.LocalEndPoint.ToString() + " is Connected to remote" + socket.RemoteEndPoint.ToString());
            byte[] msg = null;
            //send code connection established
            //--?

            //ask for DIR List
            msg = Encoding.UTF8.GetBytes("\nDIR");
            socket.Send(msg);
            byte[] data = Connection.ReceiveAll2(socket);
            //check response code 
            //--?

            //compare DIR list to own
            //--?
            string[] dirList = (Encoding.UTF8.GetString(data)).Split("/n");
            foreach(string item in dirList) { 
                Console.WriteLine(item.Trim());
            }

            //Assign data socket
            Socket _dataSocket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            _dataSocket.Bind(ep);
            
            //let the client know where to connect to and be ready to accept connection. Cast localEndpoint to IPEndpoint to get port.
            msg = Encoding.UTF8.GetBytes("PORT " + ((IPEndPoint)_dataSocket.LocalEndPoint).Port);
            Console.WriteLine("PORT " + ((IPEndPoint)_dataSocket.LocalEndPoint).Port);
            socket.Send(msg);

       
            //get new file(s)
            //put new file(s)
            FileHandler fh = new FileHandler();
            Thread t = ActionThread(() => {

                string filepath = "C:/Filesync/Server/TestServer.txt";
                long filesize = (long)new FileInfo(filepath).Length;
                msg = Encoding.UTF8.GetBytes("PUT TestServer.txt " + filesize);
                socket.Send(msg);
                Console.WriteLine("Sending file");
                //dataSocket.SendFile(filepath);
                _dataSocket.Listen();
                Socket dataSocket = _dataSocket.Accept();
                FileHandler.SendFile(dataSocket, filepath);


                //for each loop through dir files
                //socket.send("get/put " + filename)
                //
                //end

                dataSocket.Close();
                //socket.close
            });
            
                     
        }

    }
}
