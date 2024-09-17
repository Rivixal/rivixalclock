using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Rivixal_Clock
{
    public partial class about : Form
    {
        public about()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            updater FormUpdater = new updater();
            FormUpdater.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/rivixal");
        }

        private void menuItem8_Click(object sender, EventArgs e)
        {

        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            Process.Start("https://t.me/rivixal_community");
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            Process.Start("https://youtube.com/@Rivixal");
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rivixal");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rivixal/rivixalclock");
        }

        private void about_Load(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.language)
            {
                case "Русский":
                    {
                        Text = "О программе Rivixal Clock";
                        menuItem1.Text = "Проверить обновления";
                        menuItem3.Text = "Ссылки";
                        menuItem2.Text = "Telegram канал";
                        menuItem4.Text = "Сообщество Rivixal";
                        menuItem5.Text = "Исходники";
                        menuItem6.Text = "Youtube канал";
                        menuItem7.Text = "Github профиль";
                        menuItem9.Text = "Благодарности";
                        menuItem10.Text = "Оцените проект!";
                        label7.Text = "\"Rivixal Clock 2.0\" — это не просто обновленная версия, это полное переосмысление идеи многофункциональных часов. В этой версии я учел все пожелания пользователей и внес множество улучшений.\r\n\r\nВ \"Rivixal Clock 2.0\" вы найдете все те же любимые функции: будильник, мировое время, секундомер и таймер. Но теперь они стали еще более удобными и функциональными.\r\n\r\nОсобое внимание было уделено интерфейсу. Я полностью переработал его, сделав еще более интуитивно понятным и удобным.\r\n\r\n\"Rivixal Clock 2.0\" — это ваш незаменимый помощник в организации времени. Это не просто часы, это инструмент для эффективного планирования вашего дня. Попробуйте, и убедитесь сами!";
                        break;
                    }
            }
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rivixal/rivixalclock/stargazers");
        }

        private void menuItem9_Click(object sender, EventArgs e)
        {
            Manythanks mnthanks = new Manythanks();
            mnthanks.ShowDialog();
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Rivixal/rivixalclock");
        }
    }
}
