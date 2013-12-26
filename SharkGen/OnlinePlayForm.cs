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
    public partial class OnlinePlayForm : Form, GameWindow
    {
        BinaryWriter writer;
        BinaryReader reader;
        Game game1;
        bool server;
        bool moveReady;
        gameMove theMove;
        bool levelReady;
        LevelData recievedLevel;

        public OnlinePlayForm(BinaryWriter write, BinaryReader read, bool _server)
        {
            InitializeComponent();
            writer = write;
            reader = read;
            Thread listenThread = new Thread(new ThreadStart(otherThread));
            server = _server;
            if (server)
                startGame(LevelData.randomLevel());
            recievedLevel = null;
            levelReady = false;
            theMove = null;
            timer1.Start();
            listenThread.Start();
        }
        void startGame(LevelData theLevel)
        {
            bool worked = true;
            try
            {
                writer.Write("LVL" + theLevel.ToString());
            }
            catch (SocketException) { worked = false; }
            if (worked)
            {
                game1 = new Game(theLevel, new Font(Font.FontFamily, 14), gameType.Online, this);
                DisplayMessage("You have started a new game");
                refreshGraphics();
            }
        }
        void otherThread()
        {
            string message = "";
            do
            {
                try
                {
                    message = reader.ReadString();
                    if (message.Substring(0, 3) == "TXT")
                        DisplayMessage("Someone: " + message.Substring(3));
                    else if (message.Substring(0, 3) == "MOV")
                    {
                        theMove = gameMove.parseMove(message.Substring(3));
                        DisplayMessage("Opponent has made move");
                        moveReady = true;
                    }
                    else if (message.Substring(0, 3) == "LVL")
                    {
                            recievedLevel = LevelData.parseData(message.Substring(3));
                            levelReady = true;
                            DisplayMessage("A new game has been started");
                    }
                }
                catch (IOException)
                {
                    System.Environment.Exit(System.Environment.ExitCode);
                    
                }
            }
            while (message != "SERVER>>> TERMINATE");
            //while (true);
        }
        private delegate void ChatDisplayDelegate(string message);
        private void DisplayMessage(string message)
        {
            if (displayTextBox.InvokeRequired)
            {
                Invoke(new ChatDisplayDelegate(DisplayMessage), new object[] { message });
            }
            else
            {
                displayTextBox.Text += message + "\r\n";
                displayTextBox.SelectionStart = displayTextBox.Text.Length - 1;
                displayTextBox.ScrollToCaret();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendChat();
        }
        void sendChat()
        {
            DisplayMessage("Me: " + chatBox.Text);
            writer.Write("TXT" + chatBox.Text);
            chatBox.Text = "";
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (game1 != null)
            {
                game1.draw(g);
            }
        }
        public void refreshGraphics()
        {
            panel1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game1 != null)
                game1.periodicCall();
            
                if (levelReady)
                {
                    game1 = new Game(recievedLevel, new Font(Font.FontFamily, 14), gameType.Online, this);
                    levelReady = false;
                    panel1.Refresh();
                }

        }
        public gameMove getMove()
        {
            if (moveReady)
            {
                moveReady = false;
                gameMove move1 = null;
                lock (theMove)
                {
                    move1 = theMove.clone();
                }
                return move1;
            }
            else return null;
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (game1 != null)
            {
                game1.clickInput(e);
                panel1.Refresh();
            }
        }
        public void reportMove(gameMove move1)
        {
            writer.Write("MOV" + move1.ToString());
        }

        private void startNewRandomGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(LevelData.randomLevel());
        }

        private void startNewCustomGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new Generate()).ShowDialog();
            startGame(Generate.data);
        }
        public int getDifficulty() { return -1; }
    }
}
