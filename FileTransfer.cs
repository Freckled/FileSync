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

        public void sendFile2()
        {
            // Establish the local endpoint for the socket.
            //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = IPAddress.Parse("192.168.1.144");//ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            // Create a TCP socket.
            Socket client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint.
            client.Connect(ipEndPoint);

            // Send file fileName to the remote host with preBuffer and postBuffer data.
            // There is a text file test.txt located in the root directory.
            string fileName = "D:\\FileWatcher\\test.txt";

            // Create the preBuffer data.
            string string1 = String.Format("This is text data that precedes the file.{0}", Environment.NewLine);
            byte[] preBuf = Encoding.ASCII.GetBytes(string1);

            // Create the postBuffer data.
            string string2 = String.Format("This is text data that will follow the file.{0}", Environment.NewLine);
            byte[] postBuf = Encoding.ASCII.GetBytes(string2);

            //Send file fileName with buffers and default flags to the remote device.
            Console.WriteLine("Sending {0} with buffers to the host.{1}", fileName, Environment.NewLine);
            client.SendFile(fileName, preBuf, postBuf, TransmitFileOptions.UseDefaultWorkerThread);

            // Release the socket.
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }






















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

