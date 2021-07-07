using System;
using System.Threading;

namespace Server
{
    
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "127.0.0.1";
            int port = 8910;

            Server myserver = new Server(ip, port);
        }
    }
}
