using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Timers;
using System.Globalization;
using System.Threading;
using System.Diagnostics;
using NodaTime;
using NodaTime.Text;
using System.Drawing;
using System.Runtime.CompilerServices;
using Mclock.Properties;

namespace Mclock
{
    public partial class Form1 : Form
    {
        private Stopwatch stopwatch = new Stopwatch();
        // private bool alarmTriggered = false;
        // private bool isAlarmFormOpen = false;
        // private Form3 alarmForm;
        private Dictionary<string, Form3> activeAlarms = new Dictionary<string, Form3>();
        private bool isProcessingTimerTick = false;
        private System.Timers.Timer backgroundTimer;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int lapCount = 0;
        private bool isRunning = false;
        // private int millisecondsCounter = 0;
        private int totalTimeInSeconds;
        private int remainingTimeInSeconds;
        private bool isTimerRunning = false;

        Form5 savingchanges;
        public string SelectedTheme { get; set; }


        public Form1()
        {
            
            InitializeComponent();
            timer1.Interval = 1000; // 1 секунда
            timer1.Start();
            timer5.Interval = 10; // 1 миллисекунда
            timer7.Start();
            timer8.Start();
            timer9.Start();
            backgroundTimer = new System.Timers.Timer(10); // 1 минута
            backgroundTimer.Elapsed += BackgroundTimer_Elapsed;
            backgroundTimer.Start();
            timer6.Interval = 1000;
            timer9.Interval = 100;

            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);

            int currentHour = DateTime.Now.Hour;

            if (currentHour >= 5 && currentHour < 12)
            {
                panel4.BackgroundImage = Properties.Resources.morning;
            }
            else if (currentHour >= 12 && currentHour < 18)
            {
                panel4.BackgroundImage = Properties.Resources.day;
            }
            else if (currentHour >= 18 && currentHour < 21)
            {
                panel4.BackgroundImage = Properties.Resources.sunset;
            }
            else
            {
                panel4.BackgroundImage = Properties.Resources.night;
            }
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

        private void UpdateNextAlarmTimeLabel()
        {
            DateTime currentTime = DateTime.Now;
            DateTime? nextAlarmTime = null;

            foreach (string alarmTime in checkedListBox1.CheckedItems)
            {
                DateTime alarmDateTime = DateTime.ParseExact(alarmTime, "HH:mm", CultureInfo.InvariantCulture);

                if (alarmDateTime > currentTime && (!nextAlarmTime.HasValue || alarmDateTime < nextAlarmTime))
                {
                    nextAlarmTime = alarmDateTime;
                }
            }

            if (nextAlarmTime.HasValue)
            {
                string formattedTime = nextAlarmTime.Value.ToString("HH:mm");

                if (label8.InvokeRequired)
                {
                    label8.Invoke((MethodInvoker)delegate { label8.Text = formattedTime; });
                }
                else
                {
                    label8.Text = formattedTime;
                }
            }
            else
            {
                if (label8.InvokeRequired)
                {
                    label8.Invoke((MethodInvoker)delegate { label8.Text = "Ближающий сигнал не найден"; });
                }
                else
                {
                    label8.Text = "";
                }
            }


        }

        private void BackgroundTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isProcessingTimerTick) return;

            isProcessingTimerTick = true;

            string currentTime = DateTime.Now.ToString("HH:mm");

            foreach (string alarmTime in checkedListBox1.CheckedItems)
            {
                if (alarmTime == currentTime && !activeAlarms.ContainsKey(alarmTime))
                {
                    Form3 alarmform = new Form3();
                    alarmform.FormClosing += (s, args) => activeAlarms.Remove(alarmTime);
                    string AlarmText = label23.Text;
                    alarmform.SetLabelText(AlarmText);
                    alarmform.ShowDialog();
                    activeAlarms.Add(alarmTime, alarmform);
                    break;
                }
            }

            UpdateNextAlarmTimeLabel();

