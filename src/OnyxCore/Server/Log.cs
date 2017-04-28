using System;

namespace OnyxCore.Server
{
    public class Log : IDisposable
    {
        // Planning on changing all the Console.WriteLine commands to log in database eventually.

        public Log()
        {

        }

        /// <summary>
        /// Dispose of current instance
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Add a new log entry
        /// </summary>
        /// <param name="message"></param>
        public void Add(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Logs multiple errors
        /// </summary>
        /// <param name="messages">Ex. new string[] { "param1", "param2" }</param>
        public void Error(string[] messages)
        {
            Console.WriteLine("Error(s):");

            foreach (var message in messages)
            {
                Console.WriteLine(message);
            }
        }

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(message);
        }

        /// <summary>
        /// Creates a new line in the console window. (Specifically used for console logging)
        /// </summary>
        public void NewLine()
        {
            Console.WriteLine(Environment.NewLine);
        }
    }
}
