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
        GETFILES,
        NONE
    }

    public class Response
    {
        private string _message;
        private Enum _action;
        private string _fileName;
        private long _fileSize;
        private List<KeyValuePair<string, string>> dirList;


        public Response(string message, Enum action, string fileName = null, long fileSize = 0)
        {
            _message = message;
            _action = action;
            
            if (!String.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
                _fileSize = fileSize;
            }

        }

        public Response(string message, Enum action, List<KeyValuePair<string, string>> dirList)
        {
            _message = message;
            _action = action;
            this.dirList = dirList;
        }

        public string getResponseString()
        {
            return _message;
        }

        public void runAction(IPEndPoint endPoint = null)
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
                        FileHandler.GetFile(endPoint, _fileName, _fileSize);
                    }
                    break;


                case ActionType.GETFILES:

                    if (endPoint != null)
                    {
                        FileHandler.GetFiles(endPoint, dirList);
                    }
                    break;

                default:
                    break;

            }
        }
    }
}
