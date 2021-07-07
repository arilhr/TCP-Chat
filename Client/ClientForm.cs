using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace ClientForm
{
    public partial class ClientForm : Form
    {
        TcpClient client;
        NetworkStream stream;
        String readData;

        public ClientForm()
        {
            InitializeComponent();
        }

        void Connect(String ip, Int32 port)
        {
            try
            {
                // try connect to server
                client = new TcpClient(ip, port);
                stream = client.GetStream();

                // UI form updated
                connectBtn.Enabled = false;
                disconnectBtn.Enabled = true;
                sendBtn.Enabled = true;
                chatBox.Text += $"{Environment.NewLine}Client Connected";

                // read message thread
                Thread read = new Thread(GetDataFromServer);
                read.Start();
            }
            catch (Exception e)
            {
                chatBox.Text += $"{Environment.NewLine}Failed connect to server";
                Console.WriteLine("Exception: {0}", e);
            }
        }

        void Disconnect()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(Disconnect));
            }
            else
            {
                // disconnect from server
                stream.Close();
                client.Close();

                // UI Updated
                connectBtn.Enabled = true;
                disconnectBtn.Enabled = false;
                sendBtn.Enabled = false;
                chatBox.Text += $"{Environment.NewLine}Disconnected from server";
            }
        }

        void SendMessage(String message)
        {
            String sendMsg = $"{txtName.Text}: {message}";
            txtMsg.Text = "";

            // Translate the Message into ASCII.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(sendMsg);

            // Send the message to the connected server. 
            stream.Write(data, 0, data.Length);
            chatBox.Text += Environment.NewLine + $"{sendMsg}";
        }

        void GetDataFromServer()
        {
            string data = null;
            Byte[] bytes = new Byte[1024];
            int i;

            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    readData = data;

                    UpdateChatBox();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }

            // disconnected from server
            Disconnect();
        }

        private void UpdateChatBox()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateChatBox));
            }
            else
            {
                chatBox.Text += Environment.NewLine + readData;
            }
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            Int32 port = Int32.Parse(txtPort.Text);
            Connect(txtIP.Text, port);
        }

        private void disconnectBtn_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            SendMessage(txtMsg.Text);
        }
    }
}
