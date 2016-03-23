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
    class Program
    {
        private static Random _random = new Random();
        private static AbstractServer _server = null;
        private static WebManager _webManager = new WebManager();

        static void Main(string[] args)
        {
            printTitle();

            String serverIpAdress = null;
            Int32 serverPort = -1;
            
#if DEBUG
            serverIpAdress = "127.0.0.1";
            serverPort = 46580;
#else
            Console.Write("Bitte die IP-Adresse angeben, auf die der Server hören soll: ");
            serverIpAdress = Console.ReadLine();

            Console.Write("Bitte den Port angeben, auf den der Server hören soll: ");
            serverPort = Int32.Parse(Console.ReadLine());

            Console.WriteLine("");
#endif

            _server = new SequentialServer();

            _server.Init(serverIpAdress, serverPort, 50, 50);
            _server.OnLogMessage += (sender, message) => { log(message); };
            _server.OnDataReceived += Server_OnDataReceived;
            _server.Start();

            //var server = new ParallelServer("127.0.0.1", 46580, 50, 1024);
            //server.OnLogMessage += (sender, message) => { log(message); };
            //ParallelServer.OnDataReceived += Server_OnDataReceived;
            //server.Start();

            Console.Read();
        }

        private static void Server_OnDataReceived(object sender, DataContainer e)
        {
            var data = _webManager.HandleRequest(e.ReceivedData);
            _server.Send(e, data);
        }

        private static void printTitle()
        {
            Console.WriteLine("###############################");
            Console.WriteLine("## AlexWebserver - Webserver ##");
            Console.WriteLine("###############################\n");
            Console.WriteLine("Nimmt HTTP-Anfragen entgegen und beantwortet sie\n");
        }
        
        private static void log(String message)
        {
            Console.WriteLine(DateTime.Now.ToString("hh:MM:ss - ") + message);
        }
    }
}
