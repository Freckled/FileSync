using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    static class Transformer
    {

        public static int GetResponseCode(string str)
        {
            if (!str.Equals("") && str.Length >= 3){ 
                return int.Parse(str.Substring(0, 3));
            }
            return 0;
        }

        public static string RemoveResponseCode(string str)
        {
            return str.Substring(3).Trim();

        }

        public static string RemoveCommand(string str)
        {
            int commandEnd = str.IndexOf(" ", 0);
            return str.Substring(commandEnd).Trim();

        }

        public static string ParseByteArrString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        public static string parseDateToString(DateTime dateTime)
        {
            return dateTime.ToString(Config.dateTimeFormat, Config.cultureInfo);
        }

        public static DateTime parseStringToDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, Config.dateTimeFormat, Config.cultureInfo);
        }

    }
}
