namespace OnyxCore
{
    public enum ClientMessageProtocol
    {
        Xmpp = 1
    }

    public enum ClientMessageType
    {
        Initiate = 1,
        Authentication = 2,
        Message = 3,
        Terminate = 4,
        Error = 5
    }
}
