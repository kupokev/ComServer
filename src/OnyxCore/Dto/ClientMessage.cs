namespace OnyxCore.Dto
{
    public class ClientMessage
    {
        public ClientMessage()
        {
            Protocol = ClientMessageProtocol.Xmpp;
        }

        public string Message { get; set; }

        public string UserName { get; set; }

        public string Domain { get; set; }

        public string Device { get; set; }


        public ClientMessageProtocol Protocol { get; set; }

        public ClientMessageType MessageType { get; set; }
    }
}
