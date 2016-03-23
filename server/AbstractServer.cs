using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver.Server
{
    public abstract class AbstractServer
    {
        public IPAddress ServerIpAdress { get; set; }
        public Int32 ServerPort { get; set; }
        public Int32 ReceiveBufferSize { get; set; }
        public Int32 SendBufferSize { get; set; }

        public abstract event EventHandler<DataContainer> OnDataReceived;
        public abstract event EventHandler<String> OnLogMessage;

        public void Init(String ipAdress, Int32 serverPort, Int32 receiveBufferSize, Int32 sendBufferSize)
        {
            ServerIpAdress = IPAddress.Parse(ipAdress);
            ServerPort = serverPort;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = sendBufferSize;
        }

        public abstract void Start();

        public abstract void Shutdown();

        public abstract void Send(DataContainer container, Byte[] data);
    }
}
