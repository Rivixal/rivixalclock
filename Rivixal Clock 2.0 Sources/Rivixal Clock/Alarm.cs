using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Rivixal_Clock
{
    public partial class Alarm : Form
    {
        private string soundFilePath;
        private SoundPlayer soundPlayer;
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint SetThreadExecutionState(uint esFlags);

        const uint ES_DISPLAY_REQUIRED = 0x00000002;
        public Alarm(string alarmTime, string soundFile, string alarmText)
        {
            InitializeComponent();
            string format = "HH:mm";

            if (!DateTimeFormatInfo.CurrentInfo.LongTimePattern.Contains("H"))
            {
                format = "hh:mm tt";
            }
            else 
            {
                format = "hh:mm";
            }

            label2.Text = DateTime.Now.ToString(format);
            label3.Text = $"{alarmText}";

            this.DoubleBuffered = true;
            soundFilePath = soundFile;


            if (soundFile == "Homecomingremixsound")
            {
                panel2.BackgroundImage = Properties.Resources.homecomingremix;
                pictureBox2.Image = Properties.Resources.homecomingremix;
            }
            if (soundFile == "Radial")
            {
                panel2.BackgroundImage = Properties.Resources.ost;
                pictureBox2.Image = Properties.Resources.ost;
            }
            else 
            {
                int currentHour = DateTime.Now.Hour;

                if (currentHour >= 5 && currentHour < 12)
                {
                    panel2.BackgroundImage = Properties.Resources.morning;
                    pictureBox2.Image = Properties.Resources.morning;
                }
                else if (currentHour >= 12 && currentHour < 18)
                {
                    panel2.BackgroundImage = Properties.Resources.day;
                    pictureBox2.Image = Properties.Resources.day;
                }
                else if (currentHour >= 18 && currentHour < 21)
                {
                    panel2.BackgroundImage = Properties.Resources.sunset;
                    pictureBox2.Image = Properties.Resources.sunset;
                }
                else
                {
                    panel2.BackgroundImage = Properties.Resources.night;
                    pictureBox2.Image = Properties.Resources.night;
                }
            }
            InitializeSoundPlayer();
            PlaySound();
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


        private void InitializeSoundPlayer()
        {
            try
            {
                if (!string.IsNullOrEmpty(soundFilePath))
                {
                    // Получаем поток данных из ресурсов проекта и передаем его в конструктор SoundPlayer
                    soundPlayer = new SoundPlayer(Properties.Resources.ResourceManager.GetStream(soundFilePath));
                }
                else
                {
                    soundPlayer = new SoundPlayer(Properties.Resources.Standard);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing sound player: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PlaySound()
        {
            try
            {
                soundPlayer.PlayLooping();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Alarm_Load(object sender, EventArgs e)
        {
            SetThreadExecutionState(ES_DISPLAY_REQUIRED);
            notifyIcon1.Visible = false;
            label1.Text = Properties.Settings.Default.language == "English" ? "Alarm" : "Будильник";
            button1.Text = Properties.Settings.Default.language == "English" ? "Stop signal" : "Остановить сигнал";
            button2.Text = Properties.Settings.Default.language == "English" ? "Pause (9 Minutes)" : "Пауза (9 Минут)";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Hide the form
            this.Hide();
            notifyIcon1.Visible = false;

            // Set the time for automatic reopening after 5 minutes
            DateTime reopenTime = DateTime.Now.AddMinutes(5);

            // Create a new timer to count down the time until reopening the form
            Timer reopenTimer = new Timer();
            reopenTimer.Interval = 1000 * 60 * 9; // Interval - 9 minutes in milliseconds
            reopenTimer.Tick += (obj, args) =>
            {
                // Stop the timer
                reopenTimer.Stop();

                // Show the form again
                this.Show();
                PlaySound();
            };

            // Start the timer to count down the time until reopening the form
            reopenTimer.Start();
            if (soundPlayer != null)
            {
                soundPlayer.Stop();
                soundPlayer.Dispose();
                notifyIcon1.Visible = true;
            }
        }


        private void Alarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (soundPlayer != null)
            {
                soundPlayer.Stop();
                soundPlayer.Dispose();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
            label2.Text = DateTime.Now.ToShortTimeString();
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias; // Включаем сглаживание

            int radius = Math.Min(pictureBox1.Width, pictureBox1.Height) / 2 - 10;
            int centerX = pictureBox1.Width / 2;
            int centerY = pictureBox1.Height / 2;

            // Рисуем циферблат
            g.FillEllipse(Brushes.White, centerX - radius, centerY - radius, 2 * radius, 2 * radius);
            g.DrawEllipse(Pens.Black, centerX - radius, centerY - radius, 2 * radius, 2 * radius);

            // Рисуем центральный круг
            int centralCircleRadius = 5;
            g.FillEllipse(Brushes.Black, centerX - centralCircleRadius, centerY - centralCircleRadius, 2 * centralCircleRadius, 2 * centralCircleRadius);

            // Рисуем цифры
            using (Font font = new Font("Segoe UI", 12))
            {
                for (int i = 1; i <= 12; i++)
                {
                    double angle = Math.PI / 6 * i;
                    int x = (int)(centerX + (radius - 20) * Math.Sin(angle));
                    int y = (int)(centerY - (radius - 20) * Math.Cos(angle));
                    g.DrawString(i.ToString(), font, Brushes.Black, x, y, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            }

            // Рисуем минутные деления
            for (int i = 0; i < 60; i++)
            {
                double angle = Math.PI / 30 * i;
                int startX = (int)(centerX + (radius - 10) * Math.Sin(angle));
                int startY = (int)(centerY - (radius - 10) * Math.Cos(angle));
                int endX = (int)(centerX + (radius - 5) * Math.Sin(angle));
                int endY = (int)(centerY - (radius - 5) * Math.Cos(angle));
                g.DrawLine(Pens.Black, startX, startY, endX, endY);
            }

            // Получаем текущее время
            DateTime now = DateTime.Now;

            // Рисуем часовую стрелку
            double hoursAngle = Math.PI / 6 * (now.Hour % 12) + Math.PI / 360 * now.Minute;
            DrawHand(g, centerX, centerY, radius / 2, hoursAngle, 4, Color.Black);

            // Рисуем минутную стрелку
            double minutesAngle = Math.PI / 30 * now.Minute + Math.PI / 1800 * now.Second;
            DrawHand(g, centerX, centerY, (int)(radius * 0.8), minutesAngle, 3, Color.Black);

            // Рисуем секундную стрелку
            double secondsAngle = Math.PI / 30 * now.Second;
            DrawHand(g, centerX, centerY, (int)(radius * 0.9), secondsAngle, 2, Color.DarkRed);

            // Рисуем центральный круг для секундной стрелки
            int centralCircleRadius2 = 10;
            int secondHandLength = (int)(radius * 1.0);
            int secondHandSecondPointX = (int)(centerX + (secondHandLength - centralCircleRadius2) * Math.Sin(secondsAngle));
            int secondHandSecondPointY = (int)(centerY - (secondHandLength - centralCircleRadius2) * Math.Cos(secondsAngle));
            g.FillEllipse(Brushes.Red, secondHandSecondPointX - centralCircleRadius2 / 2, secondHandSecondPointY - centralCircleRadius2 / 2, centralCircleRadius2, centralCircleRadius2);
        }

        private void DrawHand(Graphics g, int centerX, int centerY, int length, double angle, int thickness, Color color)
        {
            int x = (int)(centerX + length * Math.Sin(angle));
            int y = (int)(centerY - length * Math.Cos(angle));
            g.DrawLine(new Pen(color, thickness), centerX, centerY, x, y);
        }

        private void stopSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
