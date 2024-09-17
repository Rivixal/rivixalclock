using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    public partial class screensaver : Form
    {
        Form1 screensave;
        private Random random;
        private int panelSpeed;
        private int direction;
        private Point mouseLocation;

        public screensaver()
        {
            InitializeComponent();
            InitializeScreensaver();
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        private class NativeMethods
        {
            [Flags]
            public enum EXECUTION_STATE : uint
            {
                ES_AWAYMODE_REQUIRED = 0x00000040,
                ES_CONTINUOUS = 0x80000000,
                ES_DISPLAY_REQUIRED = 0x00000002,
                ES_SYSTEM_REQUIRED = 0x00000001
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
        }

        public screensaver(IntPtr previewWndHandle)
        {
            InitializeComponent();
            InitializeScreensaver();
            SetPreviewWindow(previewWndHandle);
        }

        private void InitializeScreensaver()
        {
            this.MouseMove += new MouseEventHandler(OnActivity);
            this.KeyPress += new KeyPressEventHandler(OnActivity);
            this.MouseClick += new MouseEventHandler(OnActivity);
            random = new Random();
            panelSpeed = 10; // Скорость движения панели
            direction = 1; // 1 - вниз, -1 - вверх
            Cursor.Hide();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
        }

        private void SetPreviewWindow(IntPtr previewWndHandle)
        {
            SetParent(this.Handle, previewWndHandle);
            RECT rect;
            GetClientRect(previewWndHandle, out rect);
            this.Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
            this.Location = new Point(0, 0);
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Show();
        }

        private void OnActivity(object sender, EventArgs e)
        {
            if (IsScreensaverMode())
            {
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                this.Close();
            }
        }

        private bool IsScreensaverMode()
        {
            // Определяет, запущено ли приложение как заставка
            string fileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();
            return fileName.EndsWith(".scr");
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!mouseLocation.IsEmpty && (Math.Abs(mouseLocation.X - e.X) > 5 || Math.Abs(mouseLocation.Y - e.Y) > 5))
            {
                OnActivity(this, null);
            }
            mouseLocation = e.Location;
            base.OnMouseMove(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToShortTimeString();
            label2.Text = "  " + DateTime.Now.ToString("dd/MM/yyyy" + " " + "ddd");
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            OnActivity(this, null);
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            OnActivity(this, null);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Проверка, достигла ли панель верхнего или нижнего края окна
            if (panel1.Top <= 0 || panel1.Bottom >= this.ClientSize.Height)
            {
                direction *= -1; // Изменение направления движения
                panel1.Top = Math.Max(0, Math.Min(panel1.Top, this.ClientSize.Height - panel1.Height)); // Корректировка позиции, чтобы не выходить за границы
            }

            // Случайная позиция по оси Y
            if (random.Next(0, 100) < 90) // 10% шанс изменить позицию
            {
                panel1.Top = random.Next(0, this.ClientSize.Height - panel1.Height);
            }
        }

        private void screensaver_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Show();

            // Остановка таймеров
            if (timer1 != null)
            {
                timer1.Stop();
                timer1.Dispose();
            }
            if (timer2 != null)
            {
                timer2.Stop();
                timer2.Dispose();
            }
        }

        private void tableLayoutPanel1_Click(object sender, EventArgs e)
        {
            OnActivity(this, null);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            screensave = (Form1)this.Owner;
            string alarmisnow = screensave.label6.Text;

            bool isVisible;
            if (bool.TryParse(alarmisnow, out isVisible))
            {
                pictureBox1.Visible = isVisible;
            }
            else
            {
                // Обработка случая, когда преобразование не удалось
                pictureBox1.Visible = false; // или любое другое логическое значение по умолчанию
            }
        }

    }
}
