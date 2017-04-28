using System;

namespace OnyxCore.Server
{
    public class MessageBuilder : IDisposable
    {
        private string _message;

        public MessageBuilder()
        {
            _message = "";
        }

        public void Dispose()
        {
            _message = null;
        }

        public string Add(string message)
        {
            if (String.IsNullOrWhiteSpace(_message))
            {
                _message = message.TrimEnd();
            }
            else
            {
                _message = _message + Environment.NewLine + message.TrimEnd();
            }

            return _message;
        }

        public string New()
        {
            _message = "";
            return _message;
        }

        public string Value()
        {
            return _message;
        }
    }
}
