namespace OnyxCore.Server
{
    using Dto;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    // State object for reading client data asynchronously
    public class Listener : IDisposable
    {
        private TcpListener _listener;
        private IPAddress _serverIp;
        private int _serverPort;
        private int _bufferLength = 0;

        private bool accept = false;

        public Listener()
        {
            try
            {
                _serverIp = IPAddress.Parse("127.0.0.1");
                _serverPort = 5222;

                // 80 - Web
                // 5222 - XMPP
            }
            catch (FormatException fex)
            {
                // Log error: ipString is not a valid IP address.
            }
            catch (Exception e)
            {
                // Log error: e
            }
            finally
            {
                // Dispose of class.
                Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                _listener.Stop();
            }
            catch (Exception e)
            {

            }            
        }

        public void StartListener()
        {
            _listener = new TcpListener(_serverIp, _serverPort);
            _listener.Start();
            accept = true;

            new Log().Add($"OnyxCore Server started. Listening to TCP clients at {_serverIp}:{_serverPort}");
        }

        public void Listen()
        {

            using (var log = new Log())
            {
                try
                {
                    if (_listener != null && accept)
                    {
                        // Continue listening.  
                        while (true)
                        {
                            log.Add("Waiting for client...");
                            var clientTask = _listener.AcceptTcpClientAsync(); // Get the client  

                            if (clientTask.Result != null)
                            {
                                log.Add("Client connected. Waiting for data.");
                                var client = clientTask.Result;
                                string message = "";

                                try
                                {
                                    var user = new ClientUser();

                                    // To Do: Set variable to client user

                                    _bufferLength = client.ReceiveBufferSize;
                                    byte[] buffer = new byte[_bufferLength];

                                    int arrayLength = 0;
                                    bool resume = true;

                                    while (resume && client.Connected)
                                    {
                                        buffer = new byte[_bufferLength];
                                        client.Client.Receive(buffer);

                                        arrayLength = Array.IndexOf(buffer, (byte)0);
                                        arrayLength = arrayLength >= 0 ? arrayLength : buffer.Length;

                                        message = Encoding.ASCII.GetString(buffer, 0, arrayLength);

                                        if (!String.IsNullOrWhiteSpace(message))
                                        {
                                            log.NewLine();
                                            log.Add("=== Client Message ===");
                                            log.Add(message);
                                            log.Add("=== End Client Message ===");
                                            log.NewLine();

                                            using (var messageParser = new MessageParser())
                                            {
                                                var xmppMessage = messageParser.ParseMessage(message);

                                                if (xmppMessage != null)
                                                {
                                                    resume = HandleMessage(client, xmppMessage);
                                                }
                                                else
                                                {
                                                    resume = false;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var error = new ClientMessage()
                                    {
                                        Protocol = ClientMessageProtocol.Xmpp,
                                        MessageType = ClientMessageType.Error,
                                        Message = ex.Message
                                    };

                                    HandleMessage(client, error);
                                }                                

                                log.Add("Closing connection.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());

                    Dispose();
                }
            }
        }

        private bool HandleMessage(TcpClient client, ClientMessage message)
        {
            bool response = false;

            switch(message.Protocol)
            {
                case ClientMessageProtocol.Xmpp:
                    response = new XmppHandler().Process(client, message);
                    break;
            }

            return response;
        }
    }
}
