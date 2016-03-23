using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    public class HttpResponseStatuscode
    {
        public const String Ok = "200 OK";
        public const String Bad_Request = "400 Bad Request";
        public const String Forbidden = "403 Forbidden";
        public const String Not_Found = "404 Not Found";
        public const String Method_Not_Allowed = "405 Method Not Allowed";
        public const String Request_Timeout = "406 Request Timeout";
        public const String Internal_Server_Error = "500 Internal Server Error";
    }
}
