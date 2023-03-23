using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    internal class ResponseCode
    {
        private readonly static Dictionary<int, string> responseCodes = new Dictionary<int, string>()
        {
            { 125, "Data connection already open; transfer starting." },
            { 550, "Requested action not taken. File unavailable (e.g., file not found, no access)." },
            { 552, "Requested file action aborted. Exceeded storage allocation (for current directory or dataset)." },
            { 553, "Requested action not taken. File name not allowed." },
            { 502, "Command not implemented." },
            { 250, "Requested file action okay, completed." },
            { 225, "stop" }
        };

        /// <summary>
        /// IsValid validates wether the process can continue.
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns>Boolean indicating true when the process can continue; false when to stop.</returns>
        public static Boolean isValid(int _responseCode = 0)
        {
            Boolean ret = false;

            switch (_responseCode)
            {
                case 125:
                case 250:
                case 225:
                    ret = true;
                    break;
                case 550:
                case 552:
                case 553:
                case 502:
                    ret = false;
                    break;
                default:
                    return false;
            }

            return ret;
        }

        public static int getResponseCode(string _responseStr)
        {
            return ResponseCode.responseCodes.FirstOrDefault(x => x.Value == _responseStr).Key;
        }

        public static string getResponseText(int _responseCode)
        {
            return ResponseCode.responseCodes[_responseCode];
        }
    }
}
