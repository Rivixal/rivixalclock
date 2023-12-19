using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mclock
{
    public partial class Form6 : Form
    {
        Form1 screensaver;
        public Form6()
        {
            InitializeComponent();
            timer1.Start();
            timer2.Start();
            timer3.Start();

            Form3 alarmform2 = new Form3();
            this.AddOwnedForm(alarmform2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Обновление времени на label1
            label1.Text = DateTime.Now.ToString("HH:mm:ss");

            // Обновление даты и дня недели на label2 и label3
            label2.Text = DateTime.Now.ToString("dd/MM/yyyy");
            label3.Text = DateTime.Now.ToString("dddd");
        }

        private void Form6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            this.Close();
            Cursor.Show();
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
            screensaver = (Form1)this.Owner;
            label4.Text = screensaver.label8.Text;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            screensaver = (Form1)this.Owner;
            label7.Text = screensaver.label17.Text;
        }

        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Show();
            screensaver = (Form1)this.Owner;
            this.screensaver.Opacity = 100;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            screensaver = (Form1)this.Owner;
            label9.Text = screensaver.label19.Text;
            label10.Text = screensaver.label20.Text;
        }
    }
}