            isProcessingTimerTick = false;
        }



        private void button4_Click(object sender, EventArgs e)
        {
            string alarmTime = maskedTextBox1.Text;

            if (DateTime.TryParseExact(alarmTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedTime))
            {
                checkedListBox1.Items.Add(alarmTime, CheckState.Checked);
                maskedTextBox1.Clear();

                UpdateNextAlarmTimeLabel(); // Обновляем метку после добавления элемента

                if (!backgroundTimer.Enabled)
                {
                    backgroundTimer.Start();
                }
                panel6.Visible = false;
            }
            else
            {
                MessageBox.Show("Неправильное время. Введите время в формате чч:мм.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            string alarms = string.Empty;
            foreach (var item in checkedListBox1.Items)
            {
                alarms += item.ToString() + ",";
            }

            if (!string.IsNullOrEmpty(alarms))
            {
                alarms = alarms.Remove(alarms.Length - 1); // Удалить последнюю запятую
            }

            Properties.Settings.Default.listsAlarms = alarms;
            Properties.Settings.Default.Save();
            statusBar1.Text = label8.Text;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Обновление времени на label1
            label1.Text = DateTime.Now.ToString("HH:mm:ss");

            // Обновление даты и дня недели на label2 и label3
            label2.Text = DateTime.Now.ToString("dd/MM/yyyy");
            label3.Text = DateTime.Now.ToString("dddd");


            // Обновление прогрессбара секунд
            int seconds = DateTime.Now.Second;
            progressBar1.Value = seconds;
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            Form2 about = new Form2();
            about.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string AlarmText = label23.Text;
            Form3 alarmclock = new Form3();
            alarmclock.SetLabelText(AlarmText);
            alarmclock.ShowDialog();
            this.AddOwnedForm(alarmclock);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label11.Text = "";
            foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                string offset = GetUtcOffsetFormatted(timeZone.BaseUtcOffset);
                string timeZoneName = $"{timeZone.Id} (UTC {offset})";
                listBox1.Items.Add(timeZoneName);
                tabControl1.TabPages.Remove(tabPage5);
            }
            savingchanges = (Form5)this.Owner;
            SelectedTheme = Properties.Settings.Default.settings;
            string theme = Properties.Settings.Default.settings;
            switch (theme)
            {
                case "Светлая":
                    theme0();
                    break;
                case "Темная":
                    theme1();
                    break;
                default:
                    theme0();
                    break;
            }

            string alarms = Properties.Settings.Default.listsAlarms;

            // Проверьте, что строка alarms не пуста
            if (!string.IsNullOrEmpty(alarms))
            {
                // Разделите строку на элементы (предположим, что они разделены запятыми)
                string[] alarmArray = alarms.Split(',');

                // Теперь alarmArray содержит элементы, которые были сохранены

                // Можете использовать их, например, чтобы заполнить ваш CheckedListBox
                checkedListBox1.Items.AddRange(alarmArray);
            }
        }

        private string GetUtcOffsetFormatted(TimeSpan offset)
        {
            int hours = Math.Abs(offset.Hours);
            int minutes = Math.Abs(offset.Minutes);

            return $"{(offset.Hours >= 0 ? "+" : "-")}{hours:D2}:{minutes:D2}";
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            DateTime utcNow = DateTime.UtcNow;
            string selectedTimeZoneInfo = listBox1.SelectedItem?.ToString();
            string selectedTimeZoneInfo3 = listBox3.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedTimeZoneInfo))
            {
                string timeZoneId = selectedTimeZoneInfo.Substring(0, selectedTimeZoneInfo.IndexOf(" (UTC"));
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

                label12.Text = localTime.ToString("HH:mm:ss");
                progressBar2.Value = localTime.Second;
            }

            if (!string.IsNullOrEmpty(selectedTimeZoneInfo3))
            {
                string timeZoneId = selectedTimeZoneInfo3.Substring(0, selectedTimeZoneInfo3.IndexOf(" (UTC"));
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

                label12.Text = localTime.ToString("HH:mm:ss");
                progressBar2.Value = localTime.Second;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer4.Stop();
            timer4.Start();
        }

        private void UpdateTimerDisplay()
        {
            label5.Text = stopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss\.ff");
            label9.Text = $"Круг: {lapCount}"; // Отображение номера круга
            label19.Text = stopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss\.ff");
            label20.Text = $"Круг: {lapCount}";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            stopwatch.Start();
            isRunning = true;
            timer5.Start();
            button5.Enabled = false; // Отключаем кнопку Старт
            button6.Enabled = true;  // Включаем кнопку Пауза
            button7.Enabled = true;  // Включаем кнопку Круг
            button8.Enabled = true;  // Включаем кнопку Сброс
            progressBar3.Style = ProgressBarStyle.Marquee;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            stopwatch.Stop();
            isRunning = false;
            timer5.Stop();
            button5.Enabled = true;  // Включаем кнопку Старт
            button6.Enabled = false; // Отключаем кнопку Пауза
            progressBar3.Style = ProgressBarStyle.Blocks;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                lapCount++;
                listBox2.Items.Add($"Круг {lapCount}: {label5.Text}"); // Засечь время в круг
            }
            if (listBox2.Items.Count > 0)
            {
                listBox2.SelectedIndex = listBox2.Items.Count - 1;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            stopwatch.Reset();
            isRunning = false;
            timer5.Stop();
            elapsedTime = TimeSpan.Zero;
            lapCount = 0;
            UpdateTimerDisplay();
            listBox2.Items.Clear();
            button5.Enabled = true;  // Включаем кнопку Старт
            button6.Enabled = false; // Отключаем кнопку Пауза
            button7.Enabled = false; // Отключаем кнопку Круг
            button8.Enabled = false; // Отключаем кнопку Сброс
            progressBar3.Style = ProgressBarStyle.Blocks;
        }

    private void timer5_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                elapsedTime += TimeSpan.FromMilliseconds(timer5.Interval);
                UpdateTimerDisplay();
            }
        }

        public void updatetextnote()
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            updatetextnote();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            listBox4.Enabled = false;
            maskedTextBox2.Enabled = false;
            if (!isTimerRunning)
            {
                if (TryParseCustomTime(maskedTextBox2.Text, out totalTimeInSeconds))
                {
                    if (totalTimeInSeconds > 0 && totalTimeInSeconds <= 359999) // Максимальное значение: 99 часов 59 минут 59 секунд
                    {
                        remainingTimeInSeconds = totalTimeInSeconds;
                        isTimerRunning = true;
                        timer6.Start();
                        label11.Text = "Таймер установлен на " + maskedTextBox2.Text;
                        label17.Text = "Таймер: " + maskedTextBox2.Text;
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное время (макс: 99:59:59)");
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректное время в формате HH:mm:ss.");
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            label11.Text = "";
            timer6.Stop();
            isTimerRunning = false;
            remainingTimeInSeconds = totalTimeInSeconds; // Возвращаем оставшееся время к исходному
            progressBar4.Value = 0; // Сбрасываем прогресс
            UpdateTimerDisplay4();
            maskedTextBox2.Enabled = true;
            listBox4.Enabled = true;
            label17.Text = "Таймер: 00:00:00";
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (isTimerRunning)
            {
                timer6.Stop();
                isTimerRunning = false;
                button17.Text = "Восп.";
                label17.Text = "Таймер: Пауза " + maskedTextBox2.Text;
                button15.Enabled = false;
                button16.Enabled = false;
            }
            else
            {
                timer6.Start();
                isTimerRunning = true;
                button17.Text = "Пауза";
                label17.Text = "Таймер: " + maskedTextBox2.Text;
                button15.Enabled = true;
                button16.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 timeralarm = new Form4();
            timeralarm.ShowDialog();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            Form5 settings = new Form5();
            this.AddOwnedForm(settings);
            settings.ShowDialog();
        }


        private void timer6_Tick(object sender, EventArgs e)
        {
            if (isTimerRunning && remainingTimeInSeconds > 0)
            {
                remainingTimeInSeconds--;
                UpdateTimerDisplay4();
                double progress = 100.0 * remainingTimeInSeconds / totalTimeInSeconds;
                progressBar4.Value = (int)progress;
            }
            else
            {
                timer6.Stop();
                isTimerRunning = false;
                Form4 form4 = new Form4();
                form4.Show();
                maskedTextBox2.Enabled = true;
                listBox4.Enabled = true;
            }
        }

        private void UpdateTimerDisplay4()
        {
            int hours = remainingTimeInSeconds / 3600;
            int minutes = (remainingTimeInSeconds % 3600) / 60;
            int seconds = remainingTimeInSeconds % 60;
            label4.Text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        private bool TryParseCustomTime(string input, out int totalSeconds)
        {
            string[] parts = input.Split(':');
            if (parts.Length == 3 && int.TryParse(parts[0], out int hours) &&
                int.TryParse(parts[1], out int minutes) && int.TryParse(parts[2], out int seconds))
            {
                if (hours >= 0 && hours <= 99 && minutes >= 0 && minutes <= 59 && seconds >= 0 && seconds <= 59)
                {
                    totalSeconds = hours * 3600 + minutes * 60 + seconds;
                    return true;
                }
            }
            totalSeconds = 0;
            return false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.SelectedIndex >= 0) // Проверяем, есть ли выбранный элемент
            {
                checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
                UpdateNextAlarmTimeLabel(); // Обновляем метку после удаления элемента
            }
            else
            {
                MessageBox.Show("Выберите элемент для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            string alarms = string.Empty;
            foreach (var item in checkedListBox1.Items)
            {
                alarms += item.ToString() + ",";
            }

            if (!string.IsNullOrEmpty(alarms))
            {
                alarms = alarms.Remove(alarms.Length - 1); // Удалить последнюю запятую
            }

            Properties.Settings.Default.listsAlarms = alarms;
            Properties.Settings.Default.Save();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            timer4.Stop();
            timer4.Start();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Получаем выбранный элемент из listBox1
            object selectedItem = listBox1.SelectedItem;

            // Проверяем, что выбран элемент
            if (selectedItem != null)
            {
                // Добавляем выбранный элемент в listBox3
                listBox3.Items.Add(selectedItem);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (listBox3.SelectedIndex >= 0)
            {
                listBox3.Items.RemoveAt(listBox3.SelectedIndex);
            }
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            menuItem12.Visible = true;
        }

        private void tabPage6_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            menuItem12.Visible = false;
        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            menuItem12.Visible = false;
        }

        private void maskedTextBox2_TextChanged(object sender, EventArgs e)
        {
            label4.Text = maskedTextBox2.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;
            Form6 screensaver = new Form6();
            this.AddOwnedForm(screensaver);
            screensaver.ShowDialog();
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPage5);
            tabControl1.TabPages.Add(tabPage5);
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem != null)
            {
                maskedTextBox2.Text = listBox4.SelectedItem.ToString();
            }
            else
            {
                maskedTextBox2.Text = "00:00:00"; // Можете добавить здесь какое-либо значение по умолчанию
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (listBox4.SelectedItem != null)
            {
                listBox4.Items.Remove(listBox4.SelectedItem);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            listBox4.Items.Add(maskedTextBox2.Text);
        }

        private void button19_Click_1(object sender, EventArgs e)
        {
            if (listBox1.Visible == false)
            {
                listBox1.Visible = true;
            }
            else
            {
                listBox1.Visible = false;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            progressBar5.Style = ProgressBarStyle.Marquee;
        }

        private void button20_Click(object sender, EventArgs e)
        {
            progressBar5.Style = ProgressBarStyle.Blocks;
        }

        //Themes
        public void theme0()
        {
            tabPage1.BackColor = Color.White;
            tabPage2.BackColor = Color.White;
            tabPage3.BackColor = Color.White;
            tabPage4.BackColor = Color.White;
            tabPage5.BackColor = Color.White;
            // tabPage6.BackColor = Color.White;
            label4.ForeColor = Color.FromArgb(192, 0, 192);
            label5.ForeColor = Color.FromArgb(192, 0, 192);
            label6.ForeColor = Color.Black;
            label9.ForeColor = Color.FromArgb(192, 0, 192);
            label10.ForeColor = Color.Black;
            label11.ForeColor = Color.FromArgb(192, 0, 192);
            label12.ForeColor = Color.FromArgb(192, 0, 192);
            label14.ForeColor = Color.Black;
            label15.ForeColor = Color.FromArgb(192, 0, 192);
            label16.ForeColor = Color.FromArgb(192, 0, 192);
            label17.ForeColor = Color.FromArgb(192, 0, 192);
            label18.ForeColor = Color.FromArgb(192, 0, 192);
            label19.ForeColor = Color.FromArgb(192, 0, 192);
            label20.ForeColor = Color.FromArgb(192, 0, 192);
            button2.ForeColor = Color.Black;
            button3.ForeColor = Color.Black;
            button4.ForeColor = Color.Black;
            button9.ForeColor = Color.Black;
            button10.ForeColor = Color.Black;
            // button11.ForeColor = Color.Black;
            button12.ForeColor = Color.Black;
            button13.ForeColor = Color.Black;
            button14.ForeColor = Color.Black;
            button15.ForeColor = Color.Black;
            button16.ForeColor = Color.Black;
            button17.ForeColor = Color.Black;
            button18.ForeColor = Color.Black;
            button19.ForeColor = Color.Black;
            button20.ForeColor = Color.Black;
            button22.ForeColor = Color.Black;
            maskedTextBox1.ForeColor = Color.Black;
            maskedTextBox1.BackColor = Color.White;
            maskedTextBox2.ForeColor = Color.Black;
            maskedTextBox2.BackColor = Color.White;
            checkedListBox1.BackColor = Color.White;
            checkedListBox1.ForeColor = Color.Black;
            listBox1.BackColor = Color.White;
            listBox1.ForeColor = Color.Black;
            listBox2.BackColor = Color.White;
            listBox2.ForeColor = Color.Black;
            listBox3.BackColor = Color.White;
            listBox3.ForeColor = Color.Black;
            listBox4.BackColor = Color.White;
            listBox4.ForeColor = Color.Black;
            groupBox2.ForeColor = Color.Black;
        }

        public void theme1()
        {
            tabPage1.BackColor = Color.Black;
            tabPage2.BackColor = Color.Black;
            tabPage3.BackColor = Color.Black;
            tabPage4.BackColor = Color.Black;
            tabPage5.BackColor = Color.Black;
            // tabPage6.BackColor = Color.Black;
            label4.ForeColor = Color.FromArgb(255, 128, 255);
            label5.ForeColor = Color.FromArgb(255, 128, 255);
            label6.ForeColor = Color.White;
            label9.ForeColor = Color.FromArgb(255, 128, 255);
            label10.ForeColor = Color.White;
            label11.ForeColor = Color.FromArgb(255, 128, 255);
            label12.ForeColor = Color.FromArgb(255, 128, 255);
            label14.ForeColor = Color.White;
            label15.ForeColor = Color.FromArgb(255, 128, 255);
            label16.ForeColor = Color.FromArgb(255, 128, 255);
            label17.ForeColor = Color.FromArgb(255, 128, 255);
            label18.ForeColor = Color.FromArgb(255, 128, 255);
            label19.ForeColor = Color.FromArgb(255, 128, 255);
            label20.ForeColor = Color.FromArgb(255, 128, 255);
            button2.ForeColor = Color.White;
            button3.ForeColor = Color.White;
            button4.ForeColor = Color.White;
            button9.ForeColor = Color.White;
            button10.ForeColor = Color.White;
            // button11.ForeColor = Color.White;
            button12.ForeColor = Color.White;
            button13.ForeColor = Color.White;
            button14.ForeColor = Color.White;
            button15.ForeColor = Color.White;
            button16.ForeColor = Color.White;
            button17.ForeColor = Color.White;
            button18.ForeColor = Color.White;
            button19.ForeColor = Color.White;
            button20.ForeColor = Color.White;
            button22.ForeColor = Color.White;
            maskedTextBox1.ForeColor = Color.White;
            maskedTextBox1.BackColor = Color.Black;
            maskedTextBox2.ForeColor = Color.White;
            maskedTextBox2.BackColor = Color.Black;
            checkedListBox1.BackColor = Color.Black;
            checkedListBox1.ForeColor = Color.White;
            listBox1.BackColor = Color.Black;
            listBox1.ForeColor = Color.White;
            listBox2.BackColor = Color.Black;
            listBox2.ForeColor = Color.White;
            listBox3.BackColor = Color.Black;
            listBox3.ForeColor = Color.White;
            listBox4.BackColor = Color.Black;
            listBox4.ForeColor = Color.White;
            groupBox2.ForeColor = Color.White;
        }
        //End-themes
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            if (timer6.Enabled == true)
            {
                MessageBox.Show("Таймер все еще выполняется, приложение закрыть нельзя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            label17.Text =  label4.Text;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            string alarms = string.Empty;
            foreach (var item in checkedListBox1.Items)
            {
                alarms += item.ToString() + ",";
            }

            if (!string.IsNullOrEmpty(alarms))
            {
                alarms = alarms.Remove(alarms.Length - 1); // Удалить последнюю запятую
            }

            Properties.Settings.Default.listsAlarms = alarms;
            Properties.Settings.Default.Save();
        }

        private void menuItem7_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;
            Form6 screensaver = new Form6();
            this.AddOwnedForm(screensaver);
            screensaver.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string AlarmText = label23.Text;
            label23.Text = textBox1.Text;
        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            int currentHour = DateTime.Now.Hour;

            // Вычисляем время до ближайшего обновления
            int timeUntilNextUpdate = 0;

            if (currentHour < 5)
            {
                timeUntilNextUpdate = 5 - currentHour;
            }
            else if (currentHour < 12)
            {
                timeUntilNextUpdate = 12 - currentHour;
            }
            else if (currentHour < 18)
            {
                timeUntilNextUpdate = 18 - currentHour;
            }
            else if (currentHour < 21)
            {
                timeUntilNextUpdate = 21 - currentHour;
            }
            else
            {
                timeUntilNextUpdate = 29 - currentHour;
            }

            // Устанавливаем интервал таймера в миллисекундах до следующего обновления
            timer8.Interval = timeUntilNextUpdate * 3600000; // 1 час = 3600000 миллисекунд

            // Обновляем изображение
            if (currentHour >= 5 && currentHour < 12)
            {
                panel4.BackgroundImage = Properties.Resources.morning;
                pictureBox1.Image = Properties.Resources.morning;
            }
            else if (currentHour >= 12 && currentHour < 18)
            {
                panel4.BackgroundImage = Properties.Resources.day;
                pictureBox1.Image = Properties.Resources.day;
            }
            else if (currentHour >= 18 && currentHour < 21)
            {
                panel4.BackgroundImage = Properties.Resources.sunset;
                pictureBox1.Image = Properties.Resources.sunset;
            }
            else 
            {
                panel4.BackgroundImage = Properties.Resources.night;
                pictureBox1.Image = Properties.Resources.night;
            }
        }

        private void timer9_Tick(object sender, EventArgs e)
        {

        }

        private void menuItem12_Click(object sender, EventArgs e)
        {
            if (panel6.Visible == true)
            {
                panel6.Visible = false;
            }
            else if (panel6.Visible == false) 
            {
                panel6.Visible = true;
                MessageBox.Show("Обратите внимание: будильник активен только при активном режиме компьютера, и программа предотвращает переход в спящий режим. Однако, чтобы предотвратить статическое изображение, экран автоматически затемняется. Для восстановления яркости просто проведите мышью. Удобно, без лишних усилий, чтобы вернуться к яркому бодрствованию!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                panel6.Visible = true;
                MessageBox.Show("Обратите внимание: будильник активен только при активном режиме компьютера, и программа предотвращает переход в спящий режим. Однако, чтобы предотвратить статическое изображение, экран автоматически затемняется. Для восстановления яркости просто проведите мышью. Удобно, без лишних усилий, чтобы вернуться к яркому бодрствованию!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void label8_TextChanged(object sender, EventArgs e)
        {
            statusBar1.Text = label8.Text;
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            menuItem12.Visible = false;
        }

        private void panel3_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage4_Enter(object sender, EventArgs e)
        {
            menuItem12.Visible = false;
        }
    }
}
