using AlexWebserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            printTitle();

            String url = null;

#if DEBUG
            url = "http://127.0.0.1:46580/WebApplication1/index.html";
#else
            Console.Write("URL im Format [http://{IP-Adresse}:{Port}/Dateipfad] angeben oder 'exit' zum Beenden. ");
            url = Console.ReadLine();
#endif

            while (!String.Equals(url, "exit", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    HttpRequest request = null;
                    Socket serverSocket = null;

                    request = HttpRequest.CreateByUrl(url);

                    serverSocket = openSocket(getIpAdressFromUrl(url), getPortFromUrl(url));

                    var response = sendRequest(request);

                    Console.WriteLine("So sieht die HTTP-Response aus:");
                    Console.WriteLine(response.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Es ist ein Fehler aufgetreten: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine("\n");
                    Console.Write("URL im Format [http://{IP-Adresse}:{Port}/Dateipfad] angeben oder 'exit' zum Beenden. ");
                    url = Console.ReadLine();
                }
            }
        }

        private static void printTitle()
        {
            Console.WriteLine("#####################################");
            Console.WriteLine("## AlexWebserver - Beispiel Client ##");
            Console.WriteLine("#####################################\n");
        }

        private static Socket openSocket(String ipAdress, Int32 port)
        {
            var serverIp = IPAddress.Parse(ipAdress);
            var serverPort = port;

            var serverEndpoint = new IPEndPoint(serverIp, serverPort);
            var serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Connect(serverEndpoint);

            return serverSocket;
        }

        private static String getIpAdressFromUrl(String url)
        {
            if (url.StartsWith("http://"))
            {
                url = url.Remove(0, 7);
            }
            else
            {
                throw new FormatException("Die URL hat nicht das Format [http://{IP-Adresse}:{Port}/Dateipfad]");
            }

            var urlParts = url.Split(':');

            return urlParts[0];
        }

        private static Int32 getPortFromUrl(String url)
        {
            if (url.StartsWith("http://"))
            {
                url = url.Remove(0, 7);
            }
            else
            {
                throw new FormatException("Die URL hat nicht das Format [http://{IP-Adresse}:{Port}/Dateipfad]");
            }

            var urlParts = url.Split(new Char[] { ':', '/' });

            if (urlParts.Length < 2)
            {
                throw new FormatException("Die URL hat nicht das Format [http://{IP-Adresse}:{Port}/Dateipfad]");
            }

            return Int32.Parse(urlParts[1]);
        }

        private static HttpResponse sendRequest(HttpRequest request)
        {
            Socket serverSocket = null;

            try
            {
                var response = new HttpResponse();

                #region create und connect to Socket
                var serverIp = IPAddress.Parse("127.0.0.1");
                var serverPort = 46580;

                var serverEndpoint = new IPEndPoint(serverIp, serverPort);
                serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                serverSocket.Connect(serverEndpoint);
                #endregion


                #region send data to socket
                var data = Encoding.Default.GetBytes(request.ToString());
                var bytesSent = serverSocket.Send(data);
                #endregion


                #region receive response from socket
                var receivedBytesBufferSize = 1024;
                Byte[] receivedBytesBuffer = null;

                var allBytesReceived = false;
                StringBuilder content = new StringBuilder();

                while (!allBytesReceived)
                {
                    receivedBytesBuffer = new byte[receivedBytesBufferSize]; // Buffer für das Empfangen der Daten

                    var receivedBytesCount = serverSocket.Receive(receivedBytesBuffer);
                    content.Append(Encoding.Default.GetString(receivedBytesBuffer));

                    if (receivedBytesCount < receivedBytesBufferSize)
                    {
                        // Es wurden definitiv alle Daten eingelesen
                        allBytesReceived = true;
                    }
                    else
                    {
                        // Ggf. wurden noch nicht alle Daten eingelesen
                        allBytesReceived = false;
                    }
                }
                #endregion


                response = HttpResponse.CreateByResponseString(Encoding.Default.GetString(receivedBytesBuffer));

                return response;
            }
            catch (Exception)
            {
                if (serverSocket != null)
                {
                    serverSocket.Dispose();
                }

                throw;
            }
        }
    }
}
