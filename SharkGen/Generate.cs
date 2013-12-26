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
    public partial class Generate : Form
    {
        public static Grid grid;
        public static LevelData data;
        public Generate()
        {
            InitializeComponent();
            resetGrid();
        }
        public Generate(Game griddy)
        {
            InitializeComponent();
            openData(griddy.levelData);
        }

        void generateGrid()
        {
            data = new LevelData((int)numericUpDown1.Value, (int)numericUpDown2.Value, (int)numericUpDown3.Value, (int)numericUpDown4.Value, new List<Point>());
            grid = new Grid(data, new Point(0,0));
            panel1.Refresh();
        }
        void resetGrid()
        {
            data = new LevelData(3, 3, 2, 3, new List<Point>());
            grid = new Grid(data, new Point(0, 0));
            resetDials();
            panel1.Refresh();
        }
        void resetDials()
        {
            numericUpDown1.Value = data.width;
            numericUpDown2.Value = data.height;
            numericUpDown3.Value = data.chars;
            numericUpDown4.Value = data.rowLength;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = panel1.CreateGraphics();
            grid.drawGrid(g);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            generateGrid();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            data.width = (int)numericUpDown1.Value;
            panel1.Refresh();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            data.height = (int)numericUpDown2.Value;
            panel1.Refresh();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            data.chars = (int)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            data.rowLength = (int)numericUpDown4.Value;
        }
        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            grid.toggleBlocks(e.Location);
            panel1.Refresh();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog filechooser = new OpenFileDialog();
            filechooser.InitialDirectory = "C:/Users/Philliip/Documents/SharksAndSquids/Grids/";
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
                    LevelData dat = (LevelData)formatt.Deserialize(streamer);
                    openData(dat);
                    streamer.Close();

                }
                catch (IOException)
                {
                    System.Console.Write("NONONONONONO");
                }
            }
        }
        void openData(LevelData dat)
        {
            data = dat;
            grid = new Grid(data, new Point(0, 0));
            
            panel1.Refresh();
            resetDials();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog filechooser = new SaveFileDialog();
            filechooser.InitialDirectory = "C:/Users/Philliip/Documents/SharksAndSquids/Grids/";
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


                    formatt.Serialize(streamer, data);

                    output.Close();
                    streamer.Close();

                }
                catch (IOException)
                {
                    System.Console.Write("NONONONONONO");
                }
            }
        }
    }
}
