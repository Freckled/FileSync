using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{

    public enum ActionType
    {
        DELETE,
        SENDFILE,
        GETFILE,
        NONE
    }

    public class Response
    {

        private string _reponseString;
        private Enum _action;
        private string _fileName;
        private Socket _socket;

        public Response(string reponseString, Enum action, string fileName = null)
        {
            _reponseString = reponseString;
            _action = action;

            if (!String.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
            }

        }
        public string getResponseString()
        {
            return _reponseString;
        }


        public void runAction(SyncSocket socket = null)
        {
            switch (_action){
                case ActionType.DELETE:
                    //FileHandler fhs = new FileHandler();
                    //fileHandler.deleteFile(fileName);
                    break;

                case ActionType.SENDFILE:
                    //FileHandler fhs = new FileHandler(socket);
                    //fileHandler.sendFileAsync(fileName);

                    //////////TEMP//////////////
                    ///
                    if (socket != null)
                    {
                        SyncSocket fileSocketSend = new SyncSocket("192.168.1.144", 11305);
                        //fileSocketSend.sendFileAsync("D:\\FileWatcher\\test.txt");                        
                        fileSocketSend.sendFileAsync(_fileName);
                    }
                    ////////////////////////////
                    break;

                case ActionType.GETFILE:
                    //FileHandler fhg = new FileHandler(socket);
                    //fileHandler.getFileAsync(fileName);

                    //////////TEMP//////////////
                    ///
                    if (socket != null)
                    {
                        SyncSocket fileSocketSend = new SyncSocket("192.168.1.144", 11305);
                        fileSocketSend.getFileAsync();
                    }
                    ////////////////////////////
                    break;

                default:
                    break;

            }
        }
    }
}
