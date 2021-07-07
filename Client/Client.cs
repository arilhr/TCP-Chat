using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        String clientName;
        TcpClient client;
        NetworkStream stream;
        bool isConnected = false;

        public Client(String _clientName, String ip, Int32 port)
        {
            clientName = _clientName;

            try
            {
                // try connect to server
                client = new TcpClient(ip, port);
                stream = client.GetStream();
                isConnected = true;
                Console.WriteLine("Connect success");

                // send data thread
                Thread send = new Thread(SendMessage);
                send.Start();

                // read data thread
                Thread read = new Thread(ReadMessage);
                read.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed connect to server..");
                Console.WriteLine("Exception: {0}", e);
            }
        }

        private void SendMessage()
        {
            while (isConnected)
            {
                // user input data message
                String message = Console.ReadLine();
                String sendMsg = $"{clientName}: {message}";

                // Translate the Message into ASCII.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendMsg);

                // Send the message to the connected server. 
                stream.Write(data, 0, data.Length);
            }
        }

        private void ReadMessage()
        {
            string data = null;
            Byte[] bytes = new Byte[256];
            int i;

            try
            {
                // accept data
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    Console.WriteLine($"{data}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

        public void Disconnect()
        {

            client.Close();
        }
    }
}

