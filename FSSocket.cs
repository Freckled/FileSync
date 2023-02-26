﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public static class FSSocket
    {
        public static Socket Listen(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint IP = new IPEndPoint(Global.localIP, port);
            socket.Bind(IP);
            socket.Listen(10); //TODO adjust later
            Console.WriteLine("Socket started, waiting for a connection...");
            Console.WriteLine("Listening on : {0}", IP.ToString());
            Socket returnSocket = socket.Accept();
            socket.Close(); //TODO no need to close with multiple threads?
            return returnSocket;

        }

        public static Socket Connect(int port)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint IP = new IPEndPoint(Global.remoteIP, port);
            socket.Connect(IP);
            Console.WriteLine("Socket connected to {0}", socket.RemoteEndPoint.ToString());
            return socket;
        }

    }
}
