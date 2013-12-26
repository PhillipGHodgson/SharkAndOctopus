using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharkGen
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 localGame = new Form1();
            this.Hide();
            localGame.ShowDialog();
            this.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BeginNetwork networkSetup = new BeginNetwork();
            this.Hide();
            networkSetup.ShowDialog();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 localGame = new Form1((int)difficultySlider.Value);
            this.Hide();
            localGame.ShowDialog();
            this.Show();
        }
    }
}
