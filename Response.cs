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

    public enum ConnectorType
    {
        SERVER,
        CLIENT        
    }

    public class Response
    {
        private string _clientMessage;
        private string _serverMessage;
        private Enum _clientAction;
        private Enum _serverAction;
        private string _fileName;
        private long _fileSize;
        private Socket _socket;

        public Response(string clientMessage, string serverMessage, Enum clientAction, Enum serverAction, string fileName=null, long fileSize=0)
        {
            _clientMessage = clientMessage;
            _serverMessage = serverMessage;
            _clientAction = clientAction;
            _serverAction = serverAction;

            if (!String.IsNullOrEmpty(fileName))
            {
                _fileName = fileName;
                _fileSize = fileSize;
            }

        }

        public string getMessage(ConnectorType type=ConnectorType.CLIENT)
        {
            if (type == ConnectorType.SERVER)
            {
                return _serverMessage;
            }
            else
            {
                return _clientMessage;
            }
            
        }

        public void runAction(ConnectorType type = ConnectorType.CLIENT, IPEndPoint endPoint = null)
        {
            Enum _action = null;
            if (type == ConnectorType.SERVER)
            {
                _action = _serverAction;
            }
            else
            {
                _action = _clientAction;
            }

            switch (_action){
                case ActionType.DELETE:

                    break;

                case ActionType.SENDFILE:
                    if (endPoint != null)
                    {
                        FileHandler fhs = new FileHandler(endPoint);
                        fhs.SendFile(_fileName);
                    }
                    break;

                case ActionType.GETFILE:
                    if (endPoint != null)
                    {
                        FileHandler fhs = new FileHandler(endPoint);
                        fhs.GetFile(_fileName, _fileSize);
                    }
                    ////////////////////////////
                    break;

                default:
                    break;

            }
        }
    }
}
