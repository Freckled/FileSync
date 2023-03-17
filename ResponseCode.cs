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
        //public static readonly 125 = 125;

        /// <summary>
        /// IsValid validates wether the process can continue.
        /// </summary>
        /// <param name="responseCode"></param>
        /// <returns>Boolean indicating true when the process can continue; false when to stop.</returns>
        public static Boolean isValid(int responseCode = 0)
        {
            return true;
        }
    }
}
