using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace SharkGen
{
    public partial class BeginNetwork : Form
    {
        public BeginNetwork()
        {
            InitializeComponent();
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            MyIPBox.Text = localIP;
            /*
            string text;
            using (WebClient client = new WebClient())
            {
                text = client.DownloadString("http://checkip.dyndns.org");
            }

            string[] a = text.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            MyIPBox.Text = a3[0];
             * */


        }
        public void RunServer()
        {
            TcpListener listener;
            int counter = 1;

            try
            {
                listener = new TcpListener(IPAddress.Any, 4000);

                listener.Start();
                while (true)
                {
                    DisplayMessage("Waiting for client to connect...\r\n");
                   Socket connection = listener.AcceptSocket();
                    NetworkStream socketStream = new NetworkStream(connection);

                    BinaryWriter writer = new BinaryWriter(socketStream);
                    BinaryReader reader = new BinaryReader(socketStream);

                    DisplayMessage("Connection " + counter + " recieved\r\n");

                    writer.Write(">>>Connection successful");
                    //sendDataToCreator();


                    OnlinePlayForm gameForm = new OnlinePlayForm(writer, reader, true);
                    //this.Hide();
                    gameForm.ShowDialog();

                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    counter++;

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
        public void runClient()
        {
            TcpClient client;
            try
            {
                client = new TcpClient(AddressFamily.InterNetwork);
                DisplayMessage("\r\nAttempting Connection to Server...\r\n");
                client.Connect(IPAdressTextBox.Text, 4000);

                NetworkStream output = client.GetStream();

                BinaryWriter writer = new BinaryWriter(output);
                BinaryReader reader = new BinaryReader(output);

                DisplayMessage("\r\nConnectionSuccessful\r\n");
                OnlinePlayForm gameForm = new OnlinePlayForm(writer, reader, false);
                //this.Hide();
                gameForm.ShowDialog();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString(), "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }
        void disableButtons()
        {
            //StartClientButton.Enabled = false;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }
        private void DisplayMessage(string message)
        {
            if (displayTextBox.InvokeRequired)
            {
                Invoke(new DisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {
                displayTextBox.Text += message;
                displayTextBox.SelectionStart = displayTextBox.Text.Length - 1;
                displayTextBox.ScrollToCaret();
            }
        }
        private delegate void DisplayDelegate(string message);

        private void startServerButton_Click(object sender, EventArgs e)
        {
            Thread readThread = new Thread(new ThreadStart(RunServer));
            readThread.Start();
            disableButtons();
        }

        private void StartClientButton_Click(object sender, EventArgs e)
        {
            Thread readThread = new Thread(new ThreadStart(runClient));
            readThread.Start();
            disableButtons();
        }
    }
}
