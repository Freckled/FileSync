using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private string _message;
        private Enum _action;
        private string _fileName;
        private Socket _socket;

        public Response(string message, Enum action, string fileName = null)
        {
            _message = message;
            _action = action;

            if (!String.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
            }

        }
        public string getResponseString()
        {
            return _message;
        }

        public void runAction(IPEndPoint endPoint, long fileLength = 0)
        {
            switch (_action){
                case ActionType.DELETE:
                    break;

                case ActionType.SENDFILE:

                    if (endPoint != null)
                    {
                        FileHandler.SendFile(endPoint, _fileName);
                    }
                    break;

                case ActionType.GETFILE:

                    if (endPoint != null)
                    {
                        FileHandler.GetFile(endPoint, fileLength);
                    }
                    break;

                default:
                    break;

            }
        }
    }
}
