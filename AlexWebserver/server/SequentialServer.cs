using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver.Server
{
    public class SequentialServer : AbstractServer
    {
        public override event EventHandler<DataContainer> OnDataReceived;
        public override event EventHandler<String> OnLogMessage;

        private Socket _serverSocket = null;

        public override void Start()
        {
            IPEndPoint endpoint = null;

            try
            {
                endpoint = new IPEndPoint(ServerIpAdress, ServerPort);
                _serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                _serverSocket.Bind(endpoint);
                _serverSocket.Listen(5);

                OnLogMessage(this, "Server erfolgreich gestartet");
                receive();
            }
            catch (Exception)
            {
                if (_serverSocket != null)
                {
                    _serverSocket.Dispose();
                }
            }
        }

        private void receive()
        {
            OnLogMessage(this, "Warte auf Anfrage...");
            var clientSocket = _serverSocket.Accept();
            OnLogMessage(this, "Anfrage eingegangen");

            var receiveBuffer = new Byte[ReceiveBufferSize];

            var rceivedContent = new List<Byte>();
            var receivedAllBytes = false;

            while (!receivedAllBytes)
            {
                var receivedBytes = clientSocket.Receive(receiveBuffer, 0, ReceiveBufferSize, SocketFlags.None);

                if (receivedBytes > 0)
                {
                    rceivedContent.AddRange(receiveBuffer.ToList());
                }

                if (receivedBytes == ReceiveBufferSize)
                {
                    OnLogMessage(this, "Lese weiter");
                }
                else
                {
                    receivedAllBytes = true;
                    OnLogMessage(this, "Daten vollständig empfangen");
                }
            }

            var container = new DataContainer()
            {
                ClientSocket = clientSocket,
                ReceivedData = rceivedContent.ToArray()
            };

            OnDataReceived(this, container);
        }

        public override void Send(DataContainer container, Byte[] data)
        {
            container.ClientSocket.Send(data);
            OnLogMessage(this, "Daten verschickt");

            container.ClientSocket.Dispose();

            receive();
        }

        public override void Shutdown()
        {
            if (_serverSocket != null)
            {
                _serverSocket.Dispose();
            }

            OnLogMessage(this, "Server heruntergefahren");
        }
    }
}
