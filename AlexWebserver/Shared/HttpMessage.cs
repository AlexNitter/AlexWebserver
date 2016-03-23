using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexWebserver
{
    public abstract class HttpMessage
    {
        /*
         Die abstrakte HttpMessage-Klasse orientiert sich am RFC2616 Abschnitt 4 "HTTP Message"
        
         Grundlegender Aufbau einer HTTP Message:
         generic-message = start-line
                          *(message-header CRLF)
                          CRLF
                          [ message-body ]
        */

        /// <summary>Trennzeichen für Zeilenumbruch (carriage return line feed)</summary>
        protected const String CRLF = "\r\n";
        protected const String HEADER_BODY_SEPARATOR = "\r\n\r\n";

        /// <summary>Erste Zeile der Message (Entweder Request-Line oder Status-Line)</summary>
        protected String _startline;

        /// <summary>0 bis n Zeilen zwischen der ersten Zeile (startline) und der Leerzeile, die den Headerbereich vom Bodybereich trennt</summary>
        protected String[] _headerlines;


        /// <summary>Headers als Dictionary geparst</summary>
        public Dictionary<String, String> Headers { get; set; }

        /// <summary>Eigentlicher Inhalt der Nachricht (Nutzdaten)</summary>
        public String MessageBody { get; set; }
        

        /// <summary>
        /// Parst einen String in eine HttpMessage
        /// </summary>
        /// <exception cref="ArgumentNullException">Wird geworfen, wenn der übergene String leer ist</exception>
        /// /// <exception cref="ArgumentException">Wird geworfen, wenn der übergene String keine gültige HTTP Message ist</exception>
        protected void parse(String httpMessageString)
        {
            if (String.IsNullOrEmpty(httpMessageString))
            {
                throw new ArgumentNullException("Der String darf nicht leer sein");
            }

            var messageParts = httpMessageString.Split(new String[] { HEADER_BODY_SEPARATOR }, StringSplitOptions.None);

            if (messageParts == null || messageParts.Length == 0)
            {
                throw new ArgumentException("Der String ist keine gültige HTTP Message");
            }

            var header = messageParts[0];

            if (messageParts.Length > 1)
            {
                MessageBody = messageParts[1];
            }


            var headerParts = header.Split(new String[] { CRLF }, StringSplitOptions.None);

            if(headerParts == null || headerParts.Length == 0)
            {
                throw new ArgumentException("Der String ist keine gültige HTTP Message");
            }

            _startline = headerParts[0];

            if (headerParts.Length > 1)
            {
                Headers = new Dictionary<String, String>();

                for (int i = 1; i < headerParts.Length; i++)
                {
                    var line = headerParts[i];

                    var key = line.Substring(0, line.IndexOf(':'));
                    var value = line.Remove(0, line.IndexOf(": ")).Remove(0, 2);

                    Headers.Add(key, value);
                }
            }
        }
    }
}
