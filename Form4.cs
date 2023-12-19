using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Reflection;

namespace Mclock
{

    public partial class Form4 : Form
    {
        private SoundPlayer soundPlayer;
        private Stopwatch stopwatch = new Stopwatch();
        public Form4()
        {
            InitializeComponent();
            timer1.Start();
            stopwatch.Start();
            soundPlayer = new SoundPlayer(Properties.Resources.alarmringtone);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = stopwatch.Elapsed;
            string formattedTime = String.Format("-{0:00}:{1:00}:{2:00}",
                Math.Abs(elapsed.Hours), Math.Abs(elapsed.Minutes), Math.Abs(elapsed.Seconds));

            label2.Text = formattedTime;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            soundPlayer.PlayLooping();
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundPlayer.Stop();
        }
    }
}
