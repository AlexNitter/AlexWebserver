using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    public class HttpResponse : HttpMessage
    {
        /// <summary>Der Statuscode der Antwort</summary>
        public String Statuscode { get; set; }

        /// <summary>Die HTTP-Version der Antwort (HTTP/1.1, HTTP1.2)</summary>
        public String HttpVersion { get; set; }


        /// <summary>
        /// Erzeugt ein neues HttpResponse-Objekt aus einem String
        /// </summary>
        /// <exception cref="ArgumentNullException">Wird geworfen, wenn der übergene String leer ist</exception>
        /// <exception cref="ArgumentException">Wird geworfen, wenn der übergene String keine gültige HTTP Response ist</exception>
        public static HttpResponse CreateByResponseString(String messageString)
        {
            var entity = new HttpResponse();

            entity.parse(messageString);

            var startlineParts = entity._startline.Split(new Char[] { ' ' });

            if (startlineParts == null || startlineParts.Length != 3)
            {
                throw new ArgumentException("Der String ist keine gültige HTTP Message");
            }

            entity.HttpVersion = startlineParts[0];
            entity.Statuscode = startlineParts[1] + " " + startlineParts[2];

            return entity;
        }

        public override string ToString()
        {
            var content = new StringBuilder();
            content.AppendLine(HttpVersion + " " + Statuscode);

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
