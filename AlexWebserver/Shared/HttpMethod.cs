using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    /// <summary>HTTP-Methoden entsprechend RFC 7231 Abs. 4 (Auszug)</summary>
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
