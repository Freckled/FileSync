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
            if (!str.Equals("")){ 
                return int.Parse(str.Substring(0, 3));
            }
            return 0;
        }

        public static string RemoveResponseCode(string str)
        {
            return str.Substring(3);

        }

        public static string ParseByteArrString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

    }
}
