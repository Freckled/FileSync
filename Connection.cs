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
                
            bytes = new byte[1024];
            byte[] msg = Encoding.UTF8.GetBytes(command);

            // Send the data through the socket.
            socket.Send(msg);

            // Receive the response from the remote device.
            int bytesRec = socket.Receive(bytes);
            response += Encoding.UTF8.GetString(bytes, 0, bytesRec);
            Console.WriteLine("Text received : {0}", response);
            return response;
        }


        public static void sendCommandNoReply(Socket socket, string command)
        {
            string response = null;
            byte[] bytes = null;

            bytes = new byte[1024];
            byte[] msg = Encoding.UTF8.GetBytes(command);

            // Send the data through the socket.
            socket.Send(msg);

        }



        public static byte[] ReceiveAll2(Socket socket)
        {
            var buffer = new List<byte>();

            //Do while zodat hij niet hangt op available. Blocked nu op receive en gaat dan door. 
            try
            {
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
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            return buffer.ToArray();
        }


        public static byte[] ReceiveAll(Socket socket)
        {

            var buffer = new List<byte>();           
            byte[] endTextChar = Encoding.UTF8.GetBytes(Config.endTextChar);
            

            //Do while zodat hij niet hangt op available. Blocked nu op receive en gaat dan door. 
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
