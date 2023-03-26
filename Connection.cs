using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace FileSync
{
    public static class Connection
    {
        public static string sendCommand(Socket socket, string command)
        {
            string response = null;
            byte[] bytes = null;

            if (socket.Connected)
            {
                bytes = new byte[1024];
                byte[] msg = Encoding.UTF8.GetBytes(command + Config.endTextChar);

                // Send the data through the socket.
                socket.Send(msg);
                Console.WriteLine("Message to remote connection: {0}", command);
                // Receive the response from the remote device.
                try
                {
                    response = Transformer.ParseByteArrString(Connection.ReceiveAll(socket));
                    Console.WriteLine("Response received: {0}", response);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Socket {0} forcefully closed", socket.RemoteEndPoint.ToString());
                }               
            }
            else
            {
                Console.WriteLine("Socket {0} is not connected", socket.RemoteEndPoint.ToString());
            }
            return response;

        }


        public static void sendCommandNoReply(Socket socket, string command)
        {
            string response = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            byte[] msg = Encoding.UTF8.GetBytes(command + Config.endTextChar);

            // Send the data through the socket.
            socket.Send(msg);
            Console.WriteLine("Message to remote connection: {0}", command);

        }

        public static byte[] ReceiveAll(Socket socket)
        {

            var buffer = new List<byte>();           
            byte[] endTextChar = Encoding.UTF8.GetBytes(Config.endTextChar);
                                    
            try
            {
                var currByte = new Byte[1];
                while (socket.Connected)
                {
                    currByte = new Byte[1];
                    var byteCounter = socket.Receive(currByte, currByte.Length, SocketFlags.None);

                    if (currByte[0] == endTextChar[0])
                    {
                        return buffer.ToArray();
                    }

                    if (byteCounter.Equals(1))
                    {
                        buffer.Add(currByte[0]);
                    }                 
                }                
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Socket closed before <EOT>");
            return buffer.ToArray();
        }

        public static Socket createSocket()
        {
            Socket _socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            return _socket;
        }

    }
}
