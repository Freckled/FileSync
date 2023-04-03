using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
            _ipAdress = IPAddress.IPv6Any; //host.AddressList[0];
            _ep = new IPEndPoint(_ipAdress, Config.serverPort);
            _socket = Connection.createSocket();

        }

        //start server
        public void start()
        {
            reboot:
            try
            {
                //TODO error handling for if socket is in use?
                _socket.Bind(_ep);
                //create a loop so it keeps listening
                while (true)
                {
                    
                    Thread mainThread = Thread.CurrentThread;
                    
                    _socket.Listen(Config.serverPort);
                    Console.WriteLine("Listening on {0}", _socket.LocalEndPoint.ToString());
                    Socket client = _socket.Accept();
                    
                    Thread t = ActionThread(() => {

                        //--------------Move this----------------
                        Socket dataSocket = Connection.createSocket();
                        IPEndPoint DataEP = new IPEndPoint(_ipAdress, Config.dataPort);
                        dataSocket.Bind(DataEP);
                        dataSocket.Listen();
                        //--------------Move this----------------

                        Console.WriteLine("Listening for data connection on {0}", dataSocket.LocalEndPoint.ToString());
                        Socket _dataSocket = dataSocket.Accept();
                        
                        clientConnection(client, _dataSocket);
                        while (client.Connected){}
                    });
                    
                    //TODO make sure connection are shutdown when thread is done
                    if (!t.IsAlive){
                        Console.WriteLine("Thread died");
                    }

                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("Port in use, disable applications using port number {0}", _ep.Port.ToString());
                Program.restart();
                
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
        private void clientConnection(Socket controlSocket, Socket dataSocket)
        {
            Console.WriteLine("Client {0}. connected to {1}", controlSocket.RemoteEndPoint.ToString(), controlSocket.LocalEndPoint.ToString());
                                   
            try
            {
                CommandHandler commandHandler;
                commandHandler = new CommandHandler(controlSocket, dataSocket);
            
                Console.WriteLine("Waiting for command..");
                string command = Transformer.ParseByteArrString(Connection.ReceiveAll(controlSocket));


                commandHandler.processCommand(command, CommandHandler.Device.CLIENT);
                while (controlSocket.Connected){}

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                if (controlSocket.Connected)
                {
                    controlSocket.Shutdown(SocketShutdown.Both);
                    controlSocket.Close();
                    controlSocket.Dispose();
                }
                if (dataSocket.Connected)
                {
                    dataSocket.Shutdown(SocketShutdown.Both);
                    dataSocket.Close();
                    dataSocket.Dispose();
                }
            }
            finally
            {
                if (controlSocket.Connected)
                {
                    //Connection.sendCommand(controlSocket, "CLOSE" + Config.endTransmissionChar);
                    controlSocket.Shutdown(SocketShutdown.Both);
                    controlSocket.Close();
                    controlSocket.Dispose();
                }
                if (dataSocket.Connected)
                {
                    dataSocket.Shutdown(SocketShutdown.Both);
                    dataSocket.Close();
                    dataSocket.Dispose();
                }
            }
        } 
    }
}
