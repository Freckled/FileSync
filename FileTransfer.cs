using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public class FileTransfer
    {
        public void sendFile(string filePath, string IP, int port)
        {
            string ip = GetIP();
            IPAddress[] ipAddress = Dns.GetHostAddresses(IP);//Server
             //IPAddress[] ipAddress = Dns.GetHostAddresses("127.0.0.1");//Local
            IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], port);
            Socket socket = new Socket(SocketType.Stream, ProtocolType.IP);
            
            socket.Connect(ipEnd);
            socket.SendFile(filePath);
        }

        public void getFile(string IP, int port)
        {
            try
            {
                string ip = GetIP();               
                IPAddress[] ipAddress = Dns.GetHostAddresses(IP);//Server
                                                                              //IPAddress[] ipAddress = Dns.GetHostAddresses("127.0.0.1");//Local
                IPEndPoint ipEnd = new IPEndPoint(ipAddress[0], port);
                Socket clientSock = new Socket(SocketType.Stream, ProtocolType.IP);
                clientSock.Listen();

                byte[] clientData = new byte[1024 * 5000];
                string receivedPath = Config.clientDir;

                int receivedBytesLen = clientSock.Receive(clientData);

                int fileNameLen = BitConverter.ToInt32(clientData, 0);
                string fileName = Encoding.ASCII.GetString(clientData, 4, fileNameLen);

                Console.WriteLine("Client:{0} connected & File {1} started received.", clientSock.RemoteEndPoint, fileName);

                BinaryWriter bWrite = new BinaryWriter(File.Open(receivedPath + fileName, FileMode.Append)); ;
                bWrite.Write(clientData, 4 + fileNameLen, receivedBytesLen - 4 - fileNameLen);

                Console.WriteLine("File: {0} received & saved at path: {1}", fileName, receivedPath);

                bWrite.Close();
                clientSock.Close();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("File Sending fail." + ex.Message);
            }



        }



        public static string GetIP()
        {
            string name = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(name);
            IPAddress[] addr = entry.AddressList;
            if (addr[1].ToString().Split('.').Length == 4)
            {
                return addr[1].ToString();
            }
            return addr[2].ToString();
        }
    }
    }

