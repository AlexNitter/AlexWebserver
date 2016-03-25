using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace AlexWebserver.Server
{
    /// <summary>Ist für die Verarbeitung eines Requests zuständig</summary>
    public class WebManager
    {
        private static String _webserverRootPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, "wwwroot");

        /// <summary>Nimmt einen Request als String entgegen, verarbeitet ihn und liefert eine Response als String zurück</summary>
        public Byte[] HandleRequest(Byte[] requestStream)
        {
            HttpResponse entity = new HttpResponse()
            {
                HttpVersion = "HTTP/1.1",
                Headers = getDefaultHeaders()
            };

            try
            {
                HttpRequest request = null;

                try
                {
                    request = HttpRequest.CreateByMessageString(Encoding.Default.GetString(requestStream));
                }
                catch (Exception ex)
                {
                    throw new HttpException(HttpResponseStatuscode.Bad_Request, "Das Format der Anfrage entspricht nicht der HTTP-Spezifikation");
                }


                if (request.Method != HttpMethod.GET)
                {
                    throw new HttpException(HttpResponseStatuscode.Method_Not_Allowed, "In der aktuellen Version ist lediglich die Methode \"GET\" erlaubt");
                }


                var requestedFile = Path.Combine(_webserverRootPath, request.RequestedFile);

                var info = new FileInfo(requestedFile);

                if (!info.Exists)
                {
                    throw new HttpException(HttpResponseStatuscode.Not_Found, "Die angeforderte Datei existiert nicht");
                }

                String contentType = String.Empty;
                switch (info.Extension)
                {
                    case ".htm":
                    case ".html":
                        contentType = "text/html";
                        entity.MessageBody = getMessageBodyByText(requestedFile);
                        break;
                    case ".jpg":
                        contentType = "image/jpg";
                        entity.MessageBody = getMessageBodyByImage(requestedFile);
                        break;
                    case ".jpeg":
                        contentType = "image/jpeg";
                        entity.MessageBody = getMessageBodyByImage(requestedFile);
                        break;
                    case ".gif":
                        contentType = "image/gif";
                        entity.MessageBody = getMessageBodyByImage(requestedFile);
                        break;
                    case ".png":
                        contentType = "image/png";
                        entity.MessageBody = getMessageBodyByImage(requestedFile);
                        break;
                    case ".js":
                        contentType = "application/javascript";
                        entity.MessageBody = getMessageBodyByText(requestedFile);
                        break;
                    case ".css":
                        contentType = "text/css";
                        entity.MessageBody = getMessageBodyByText(requestedFile);
                        break;
                    default:
                        throw new HttpException(HttpResponseStatuscode.Not_Found, "Die angeforderte Datei existiert nicht");
                }
                
                entity.Statuscode = HttpResponseStatuscode.Ok;
                entity.Headers.Add("Content-Length", entity.MessageBody.Length.ToString());

            }
            catch (Exception ex)
            {
                entity = getResponseByException(ex);
            }

            return Encoding.Default.GetBytes(entity.ToString());

            //return getDummyResponse();
        }

        private Dictionary<String, String> getDefaultHeaders()
        {
            return new Dictionary<String, String>()
            {
                { "Date", DateTime.Now.ToString("r") },
                { "Server", "AlexWebserver 1.0" },
                { "Connection", "Closed" }
            };
        }

        private HttpResponse getResponseByException(Exception ex)
        {
            var entity = new HttpResponse()
            {
                HttpVersion = "HTTP/1.1",
                Headers = getDefaultHeaders()
            };

            if (ex is HttpException)
            {
                var httpEx = (HttpException)ex;
                entity.Statuscode = httpEx.Statuscode;
            }
            else
            {
                entity.Statuscode = HttpResponseStatuscode.Internal_Server_Error;
            }

            entity.MessageBody = "<html><head><title>" + entity.Statuscode + "</title></head><body><h1>" + entity.Statuscode + "</h1></body></hhtml>";
            entity.Headers.Add("Content-Type", "text/html");
            entity.Headers.Add("Content-Length", entity.MessageBody.Length.ToString());

            return entity;
        }

        private String getMessageBodyByText(String filePath)
        {
            return File.ReadAllText(filePath);
        }

        private String getMessageBodyByImage(String filePath)
        {
            var image = Image.FromFile(filePath);
            var ms = new MemoryStream();
            image.Save(ms, image.RawFormat);
            return Encoding.Default.GetString(ms.ToArray());
        }

        private Byte[] getDummyResponse()
        {
            var content = "<html><body><h1>Hello, World!</h1></body></html>";

            var entity = new HttpResponse()
            {
                Statuscode = HttpResponseStatuscode.Ok,
                HttpVersion = "HTTP/1.1",
                Headers = new Dictionary<String, String>()
                {
                    {"Date", DateTime.Now.ToString("r") },
                    {"Server", "AlexWebserver" },
                    {"Content-Length", content.Length.ToString() },
                    {"Connection", "Closed" },
                    {"Content-Type", "text/html" }
                },
                MessageBody = content
            };

            var responseString = entity.ToString();

            return Encoding.Default.GetBytes(responseString);
        }
    }
}
