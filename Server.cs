using System;
using System.Collections.Generic;
using System.IO;
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
        private void clientConnection(Socket controlSocket)
        {

            //Assign data socket
            Socket _dataSocket = Connection.createSocket();
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            //IPEndPoint ep = new IPEndPoint(_ipAdress, Config.dataPort);
            _dataSocket.Bind(ep);


            //TESTING synch files
            synchFiles(controlSocket, _dataSocket);


            Console.WriteLine(controlSocket.LocalEndPoint.ToString() + " is Connected to remote" + controlSocket.RemoteEndPoint.ToString());
            byte[] msg = null;
            //send code connection established
            //--?

            //ask for DIR List
            msg = Encoding.UTF8.GetBytes("DIR");// + Config.endTextChar);
            controlSocket.Send(msg);
            byte[] data = Connection.ReceiveAll(controlSocket);
            //check response code 
            //--?

            //compare DIR list to own
            //--?
            string[] dirList = (Encoding.UTF8.GetString(data)).Split("/n");
            foreach(string item in dirList) { 
                Console.WriteLine(item.Trim());
            }


            
            //let the client know where to connect to and be ready to accept connection. Cast localEndpoint to IPEndpoint to get port.
            msg = Encoding.UTF8.GetBytes("PORT " + ((IPEndPoint)_dataSocket.LocalEndPoint).Port);
            Console.WriteLine("PORT " + ((IPEndPoint)_dataSocket.LocalEndPoint).Port);
            controlSocket.Send(msg);

       
            //get new file(s)
            //put new file(s)
            FileHandler fh = new FileHandler();
            
            Thread t = ActionThread(() => {

                string filepath = "D:/Filesync/Server/Vesper.mkv";
                long filesize = (long)new FileInfo(filepath).Length;
                msg = Encoding.UTF8.GetBytes("PUT Vesper.mkv " + filesize);
                controlSocket.Send(msg);
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


        private void synchFiles(Socket controlSocket, Socket dataSocket)
        {

            //---synch--
            //get local DIR
            var LocalfileList = FileHelper.DictFilesWithDateTime(Config.rootDir);


            //get dir list from client
            string response =  Connection.sendCommand(controlSocket, "DIR");
            string[] files = response.Split(Config.linebreak);
            Dictionary<string, string> remoteFileList = new Dictionary<string, string>();

            foreach (string file in files)
                {
                    var fileSplit = file.Split(" ");
                    remoteFileList.Add(fileSplit[0], fileSplit[1] + " " + fileSplit[2]);
                }

            //compare dir list and find out what to get and what to put


            string[] getFiles = { "", "" }; //TODO change placeholder
            string[] putFiles = { "", "" }; //TODO change placeholder


            //setup data endpoint
            //listen on socket

            //get files
            //put files

            Socket _dataSocket = Connection.createSocket();
            IPEndPoint ep = new IPEndPoint(_ipAdress, 0);
            dataSocket.Bind(ep);
            Connection.sendCommand(controlSocket, "PORT" + " " + ((IPEndPoint)_dataSocket.LocalEndPoint).Port);

            //TODO check if we want to connect on PORT command or on GET command.
            Thread t = ActionThread(() =>
            {

                dataSocket.Listen();
                foreach (string file in getFiles)
                {
                    //openDataStream;
                    
                    //get fileheader
                    string fileHeader = Connection.sendCommand(controlSocket, "GET" + " " + file);
                    //parse fileheader
                    string filePath = fileHeader; //TODO change placeholder
                    int size = fileHeader.Length; //TODO change placeholder
                    FileHandler.receiveFile(dataSocket, filePath, size);


                }
                
                foreach (string file in putFiles)
                {
                    //create fileheader
                    string fileHeader = "";//Filehandlder.filehead(filepath)
                    string filePath = "";
                    //send fileheader over socket.
                    Connection.sendCommand(controlSocket, fileHeader);
                    Connection.sendCommand(controlSocket, "PUT" + " " + file);
                    FileHandler.SendFile(dataSocket, filePath);
                }

                dataSocket.Close();
            });

            //---synch--


        }

    }
}
