using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace FileSync
{
    internal class SyncSocket
    {
        private Socket _socket;
        
        private IPAddress ipAddress;
        private IPEndPoint remoteEndPoint;

        public SyncSocket(string remoteIP, int remotePort)
        {
            ipAddress = IPAddress.Parse(remoteIP);
            remoteEndPoint = new IPEndPoint(ipAddress, remotePort);
        }

        public void ServerStart()
        {
            //Start new command handler to handle incoming commands
            CommandHandler2 cmdHandler = new CommandHandler2();
            try
            {

                // Create a Socket that will use Tcp protocol
                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                // A Socket must be associated with an endpoint using the Bind method
                listener.Bind(remoteEndPoint);

                //create a loop so it keeps listening
                while (true)
                {

                    listener.Listen(10);
                    Console.WriteLine("Waiting for a connection...");
                    Socket clientSocket = listener.Accept();

                    Console.WriteLine("Connected to " + clientSocket.RemoteEndPoint.ToString());

                    // Incoming data from the client.
                    string data = null;
                    byte[] bytes = null;

                    bytes = new byte[1024];
                    int bytesRec = clientSocket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                    Console.WriteLine("Text received : {0}", data);
                    string response = cmdHandler.getResponse(data);

                    byte[] msg = Encoding.ASCII.GetBytes(response);
                    clientSocket.Send(msg);

                    ///////////////////////////////////////
                    // test send action after---- -if works, make command handler give back the action todo then run that after
                    if (data.Contains("get"))
                    {
                        SyncSocket fileSocketSend = new SyncSocket("192.168.1.144", 11305);
                        fileSocketSend.sendFileAsync("D:\\FileWatcher\\test.txt");
                    }
                    ///////////////////////////////////////
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


        }

        public void connectToRemote()
        {              
                try
                {
                                       
                    // Connect to Remote EndPoint -- needs to be created each time (see error below)
                    _socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(remoteEndPoint);


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

        public string sendCommand(string command)
        {
            connectToRemote();
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

        public async Task<string> sendFileAsync(string fileLoc)
        {
            //string fileName = "D:\\FileWatcher\\test.txt";
            connectToRemote();
            // Create the preBuffer data.
            try
            {
                string string1 = String.Format("This is text data that precedes the file.{0}", Environment.NewLine);
                byte[] preBuf = Encoding.ASCII.GetBytes(string1);

                // Create the postBuffer data.
                string string2 = String.Format("This is text data that will follow the file.{0}", Environment.NewLine);
                byte[] postBuf = Encoding.ASCII.GetBytes(string2);

                //Send file fileName with buffers and default flags to the remote device.
                Console.WriteLine("Sending {0} with buffers to the host.{1}", fileLoc, Environment.NewLine);
                //_socket.SendFile(fileName, preBuf, postBuf, TransmitFileOptions.UseDefaultWorkerThread);
                _socket.SendFile(fileLoc);
                Console.WriteLine("File Transfer started");
                return "File transfer started";
            }
            catch(SocketException se)
            {
                return "File transfer failed";
            }

            // Release the socket.
            //_socket.Shutdown(SocketShutdown.Both);
            //_socket.Close();
        }

        public async Task<string> getFileAsync()
        {
            try
            {
                // Create a Socket that will use Tcp protocol
                _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(remoteEndPoint);
                _socket.Listen(10);

                Console.WriteLine("Waiting for filetransfer...");
                Socket _dataSocket = await _socket.AcceptAsync();

                ///receive file
                try
                {
                    using (NetworkStream networkStream = new NetworkStream(_dataSocket))
                    using (FileStream fileStream = File.Open("D:\\FileWatcherTo\\test.txt", FileMode.OpenOrCreate))
                    {
                        networkStream.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {

                }
                ///
                Console.WriteLine("transfer complete");
                return "Filetransfer complete";
            }
            catch (Exception e)
            {
                return "Error transferring files";
            }
        }           

    }
}
