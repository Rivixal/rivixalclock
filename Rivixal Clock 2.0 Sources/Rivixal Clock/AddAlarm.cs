using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rivixal_Clock
{
    public partial class AddAlarm : Form
    {
        private string soundFilePath;
        private SoundPlayer soundPlayer;
        public AddAlarm()
        {
            InitializeComponent();
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "HH:mm";
            UpdateTimeFormat();


            string timeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;

            if (timeFormat.Contains("tt")) // Проверяем, содержит ли формат времени "AM" или "PM"
            {
                radioButton1.Checked = true; // Если содержит, устанавливаем 12H формат
                radioButton2.Checked = false;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = true; // Иначе устанавливаем 24H формат
            }
        }

        private void PlaySound()
        {
            try
            {
                soundPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSoundPlayer()
        {
            try
            {
                if (!string.IsNullOrEmpty(label6.Text))
                {
                    // Получаем поток данных из ресурсов проекта и передаем его в конструктор SoundPlayer
                    soundPlayer = new SoundPlayer(Properties.Resources.ResourceManager.GetStream(label6.Text));
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

        private bool IsValidTime(string time)
        {
            // Проверка корректности времени в форматах HH:mm и hh:mm tt
            DateTime dummyTime;
            return DateTime.TryParseExact(time, new[] { "HH:mm", "hh:mm tt" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dummyTime);
        }


        private void btnAddAlarm_Click(object sender, EventArgs e)
        {
            // Получаем экземпляр Form1 из открытых форм приложения
            Form1 form1 = Application.OpenForms.OfType<Form1>().FirstOrDefault();

            if (form1 != null)
            {
                // Получаем значение времени из элемента управления в Form2
                string time;
                if (radioButton1.Checked)
                {
                    time = dateTimePicker1.Value.ToString("hh:mm tt", CultureInfo.InvariantCulture); // Формат времени в 12-часовом формате
                }
                else
                {
                    time = dateTimePicker1.Value.ToString("HH:mm", CultureInfo.InvariantCulture); // Формат времени в 24-часовом формате
                }

                // Проверяем корректность времени
                if (IsValidTime(time))
                {
                    // Продолжаем сбор остальных данных
                    bool once = checkBoxOnce.Checked;
                    bool monday = checkBoxMonday.Checked;
                    bool tuesday = checkBoxTuesday.Checked;
                    bool wednesday = checkBoxWednesday.Checked;
                    bool thursday = checkBoxThursday.Checked;
                    bool friday = checkBoxFriday.Checked;
                    bool saturday = checkBoxSaturday.Checked;
                    bool sunday = checkBoxSunday.Checked;
                    string soundFile = label6.Text; // Теперь используем ComboBox для выбора звука
                    string alarmText = textBoxAlarmText.Text;

                    // Добавляем новую строку в dataGridView1 в Form1
                    form1.AddAlarm2(time, once, monday, tuesday, wednesday, thursday, friday, saturday, sunday, soundFile, alarmText);

                    // Закрываем Form2 после добавления будильника
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Некорректное время. Пожалуйста, введите время в формате HH:mm.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void UpdateTimeFormat()
        {
            if (radioButton1.Checked)
            {
                dateTimePicker1.CustomFormat = "hh:mm tt"; // 12-часовой формат
            }
            else if (radioButton2.Checked)
            {
                dateTimePicker1.CustomFormat = "HH:mm"; // 24-часовой формат
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeFormat();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeFormat();
        }

        private void checkBoxOnce_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOnce.Checked == true)
            {
                checkBoxMonday.Enabled = false;
                checkBoxTuesday.Enabled = false;
                checkBoxWednesday.Enabled = false;
                checkBoxThursday.Enabled = false;
                checkBoxFriday.Enabled = false;
                checkBoxSaturday.Enabled = false;
                checkBoxSunday.Enabled = false;
            }
            else
            {
                checkBoxMonday.Enabled = true;
                checkBoxTuesday.Enabled = true;
                checkBoxWednesday.Enabled = true;
                checkBoxThursday.Enabled = true;
                checkBoxFriday.Enabled = true;
                checkBoxSaturday.Enabled = true;
                checkBoxSunday.Enabled = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxOnce.Checked = true;
            checkBoxMonday.Checked = false;
            checkBoxTuesday.Checked = false;
            checkBoxWednesday.Checked = false;
            checkBoxThursday.Checked = false;
            checkBoxFriday.Checked = false;
            checkBoxSaturday.Checked = false;
            checkBoxSunday.Checked = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxOnce.Checked = false;
            checkBoxMonday.Checked = true;
            checkBoxTuesday.Checked = true;
            checkBoxWednesday.Checked = true;
            checkBoxThursday.Checked = true;
            checkBoxFriday.Checked = true;
            checkBoxSaturday.Checked = true;
            checkBoxSunday.Checked = true;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxOnce.Checked = false;
            checkBoxMonday.Checked = false;
            checkBoxTuesday.Checked = false;
            checkBoxWednesday.Checked = false;
            checkBoxThursday.Checked = false;
            checkBoxFriday.Checked = false;
            checkBoxSaturday.Checked = false;
            checkBoxSunday.Checked = false;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxOnce.Checked = false;
            checkBoxMonday.Checked = true;
            checkBoxTuesday.Checked = true;
            checkBoxWednesday.Checked = true;
            checkBoxThursday.Checked = true;
            checkBoxFriday.Checked = true;
            checkBoxSaturday.Checked = false;
            checkBoxSunday.Checked = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                ///Themes
                string ringtone = comboBox1.SelectedItem.ToString();
                switch (ringtone)
                {
                    case "Standard":
                        label6.Text = "Standard";
                        break;
                    case "Timer":
                        label6.Text = "Timer";
                        break;
                    case "Samsung default timer":
                        label6.Text = "Samsung-Default-Timer";
                        break;
                    case "Homecoming":
                        label6.Text = "Homecoming";
                        break;
                    case "Morning Glory":
                        label6.Text = "Morning-Glory";
                        break;
                    case "The Secret forest":
                        label6.Text = "The-Secret-forest-Samsung";
                        break;
                    case "Good Morning":
                        label6.Text = "Good-Morning";
                        break;
                    case "Morning Flower":
                        label6.Text = "Morning-Flower";
                        break;
                    case "Bridsong by the lake":
                        label6.Text = "Birdsong-by-the-lake";
                        break;
                    case "Fairy fountain":
                        label6.Text = "Fairy-fountain";
                        break;
                    case "Gentle spring rain":
                        label6.Text = "Gentle-spring-rain";
                        break;
                    case "Homecoming Metal Remix - by Scott Wyllie":
                        label6.Text = "Homecomingremixsound";
                        break;
                    case "Radial0s1-":
                        label6.Text = "Radial";
                        break;
                    default:
                        label6.Text = "Standard";
                        break;
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitializeSoundPlayer();
            PlaySound();
        }

        private void PlayStop() 
        {
            try
            {
                soundPlayer.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlayStop();
        }

        private void AddAlarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (soundPlayer != null && soundPlayer.IsLoadCompleted)
            {
                PlayStop();
            }
        }

        private void russian()
        {
            Text = "Добавить будильник";
            label1.Text = "Настроить будильник";
            radioButton1.Text = "12-часовой формат";
            radioButton2.Text = "24-часовой формат";
            label4.Text = "Внимание! Звук будильника будет воспроизводиться только если компьютер в активном режиме.";
            checkBoxOnce.Text = "Один раз";
            checkBoxMonday.Text = "Пн";
            checkBoxTuesday.Text = "Вт";
            checkBoxWednesday.Text = "Ср";
            checkBoxThursday.Text = "Чт";
            checkBoxFriday.Text = "Пт";
            checkBoxSaturday.Text = "Сб";
            checkBoxSunday.Text = "Вс";
            label5.Text = "Режим";
            radioButton3.Text = "Один раз";
            radioButton4.Text = "Ежедневно";
            radioButton5.Text = "С понедельника по пятницу";
            radioButton6.Text = "Кастомный";
            label2.Text = "Рингтон будильника";
            button1.Text = "Воспроизвести";
            button2.Text = "Остановить";
            label7.Text = "Рингтон:";
            label3.Text = "Описание";
            textBoxAlarmText.Text = "Доброе утро";
            btnAddAlarm.Text = "Добавить будильник";
        }

        private void AddAlarm_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "Standard";
            switch (Properties.Settings.Default.language)
            {
                case "Русский":
                    {
                        Text = "Добавить будильник";
                        label1.Text = "Настроить будильник";
                        radioButton1.Text = "12-часовой формат";
                        radioButton2.Text = "24-часовой формат";
                        label4.Text = "Внимание! Звук будильника будет воспроизводиться только если компьютер в активном режиме.";
                        checkBoxOnce.Text = "Один раз";
                        checkBoxMonday.Text = "Пн";
                        checkBoxTuesday.Text = "Вт";
                        checkBoxWednesday.Text = "Ср";
                        checkBoxThursday.Text = "Чт";
                        checkBoxFriday.Text = "Пт";
                        checkBoxSaturday.Text = "Сб";
                        checkBoxSunday.Text = "Вс";
                        label5.Text = "Режим";
                        radioButton3.Text = "Один раз";
                        radioButton4.Text = "Ежедневно";
                        radioButton5.Text = "С Пн по Пт";
                        radioButton6.Text = "Кастомный";
                        label2.Text = "Рингтон будильника";
                        button1.Text = "Воспроизвести";
                        button2.Text = "Остановить";
                        label7.Text = "Рингтон:";
                        label3.Text = "Описание";
                        textBoxAlarmText.Text = "Доброе утро";
                        btnAddAlarm.Text = "Добавить будильник";
                        break;
                    }
            }
        }
    }
}
