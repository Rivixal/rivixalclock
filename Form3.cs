using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    
    public partial class Form3 : Form
    {
        Form6 alarmform2;
        public string Label17TextFromForm1 { get; set; }

        private SoundPlayer soundPlayer;
        public Form3()
        {
            InitializeComponent();
            // this.mainForm = mainForm;
            timer1.Start();
            timer2.Start();

            int currentHour = DateTime.Now.Hour;

            if (currentHour >= 5 && currentHour < 12)
            {
                panel2.BackgroundImage = Properties.Resources.morning;
                pictureBox1.Image = Properties.Resources.morning;
                soundPlayer = new SoundPlayer(Properties.Resources.The_Secret_forest_Samsung);
            }
            else if (currentHour >= 12 && currentHour < 18)
            {
                panel2.BackgroundImage = Properties.Resources.day;
                pictureBox1.Image = Properties.Resources.day;
                soundPlayer = new SoundPlayer(Properties.Resources.ringtone2);
            }
            else if (currentHour >= 18 && currentHour < 21)
            {
                panel2.BackgroundImage = Properties.Resources.sunset;
                pictureBox1.Image = Properties.Resources.sunset;
                soundPlayer = new SoundPlayer(Properties.Resources.ringtone2);
            }
            else
            {
                panel2.BackgroundImage = Properties.Resources.night;
                pictureBox1.Image = Properties.Resources.night;
                soundPlayer = new SoundPlayer(Properties.Resources.ringtone2);
            }
        }

        public void SetLabelText(string AlarmText) {
            label3.Text = AlarmText;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            soundPlayer.PlayLooping();
        }


        private Stream GetResourceStream(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundPlayer.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("HH:mm");
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
        }

        private void updatetextnote()
        {
                 
        }

        private void timer2_Tick_1(object sender, EventArgs e)
        {

        }

        private void label3_TextChanged(object sender, EventArgs e)
        {
            SetLabelText(label3.Text);
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {

        }

        ///            alarmForm = new Form1();
        /// label3.Text = alarmForm.textBox1.Text;
    }
}
