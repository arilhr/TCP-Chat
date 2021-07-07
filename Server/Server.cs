using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Server
    {
        String filePath = @"D:\chat-history.txt";
        List<String> chatHistory;
        TcpListener server = null;
        List<TcpClient> clients;
        bool isConnected = false;

        public Server(string ip, int port)
        {
            Console.WriteLine($"IP: {ip}");
            Console.WriteLine($"Port: {port}");

            // initialize attribute
            clients = new List<TcpClient>();
            chatHistory = new List<string>();

            // create new server with ip and port server
            StartServer(ip, port);

            if (Console.ReadLine() == "_STOP")
            {
                StopServer();
            }
        }

        public void StartServer(string ip, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);

            // start server
            server.Start();
            Console.WriteLine("Server Started...!");
            isConnected = true;

            // start server to accept client
            Thread acceptClient = new Thread(AcceptClient);
            acceptClient.Start();
        }

        public void AcceptClient()
        {
            try
            {
                while (true)
                {
                    // accept client
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client Connected!");

                    // make a thread to handle connected client
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Server Stopped");
                server.Stop();
            }
        }

        public void HandleClient(Object obj)
        {
            // add client to list client connected
            TcpClient client = (TcpClient) obj;
            clients.Add(client);

            var stream = client.GetStream();
            string data = null;
            Byte[] bytes = new Byte[1024];
            int i;
            try
            {
                // check sending data from client
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine($"{data}");

                    // add data chat to history
                    chatHistory.Add(data);

                    // broadcast message to all client except sender
                    BroadcastMessage(client, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
                clients.Remove(client);
                client.Close();
            }

            clients.Remove(client);
        }

        public void BroadcastMessage(Object obj, string message)
        {
            // send chat message to all client, except client sender
            foreach (TcpClient client in clients)
            {
                if (client != (TcpClient) obj)
                {
                    var stream = client.GetStream();
                    Byte[] reply = Encoding.ASCII.GetBytes(message);
                    stream.Write(reply, 0, reply.Length);
                }
            }
        }

        public void StopServer()
        {
            // write chat history
            File.WriteAllLines(filePath, chatHistory);

            // stop server
            server.Stop();
            isConnected = false;

            // disconnect all client
            foreach (TcpClient client in clients)
            {
                client.Close();
            }
            clients.Clear();
        }
    }
}
