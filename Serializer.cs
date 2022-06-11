using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FileSync
{
    public static class Serializer
    {

        public static byte[] Serialize(Response response)
        {
            if (response == null)
            {
                return null;
            }
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(Response));
                serializer.WriteObject(ms, response);
                return ms.ToArray();
            }

        }

        public static Response Deserialize(byte[] response)
        {
            if (response == null)
            {
                return default(Response);
            }
            using (var memStream = new MemoryStream(response))
            {
                var serializer = new DataContractSerializer(typeof(Response));
                var obj = (Response)serializer.ReadObject(memStream);
                return obj;
            }

        }


    }
}
