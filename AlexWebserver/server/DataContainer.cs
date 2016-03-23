using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver.Server
{
    public class DataContainer
    {
        public Socket ClientSocket { get; set; }
        public Byte[] ReceivedData { get; set; }
    }
}
