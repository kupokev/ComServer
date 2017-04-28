using OnyxCore.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnyxCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new Listener())
            {
                bool stayOpen = true;

                do
                {
                    server.StartListener();
                    server.Listen();
                } while (stayOpen);
            }
        }
    }
}
