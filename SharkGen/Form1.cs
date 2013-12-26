using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharkGen
{
    public partial class Form1 : Form,GameWindow
    {
        Game game1;
        bool AIGame;
        int difficulty;

        public Form1()
        {
            InitializeComponent();
            startGame(LevelData.randomLevel());
            timer1.Start();
        }
        public Form1(int difficulty)
        {
            this.difficulty = difficulty;
            AIGame = true;
            InitializeComponent();
            startGame(LevelData.randomLevel());
            timer1.Start();
        }

        void newGrid()
        {
            (new Generate()).ShowDialog();
            startGame(Generate.data);
        }
        void editGrid()
        {
            if (game1 != null)
            {
                (new Generate(game1)).ShowDialog();
                startGame(Generate.data);
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (game1 == null)
            {
                MessageBox.Show("No Game!");
                return;
            }
            SaveFileDialog filechooser = new SaveFileDialog();
            filechooser.InitialDirectory = "C:/Users/Philliip/Documents/SharksAndSquids";
            DialogResult result = filechooser.ShowDialog();
            string FileName;

            if (result == DialogResult.Cancel)
                return;

            FileName = filechooser.FileName;
            if (FileName == "" || FileName == null)
                MessageBox.Show("Invalid File Name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                try
                {
                    FileStream streamer = new FileStream(FileName, FileMode.OpenOrCreate,
                        FileAccess.Write);
                    StreamWriter output = new StreamWriter(streamer);

                    //saveToolStripMenuItem.Enabled = false;

                    IFormatter formatt = new BinaryFormatter();

                    SaveGame savy = new SaveGame(game1);
                    formatt.Serialize(streamer, savy);

                    output.Close();
                    streamer.Close();

                }
                catch (IOException)
                {
                    System.Console.Write("NONONONONONO");
                }
            }
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (game1 != null)
            {
                game1.draw(g);
            }
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
            return;
        }
        public void refreshGraphics()
        {
            panel1.Refresh();
        }
        void openGrid()
        {
            OpenFileDialog filechooser = new OpenFileDialog();
            filechooser.InitialDirectory = "C:/Users/Philliip/Documents/SharksAndSquids/Grids";
            DialogResult result = filechooser.ShowDialog();
            string FileName;

            if (result == DialogResult.Cancel)
                return;

            FileName = filechooser.FileName;
            if (FileName == "" || FileName == null)
                MessageBox.Show("Invalid File Name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                try
                {

                    FileStream streamer = new FileStream(FileName, FileMode.Open,
                        FileAccess.Read);

                    //saveToolStripMenuItem.Enabled = false;

                    IFormatter formatt = new BinaryFormatter();
                    LevelData data = (LevelData)formatt.Deserialize(streamer);

                    streamer.Close();
                    startGame(data);


                }
                catch (IOException)
                {
                    System.Console.Write("NONONONONONO");
                }
            }
        }
        void startGame(LevelData data)
        {
            if(AIGame)
                game1 = new Game(data, new Font(Font.FontFamily, 14), gameType.VsAI,  this);
            else
                game1 = new Game(data, new Font(Font.FontFamily, 14), gameType.Local, this);
            panel1.Refresh();
        }
        public gameMove getMove()
        {
            return null;
        }
        void reset()
        {
            if (game1 != null)
            {
                game1.reset();
                panel1.Refresh();
            }
        }

        private void openGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog filechooser = new OpenFileDialog();
            filechooser.InitialDirectory = "C:/Users/Philliip/Documents/SharksAndSquids";
            DialogResult result = filechooser.ShowDialog();
            string FileName;

            if (result == DialogResult.Cancel)
                return;

            FileName = filechooser.FileName;
            if (FileName == "" || FileName == null)
                MessageBox.Show("Invalid File Name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                try
                {

                    FileStream streamer = new FileStream(FileName, FileMode.Open,
                        FileAccess.Read);

                    //saveToolStripMenuItem.Enabled = false;

                    IFormatter formatt = new BinaryFormatter();
                    SaveGame savy = (SaveGame)formatt.Deserialize(streamer);
                    openSave(savy);
                }
                catch (IOException)
                {
                    System.Console.Write("NONONONONONO");
                }
            }
        }

        void openSave(SaveGame savy)
        {
            game1 = savy.game;
            panel1.Refresh();
        }

        private void editGridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            editGrid();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openGrid();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (gameActive && grid.undo())
            {
                switchTurn();
                panel1.Refresh();
            }
            */
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGrid();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            game1.periodicCall();
        }

        private void randomGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startGame(LevelData.randomLevel());
        }
        public int getDifficulty()
        {
            return difficulty;
        }

    }
}
