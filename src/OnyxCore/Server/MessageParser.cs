using OnyxCore.Dto;
using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace OnyxCore.Server
{
    public class MessageParser : IDisposable
    {
        public void Dispose()
        {

        }

        public ClientMessage ParseMessage(string message)
        {
            ClientMessage response = new ClientMessage();

            try
            {
                if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException("No message");

                if (message.StartsWith(@"<stream:stream to="))
                {
                    response.Protocol = ClientMessageProtocol.Xmpp;
                    response.MessageType = ClientMessageType.Initiate;
                }
                else if (message.Contains(@"</stream:stream>"))
                {
                    response.Protocol = ClientMessageProtocol.Xmpp;
                    response.MessageType = ClientMessageType.Terminate;
                }
                else
                {
                    response.Protocol = ClientMessageProtocol.Xmpp;

                    XDocument xdoc = XDocument.Load(new MemoryStream(Encoding.Unicode.GetBytes(@"<?xml version='1.0'?><root>" + (message.Replace(@"</stream:stream>", "") ?? message) + @"</root>")));
                    var name = ((XElement)((XElement)xdoc.FirstNode).FirstNode).Name.ToString().Replace("{urn:ietf:params:xml:ns:xmpp-tls}", "");

                    new Log().Add("Name: " + name);

                    switch (name)
                    {
                        case "starttls":
                            response.Protocol = ClientMessageProtocol.Xmpp;
                            response.MessageType = ClientMessageType.Authentication;

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                using (var log = new Log())
                {
                    log.Error(new string[] { "Error parsing text to XML:", message, "Error: " + ex.ToString() });
                }

                response = null;
            }
            
            return response;
        }
    }
}
