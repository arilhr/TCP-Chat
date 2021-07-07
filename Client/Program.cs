using System;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            String name;
            String ip;
            Int32 port;

            // user input ip and port
            Console.Write("Enter name: ");
            name = Console.ReadLine();
            Console.Write("Enter IP: ");
            ip = Console.ReadLine();
            Console.Write("Enter Port: ");
            port = Int32.Parse(Console.ReadLine());

            Client client = new Client(name, ip, port);
        }
    }

    
}
