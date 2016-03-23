using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    public class HttpException : Exception
    {
        public String Statuscode { get; set; }


        public HttpException(String statuscode, String message) : base(message) { Statuscode = statuscode; }

        public HttpException(String statuscode, String message, Exception innerException) : base(message, innerException) { Statuscode = statuscode; }
    }
}
