using OnyxCore.Dto;
using System;
using System.Net.Sockets;
using System.Text;

namespace OnyxCore.Server
{
    public class XmppHandler
    {
        public bool Process(TcpClient client, ClientMessage message)
        {
            bool response = true;
            byte[] data;
            string streamResponse = "";

            try
            {
                using (var log = new Log())
                {
                    switch (message.MessageType)
                    {
                        case ClientMessageType.Initiate:
                            // Open Connection
                            log.NewLine();
                            log.Add(@"Opening Stream with client...");

                            using (var msgBuilder = new MessageBuilder())
                            {
                                var msg = @"<stream:stream from='accellacorp.com' ";
                                msg += @"id='" + 1000 + "' ";
                                if(!String.IsNullOrEmpty(message.UserName)) msg += @"to='" + message.UserName.Trim() + "' ";
                                msg += @"version='1.0' xml:lang='en' xmlns='jabber:client' xmlns:stream='http://etherx.jabber.org/streams'>";

                                msgBuilder.Add(@"<?xml version='1.0'?>");
                                msgBuilder.Add(msg);
                                
                                streamResponse = msgBuilder.Value();

                                log.Add(streamResponse);

                                data = Encoding.ASCII.GetBytes(streamResponse);
                                client.GetStream().Write(data, 0, data.Length);

                                // Start TLS
                                log.NewLine();
                                log.Add(@"Starting TLS...");
                                msgBuilder.New();

                                //msgBuilder.Add(@"<?xml version='1.0'?>");
                                msgBuilder.Add(@"   <stream:features>");
                                msgBuilder.Add(@"      <starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'>");
                                msgBuilder.Add(@"         <required/>");
                                msgBuilder.Add(@"      </starttls>");
                                //msgBuilder.Add(@"      <mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>");
                                //msgBuilder.Add(@"         <mechanism>DIGEST-MD5</mechanism>");
                                //msgBuilder.Add(@"         <mechanism>PLAIN</mechanism>");
                                //msgBuilder.Add(@"      </mechanisms>");
                                msgBuilder.Add(@"   </stream:features>");

                                streamResponse = msgBuilder.Value();

                                log.Add(streamResponse);

                                data = Encoding.ASCII.GetBytes(streamResponse);
                                client.GetStream().Write(data, 0, data.Length);
                            }

                            break;

                        case ClientMessageType.Authentication:

                            // Authenticated
                            log.NewLine();
                            log.Add(@"Authenticated. Proceeding...");

                            using (var msgBuilder = new MessageBuilder())
                            {
                                msgBuilder.Add(@"<proceed xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>");

                                streamResponse = msgBuilder.Value();

                                log.Add(streamResponse);

                                data = Encoding.ASCII.GetBytes(streamResponse);
                                client.GetStream().Write(data, 0, data.Length);

                                try
                                {
                                    using (var tlsManager = new TlsManager())
                                    {
                                        if (!tlsManager.Authenticate())
                                        {
                                            log.Error("TLS authentication failed...");
                                            response = false;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Server encountered a problem while authenticating TLS with client...");
                                    log.Error(ex.Message);
                                    response = false;
                                }
                            }

                            break;

                        case ClientMessageType.Message:

                            // Message Handling here...
                            log.NewLine();
                            log.Add(message.Message);

                            break;

                        case ClientMessageType.Terminate:

                            log.NewLine();
                            log.Add(@"Stream closing...");

                            response = false;

                            break;

                        case ClientMessageType.Error:

                            // An error has occured
                            log.NewLine();
                            log.Error(message.Message);

                            using (var msgBuilder = new MessageBuilder())
                            {
                                msgBuilder.Add(@"<?xml version='1.0'?>");
                                msgBuilder.Add(@"   <stream:error>");
                                msgBuilder.Add(@"       <xml-not-well-formed xmlns='urn:ietf:params:xml:ns:xmpp-streams'/>");
                                msgBuilder.Add(@"       <text xml:lang='en' xmlns='urn:ietf:params:xml:ns:xmpp-streams'>");
                                msgBuilder.Add(message.Message);
                                msgBuilder.Add(@"       </text>");
                                msgBuilder.Add(@"   <escape-your-data xmlns='application-ns'/>");
                                msgBuilder.Add(@"   </stream:error>");
                                msgBuilder.Add(@"</stream:stream>");

                                streamResponse = msgBuilder.Value();

                                log.Add(streamResponse);

                                data = Encoding.ASCII.GetBytes(streamResponse);
                                client.GetStream().Write(data, 0, data.Length);
                            }

                            break;
                    }
                    
                    log.Add("Success");
                }
            }
            catch (Exception ex)
            {
                new Log().Error(ex.Message);

                response = false;
            }

            return response;
        }
    }
}
