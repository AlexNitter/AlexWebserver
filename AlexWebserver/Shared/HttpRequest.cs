using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    public class HttpRequest : HttpMessage
    {
        /// <summary>Die Methode der Anfrage (GET, POST, ...)</summary>
        public HttpMethod Method { get; set; }

        /// <summary>Die angefragte Datei ("MeineWebanwendung/index.html")</summary>
        public String RequestedFile { get; set; }

        /// <summary>Die HTTP-Version der Anfrage (HTTP/1.1, HTTP1.2)</summary>
        public String HttpVersion { get; set; }

        /// <summary>Der angefragte Host</summary>
        public String Host { get; set; }


        /// <summary>Erzeugt ein neues HttpRequest-Objekt aus einem String</summary>
        /// <exception cref="ArgumentNullException">Wird geworfen, wenn der übergene String leer ist</exception>
        /// <exception cref="ArgumentException">Wird geworfen, wenn der übergene String kein gültiger HTTP Request ist</exception>
        public static HttpRequest CreateByMessageString(String messageString)
        {
            var entity = new HttpRequest();

            entity.parse(messageString);

            var startlineParts = entity._startline.Split(new Char[] { ' ' });

            if (startlineParts == null || startlineParts.Length != 3)
            {
                throw new ArgumentException("Der String ist keine gültige HTTP Message");
            }

            entity.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), startlineParts[0]);
            entity.RequestedFile = startlineParts[1];

            if (entity.RequestedFile.StartsWith("/"))
            {
                entity.RequestedFile = entity.RequestedFile.Remove(0, 1);
            }

            entity.HttpVersion = startlineParts[2];

            return entity;
        }

        /// <summary>Erzeugt ein neues HTTP-Request-Objekt anhand der übergebenen URL</summary>
        /// <exception cref="ArgumentException">Wirft geworfen, wenn der übergebene Parameter keine gültige URL ist</exception>
        public static HttpRequest CreateByUrl(String url)
        {
            Uri uri = null;

            try
            {
                uri = new Uri(url);
            }
            catch (Exception)
            {
                throw new ArgumentException("Der angegebene Wert ist keine gültige URL");
            }

            return new HttpRequest()
            {
                Host = uri.Host,
                HttpVersion = "HTTP/1.1",
                Method = HttpMethod.GET,
                RequestedFile = url,
                Headers = new Dictionary<String, String>()
                {
                    {"User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38." },
                    {"Connection", "keep-alive" }
                }
            };
        }


        public override string ToString()
        {
            var content = new StringBuilder();
            content.AppendLine(Method + " " + RequestedFile + " " + HttpVersion);
            content.AppendLine(Host);

            foreach (var item in Headers)
            {
                content.AppendLine(item.Key + ": " + item.Value);
            }

            content.AppendLine("");
            content.AppendLine(MessageBody);

            return content.ToString();
        }
    }
}
