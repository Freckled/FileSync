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
        SENDMESSAGE,
        NONE
    }

    /// <summary>
    /// Response class
    /// </summary>
    /// <returns></returns>
    public class Response
    {
        private string _message;
        private Enum _action;
        private string _fileName;
        private long _fileSize;
        private Dictionary<string, string> dirList;
        DateTime? _modDate;


        public Response(string message, Enum action, string fileName = null, long fileSize = 0, DateTime? modDate = null)
        {
            _message = message;
            _action = action;

            if (!String.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
                _fileSize = fileSize;
                _modDate = modDate;
            }
        }

        public Response(string message, Enum action, Dictionary<string, string> dirList)
        {
            _message = message;
            _action = action;
            this.dirList = dirList;
        }

        public string getResponseString()
        {
            return _message;
        }

        /// <summary>
        /// Runs the selected action
        /// </summary>
        /// <param name="endPoint">The remote IPEndPoint</param>
        /// <returns></returns>
        public void runAction(IPEndPoint endPoint)
        {
            switch (_action)
            {
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
                        FileHandler.GetFile(endPoint, _fileName, _fileSize, _modDate);
                    }
                    break;

                case ActionType.GETFILES:

                    if (endPoint != null)
                    {
                        FileHandler.GetFiles(endPoint, dirList);
                    }
                    break;

                case ActionType.SENDMESSAGE:

                    if (endPoint != null)
                    {
                        Connection con = new Connection(endPoint);
                        con.sendCommand(_message);
                    }
                    break;

                case ActionType.NONE:
                    break;

                default:
                    throw new NotImplementedException("Action type is not supported.");
                    break;

            }
        }
    }
}
