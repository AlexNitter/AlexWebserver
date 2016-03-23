using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AlexWebserver.Server
{
    /// <summary>Zuständig für die Netzwerk-Kommunikation</summary>
    public class ParallelServer
    {
        internal class ReceiveContainer
        {
            public Int32 BufferSize { get; set; }
            public Byte[] Buffer { get; set; }
            public StringBuilder Content { get; set; }
            public Socket ClientSocket { get; set; }
            

            public ReceiveContainer(Int32 bufferSize)
            {
                BufferSize = bufferSize;
                Buffer = new Byte[BufferSize];
                Content = new StringBuilder();
            }
        }

        public IPAddress ServerIpAdress { get; set; }
        public Int32 ServerPort { get; set; }
        public Int32 ReceiveBufferSize { get; set; }
        public Int32 SendBufferSize { get; set; }

        
        private static Random _random = new Random();
        public event EventHandler<String> OnLogMessage;
        public static event EventHandler<String> OnDataReceived;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public ParallelServer(String serverIp, Int32 serverPort, Int32 receiveBufferSize, Int32 sendBufferSize)
        {
            ServerIpAdress = IPAddress.Parse(serverIp);
            ServerPort = serverPort;
            ReceiveBufferSize = receiveBufferSize;
            SendBufferSize = SendBufferSize;
        }

        /// <summary>Öffnet den Socket und bearbeitet parallel Anfragen</summary>
        public void Start()
        {
            IPEndPoint endpoint = null;
            Socket serverSocket = null;

            try
            {
                endpoint = new IPEndPoint(ServerIpAdress, ServerPort);
                serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                serverSocket.Bind(endpoint);
                serverSocket.Listen(5);

                OnLogMessage(this, "Server erfolgreich gestartet, warte auf Anfragen...");

                Console.WriteLine();

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    serverSocket.BeginAccept(new AsyncCallback(BeginAcceptCallback), serverSocket);
                 
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                if (serverSocket != null)
                {
                    serverSocket.Dispose();
                }

                OnLogMessage(this, ex.Message);
            }
        }

        public static void BeginAcceptCallback(IAsyncResult result)
        {
            var container = new ReceiveContainer(50);

            Socket serverSocket = (Socket)result.AsyncState;
            container.ClientSocket = serverSocket.EndAccept(result);
            
            container.ClientSocket.BeginReceive(container.Buffer, 0, container.BufferSize, 0, new AsyncCallback(BeginReceiveCallback), container);

            var test = "";
        }

        public static void BeginReceiveCallback(IAsyncResult result)
        {
            var container = (ReceiveContainer)result.AsyncState;
            var clientSocket = container.ClientSocket;

            Int32 receivedBytesCount = clientSocket.EndReceive(result);

            if (receivedBytesCount > 0)
            {
                container.Content.Append(Encoding.ASCII.GetString(container.Buffer, 0, receivedBytesCount));
                container.ClientSocket.BeginReceive(container.Buffer, 0, container.BufferSize, 0, new AsyncCallback(BeginReceiveCallback), container);
            }
            else
            {
                if (container.Content.Length > 1)
                {
                    string content = container.Content.ToString();
                    Console.WriteLine("Read {0} bytes from socket.\n Data : {1}",
                       content.Length, content);

                    //OnDataReceived(null, container.Content.ToString());
                }

                clientSocket.Close();
            }
        }
    }
}
