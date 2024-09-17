using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    public partial class timeout : Form
    {
        private SoundPlayer soundPlayer;
        private Stopwatch stopwatch = new Stopwatch();
        public timeout()
        {
            InitializeComponent();
            timer1.Start();
            stopwatch.Start();
            soundPlayer = new SoundPlayer(Properties.Resources.Timer);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // WS_EX_COMPOSITED
                return cp;
            }
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

        private void timeout_Load(object sender, EventArgs e)
        {
            soundPlayer.PlayLooping();
            label1.Text = Properties.Settings.Default.language == "English" ? "Time out!" : "Время истекло!";
            button1.Text = Properties.Settings.Default.language == "English" ? "Stop signal" : "Остановить сигнал";
        }
        private void timeout_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundPlayer.Stop();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
