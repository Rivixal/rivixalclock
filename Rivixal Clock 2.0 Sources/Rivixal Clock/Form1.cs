using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Drawing.Drawing2D;
using System.Web;
using System.Xml.Serialization;
using System.Resources;

namespace Rivixal_Clock
{
    public partial class Form1 : Form
    {
        private Stopwatch stopwatch = new Stopwatch();
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int lapCount = 0;
        private bool isRunning = false;
        // private int millisecondsCounter = 0;
        private int totalTimeInSeconds;
        private int remainingTimeInSeconds;
        private bool isTimerRunning = false;
        private double millisecondsAngle = -90; // Угол поворота миллисекундной стрелки
        private double minutesAngle = -90; // Угол поворота минутной стрелки
        int sandTimerHeight; // Высота песочных часов
        int sandTimerY; // Вертикальная позиция песочных часов
        int sandTimerSpeed = 2; // Скорость анимации песочных часов
        Timer sandTimerAnimationTimer;

        public Form1()
        {
            InitializeComponent();
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS | NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);

            int progress2 = progressBar1.Value;
            DrawCircularProgressBar(progress2);
            lap_score.Text = Properties.Settings.Default.language == "English" ? $"lap: {lapCount}" : $"Круг: {lapCount}";
        }

        /*native-methods*/
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

        /*end*/

        private void datetime_Tick(object sender, EventArgs e)
        {
            string format = "HH:mm:ss";

            if (!DateTimeFormatInfo.CurrentInfo.LongTimePattern.Contains("H"))
            {
                format = "hh:mm:ss tt";
            }

            label1.Text = DateTime.Now.ToString(format);
            label2.Text = DateTime.Now.ToString("dd/MM/yyyy" + " " + "ddd");
            pictureBox1.Invalidate();
        }

        public void AddAlarm2(string time, bool once, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday, bool sunday, string soundFile, string alarmText)
        {
            // Добавление нового будильника в dataGridView1
            dataGridView1.Rows.Add(true, time, once, monday, tuesday, wednesday, thursday, friday, saturday, sunday, soundFile, alarmText);

            dataGridView1.Invalidate();
            dataGridView1.EndEdit();
            dataGridView1.ClearSelection();
        }

        private void menuItem6_Click(object sender, EventArgs e)
        {
            AddAlarm form2 = new AddAlarm();
            form2.ShowDialog();
        }

        /*Alarm-Settings*/

        private bool alarmDisplayed = false; // Переменная для отслеживания открытых окон будильника
        private void CheckAlarms()
        {
            if (alarmDisplayed) // Проверяем, открыто ли уже окно будильника
                return;

            // Создаем список для хранения строк, которые удовлетворяют условиям
            List<DataGridViewRow> eligibleRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isAlarmActive = Convert.ToBoolean(row.Cells["Column"].Value);

                if (isAlarmActive)
                {
                    DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek;
                    bool isDaySelected = Convert.ToBoolean(row.Cells[currentDayOfWeek.ToString()].Value);
                    bool isOnceSelected = Convert.ToBoolean(row.Cells["Column2"].Value); // Добавляем проверку на статус "Once"

                    if (isDaySelected || (isOnceSelected && !alarmDisplayed)) // Добавляем условие для "Once"
                    {
                        string alarmTime = row.Cells["Column1"].Value.ToString(); // Получаем время срабатывания из строки

                        // Проверяем корректность формата времени
                        if (!DateTime.TryParse(alarmTime, out DateTime alarmDateTime))
                        {
                            alarmtimer.Stop();
                            MessageBox.Show(Properties.Settings.Default.language == "English" ? "Invalid time format in 'Column1' column. Please use a valid time format." : "Неверный формат времени в столбце 'Column1'. Пожалуйста, используйте допустимый формат времени.", Properties.Settings.Default.language == "English" ? "Error" : "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        // Проверяем, что текущее время ближе к времени будильника, чем за 1 секунду
                        if (DateTime.Now >= alarmDateTime && DateTime.Now < alarmDateTime.AddSeconds(1))
                        {
                            eligibleRows.Add(row); // Добавляем подходящую строку в список
                        }
                    }
                }
            }

            // Если есть подходящие строки, выбираем первую и отображаем окно будильника
            if (eligibleRows.Count > 0 && !alarmDisplayed)
            {
                DataGridViewRow firstEligibleRow = eligibleRows.OrderBy(r => Convert.ToDateTime(r.Cells["Column1"].Value)).First();
                string soundFile = firstEligibleRow.Cells["Column10"].Value.ToString();
                string alarmText = firstEligibleRow.Cells["Column12"].Value.ToString();
                string alarmTime = firstEligibleRow.Cells["Column1"].Value.ToString(); // Получаем время срабатывания из строки

                Alarm form3 = new Alarm(alarmTime, soundFile, alarmText);
                form3.FormClosed += (s, args) =>
                {
                    alarmDisplayed = false;
                    firstEligibleRow.Cells["Column2"].Value = false; // Устанавливаем статус "Once" в "False" после закрытия окна будильника
                };
                form3.Show();
                alarmDisplayed = true; // Устанавливаем флаг отображения окна будильника
            }
            UpdateNextAlarmLabel();
        }

        private void alarm_Tick(object sender, EventArgs e)
        {
            CheckAlarms();
        }

        private void UpdateNextAlarmLabel()
        {
            DateTime nextAlarmTime = DateTime.MaxValue;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                bool isAlarmActive = Convert.ToBoolean(row.Cells["Column"].Value);
                bool isOnceSelected = Convert.ToBoolean(row.Cells["Column2"].Value); // Добавляем проверку на статус "Once"

                if (isAlarmActive && (!alarmDisplayed)) // Добавляем условие для "Once"
                {
                    DayOfWeek currentDayOfWeek = DateTime.Now.DayOfWeek;
                    bool isDaySelected = Convert.ToBoolean(row.Cells[currentDayOfWeek.ToString()].Value);

                    if (isDaySelected)
                    {
                        string alarmTime = row.Cells["Column1"].Value.ToString();
                        DateTime alarmDateTime;

                        // Попробуйте сначала разобрать время как 12-часовой формат
                        if (!DateTime.TryParseExact(alarmTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmDateTime))
                        {
                            // Если не удалось, попробуйте как 24-часовой формат
                            if (!DateTime.TryParseExact(alarmTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmDateTime))
                            {
                                // Если оба формата не удалось разобрать, переходим к следующей строке
                                continue;
                            }
                        }

                        // Проверяем, что будильник в будущем и ближе к текущему времени, чем предыдущий найденный будильник
                        if (alarmDateTime > DateTime.Now && alarmDateTime < nextAlarmTime)
                        {
                            nextAlarmTime = alarmDateTime;
                        }
                    }
                    else if (isOnceSelected && (!alarmDisplayed))
                    {
                        if (isOnceSelected)
                        {
                            string alarmTime = row.Cells["Column1"].Value.ToString();
                            DateTime alarmDateTime;

                            // Попробуйте сначала разобрать время как 12-часовой формат
                            if (!DateTime.TryParseExact(alarmTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmDateTime))
                            {
                                // Если не удалось, попробуйте как 24-часовой формат
                                if (!DateTime.TryParseExact(alarmTime, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out alarmDateTime))
                                {
                                    // Если оба формата не удалось разобрать, переходим к следующей строке
                                    continue;
                                }
                            }

                            // Проверяем, что будильник в будущем и ближе к текущему времени, чем предыдущий найденный будильник
                            if (alarmDateTime > DateTime.Now && alarmDateTime < nextAlarmTime)
                            {
                                nextAlarmTime = alarmDateTime;
                            }
                        } // Добавляем условие для "Once" 
                    }
                }

                // Проверяем, было ли найдено следующее время будильника
                if (nextAlarmTime != DateTime.MaxValue)
                {
                    string format2;

                    if (DateTimeFormatInfo.CurrentInfo.LongTimePattern.Contains("tt"))
                    {
                        format2 = "hh:mm tt"; // Формат времени с AM/PM
                    }
                    else
                    {
                        format2 = "HH:mm"; // Формат времени в 24-часовом формате
                    }

                    // Обновляем вашу метку с временем следующего будильника
                    statusBarPanel1.Text = Properties.Settings.Default.language == "English" ? "Next signal in" + " " + nextAlarmTime.ToString(format2) : "Следующий сигнал в" + " " + nextAlarmTime.ToString(format2);
                    label6.Text = "True";
                }
                else
                {
                    // Если следующего будильника не найдено, установите метку на какой-то стандартный текст
                    statusBarPanel1.Text = Properties.Settings.Default.language == "English" ? "No alarms set" : "Нет заданых будильников";
                    label6.Text = "False";
                }
            }
        }

        private void menuItem13_Click(object sender, EventArgs e)
        {
            about FormAbout = new about();
            FormAbout.ShowDialog();
        }

        private void menuItem10_Click(object sender, EventArgs e)
        {

        }

        private void UpdateTimerDisplay()
        {
            stopwatch_time.Text = stopwatch.Elapsed.ToString(@"d\.hh\:mm\:ss\.ff");
            lap_score.Text = Properties.Settings.Default.language == "English" ? $"lap: {lapCount}" : $"Круг: {lapCount}";
        }

        private void stopwatch_start_Click(object sender, EventArgs e)
        {
            stopwatch.Start();
            isRunning = true;
            Swatch.Start();
            stopwatch_pause.Enabled = true;
            stopwatch_stop.Enabled = true;
            stopwatch_start.Enabled = false;
        }

        private void stopwatch_stop_Click(object sender, EventArgs e)
        {
            millisecondsAngle = -90;
            minutesAngle = -90;
            stopwatch.Reset();
            isRunning = false;
            Swatch.Stop();
            elapsedTime = TimeSpan.Zero;
            lapCount = 0;
            UpdateTimerDisplay();
            lapsBox1.Items.Clear();
            stopwatch_start.Enabled = true;  // Включаем кнопку Старт
            stopwatch_pause.Enabled = true; // Отключаем кнопку Пауза
        }

        private void Swatch_Tick(object sender, EventArgs e)
        {
            if (isRunning)
            {
                elapsedTime += TimeSpan.FromMilliseconds(Swatch.Interval);
                UpdateTimerDisplay();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                lapCount++;
                lapsBox1.Items.Add(Properties.Settings.Default.language == "English" ? $"lap: {lapCount} - {stopwatch_time.Text}" : $"Круг: {lapCount} - {stopwatch_time.Text}"); // Засечь время в круг
            }
            if (lapsBox1.Items.Count > 0)
            {
                lapsBox1.SelectedIndex = lapsBox1.Items.Count - 1;
            }
        }

        private void stopwatch_pause_Click(object sender, EventArgs e)
        {
            stopwatch.Stop();
            isRunning = false;
            Swatch.Stop();
            stopwatch_start.Enabled = true;
            stopwatch_pause.Enabled = false;
            stopwatch_stop.Enabled = false;
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (!isTimerRunning)
            {
                if (TryParseCustomTime(maskedTextBox1.Text, out totalTimeInSeconds))
                {
                    if (totalTimeInSeconds > 0 && totalTimeInSeconds <= 359999) // Максимальное значение: 99 часов 59 минут 59 секунд
                    {
                        remainingTimeInSeconds = totalTimeInSeconds;
                        isTimerRunning = true;
                        Timeout.Start();
                        Timerset.Text = "Timer set on: " + maskedTextBox1.Text;
                    }
                    else
                    {
                        MessageBox.Show(Properties.Settings.Default.language == "English" ? "Set valid time format: (max 99:59:59)" : "Введите корректное время (макс: 99:59:59)");
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? "Set valid time format (HH:mm:ss)" : "Введите корректное время в формате HH:mm:ss.");
                }
            }
        }

        private void UpdateTimerDisplay4()
        {
            int hours = remainingTimeInSeconds / 3600;
            int minutes = (remainingTimeInSeconds % 3600) / 60;
            int seconds = remainingTimeInSeconds % 60;
            DidgitalTimer.Text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isTimerRunning && remainingTimeInSeconds > 0)
            {
                button3.Enabled = true;
                button1.Enabled = true;
                QuickTimer.Enabled = false;
                maskedTextBox1.Enabled = false;
                remainingTimeInSeconds--;
                UpdateTimerDisplay4();
                double progress = 100.0 * remainingTimeInSeconds / totalTimeInSeconds;
                progressBar1.Value = (int)progress;
                int progress2 = progressBar1.Value;
                DrawCircularProgressBar(progress2);
                button2.Enabled = false;
            }
            else
            {
                button3.Enabled = false;
                button1.Enabled = false;
                QuickTimer.Enabled = true;
                maskedTextBox1.Enabled = true;
                Timeout.Stop();
                isTimerRunning = false;
                timeout form4 = new timeout();
                form4.Show();
                maskedTextBox1.Enabled = true;
                QuickTimer.Enabled = true;
                button2.Enabled = true;
                int progress2 = progressBar1.Value;
                DrawCircularProgressBar(progress2);
            }
        }

        private void DrawCircularProgressBar(int progress)
        {
            int diameter = Math.Min(pictureBox3.Width, pictureBox3.Height) - 10;
            Bitmap bmp = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Очистим фон
            g.Clear(Color.White);

            // Определение центра круга
            int x = (pictureBox3.Width - diameter) / 2;
            int y = (pictureBox3.Height - diameter) / 2;

            // Рассчитаем угол заполнения в градусах
            float sweepAngle = 360f * progress / 100f;

            // Заполним круг полным цветом
            using (SolidBrush brush = new SolidBrush(Color.Green))
            {
                g.FillPie(brush, x, y, diameter, diameter, -90, sweepAngle);
            }

            pictureBox3.Image = bmp;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            DidgitalTimer.Text = maskedTextBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            isTimerRunning = false;
            remainingTimeInSeconds = totalTimeInSeconds; // Возвращаем оставшееся время к исходному
            progressBar1.Value = 100; // Сбрасываем прогресс
            UpdateTimerDisplay4();
            Timeout.Stop();
            button2.Enabled = true;
            button3.Enabled = false;
            button1.Enabled = false;
            maskedTextBox1.Enabled = true;
            QuickTimer.Enabled = true;
            int progress2 = progressBar1.Value;
            DrawCircularProgressBar(progress2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isTimerRunning)
            {
                Timeout.Stop();
                isTimerRunning = false;
                button2.Enabled = false;
                button3.Enabled = false;
            }
            else
            {
                Timeout.Start();
                isTimerRunning = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            QuickTimer.Items.Add(maskedTextBox1.Text);
            SaveQuickTimerItems();
        }

        private void LoadQuickTimerItems()
        {
            var items = Properties.Settings.Default.quicktimer?.Split(',');
            if (items != null)
            {
                QuickTimer.Items.Clear();
                foreach (var item in items)
                {
                    QuickTimer.Items.Add(item);
                }
            }
        }

        private void SaveQuickTimerItems()
        {
            var items = QuickTimer.Items.Cast<string>().ToArray();
            Properties.Settings.Default.quicktimer = string.Join(",", items);
            Properties.Settings.Default.Save();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (QuickTimer.SelectedItem != null)
            {
                QuickTimer.Items.Remove(QuickTimer.SelectedItem);
                SaveQuickTimerItems();
            }
        }

        private void QuickTimer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (QuickTimer.SelectedItem != null)
            {
                maskedTextBox1.Text = QuickTimer.SelectedItem.ToString();
            }
            else
            {
                maskedTextBox1.Text = "00:00:00"; // Можете добавить здесь какое-либо значение по умолчанию
            }
        }

        private string GetUtcOffsetFormatted(TimeSpan offset)
        {
            int hours = Math.Abs(offset.Hours);
            int minutes = Math.Abs(offset.Minutes);

            return $"{(offset.Hours >= 0 ? "+" : "-")}{hours:D2}:{minutes:D2}";
        }

        private string _lang;

        private void Form1_Load(object sender, EventArgs e)
        {
            _lang = Properties.Settings.Default.language;
            string lang = Properties.Settings.Default.language;
            LoadTimeZones();
            switch (lang)
            {
                case "English":
                    english();
                    break;
                case "Русский":
                    russian();
                    break;
                default:
                    english();
                    break;
            }
            comboBox1.SelectedItem = Properties.Settings.Default.language;
            comboBox3.SelectedIndex = Properties.Settings.Default.timer_style;
            comboBox4.SelectedIndex = Properties.Settings.Default.clock_style;
            LoadDataFromSettings();
            LoadQuickTimerItems();
        }

        private void LoadDataFromSettings()
        {
            // Получаем сохраненные данные из настроек
            string savedData = Properties.Settings.Default.salarm;

            // Если данных нет, выходим из метода
            if (string.IsNullOrEmpty(savedData))
                return;

            // Очищаем текущие данные DataGridView
            dataGridView1.Rows.Clear();

            // Разбиваем данные на строки
            string[] rows = savedData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Разбиваем каждую строку на значения ячеек и добавляем в DataGridView
            foreach (string row in rows)
            {
                string[] cells = row.Split(',');
                dataGridView1.Rows.Add(cells);
                dataGridView1.ClearSelection();
            }
        }


        private void WorldClock_Tick(object sender, EventArgs e)
        {
            DateTime utcNow = DateTime.UtcNow;
            string selectedTimeZoneInfo2 = listBox3.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedTimeZoneInfo2))
            {
                string timeZoneId = selectedTimeZoneInfo2.Substring(0, selectedTimeZoneInfo2.IndexOf(" (UTC"));
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

                string format3 = "HH:mm:ss";

                if (!DateTimeFormatInfo.CurrentInfo.LongTimePattern.Contains("H"))
                {
                    format3 = "hh:mm:ss tt";
                }
                else
                {
                    format3 = "hh:mm:ss";
                }

                world_time_text.Text = localTime.ToString(format3);
                date_world_text.Text = localTime.ToString("dd/MM/yyyy");
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItem != null)
            {
                label3.Text = listBox3.SelectedItem.ToString();
                WorldClock.Stop();
                WorldClock.Start();
                WorldClock_Tick(sender, e);
            }
        }

        private void russian() 
        {
            menuItem11.Text = "Главная";
            menuItem5.Text = "Сигнал";
            menuItem6.Text = "Добавить сигнал";
            menuItem12.Text = "Справка";
            menuItem13.Text = "О программе";
            tabPage1.Text = "Будильник";
            tabPage2.Text = "Мировое время";
            tabPage3.Text = "Секундомер";
            tabPage4.Text = "Таймер";
            tabPage5.Text = "Настройки";
            tabPage6.Text = "Установить таймер";
            tabPage7.Text = "Быстрый таймер";
            tabPage8.Text = "Круги";
            tabPage10.Text = "Часовые пояса";
            menuItem1.Text = "Заставка";
            menuItem2.Text = "Сохранить сигнал";
            button5.Text = "Добавить";
            button6.Text = "Удалить";
            button7.Text = "Круг";
            button12.Text = "Добавить быстрый таймер";
            button9.Text = "Удалить выбранный быстрый таймер";
            button1.Text = "Пауза";
            button2.Text = "Старт";
            button3.Text = "Стоп";
            label4.Text = "Настройки";
            label5.Text = "Язык (потребуется перезагрузка)";
            button4.Text = "Сохранить изменения";
            label9.Text = "Установить таймер";
            stopwatch_pause.Text = button1.Text;
            stopwatch_start.Text = button2.Text;
            stopwatch_stop.Text = button3.Text;
            label7.Text = "Вид таймера";
            label8.Text = "Вид часов";
            comboBox3.Items[0] = "Прогресс";
            comboBox3.Items[1] = "Колесо";
            comboBox4.Items[0] = "Аналоговые";
            comboBox4.Items[1] = "Цифровые";
            comboBox4.Items[2] = "Аналоговые и цифровые";
            toolStripButton1.Text = "Добавить";
            toolStripButton2.Text = "Удалить";
            toolStripButton3.Text = "Сохранить";

            //Перевод на русский язык
            dataGridView1.Columns[0].HeaderText = "Активный сигнал";
            dataGridView1.Columns[1].HeaderText = "Время";
            dataGridView1.Columns[2].HeaderText = "Один раз";
            dataGridView1.Columns[3].HeaderText = "ПН";
            dataGridView1.Columns[4].HeaderText = "ВТ";
            dataGridView1.Columns[5].HeaderText = "СР";
            dataGridView1.Columns[6].HeaderText = "ЧТ";
            dataGridView1.Columns[7].HeaderText = "ПТ";
            dataGridView1.Columns[8].HeaderText = "СБ";
            dataGridView1.Columns[9].HeaderText = "ВС";
            dataGridView1.Columns[10].HeaderText = "Звуковой сигнал";
            dataGridView1.Columns[11].HeaderText = "Текст сигнала";
        }
        private void english() {
            dataGridView1.Columns[0].HeaderText = "Active signal";
            dataGridView1.Columns[1].HeaderText = "Time";
            dataGridView1.Columns[2].HeaderText = "Once";
            dataGridView1.Columns[3].HeaderText = "Mon";
            dataGridView1.Columns[4].HeaderText = "Tue";
            dataGridView1.Columns[5].HeaderText = "Wed";
            dataGridView1.Columns[6].HeaderText = "Thu";
            dataGridView1.Columns[7].HeaderText = "Fri";
            dataGridView1.Columns[8].HeaderText = "Sat";
            dataGridView1.Columns[9].HeaderText = "Sun";
            dataGridView1.Columns[10].HeaderText = "Sound";
            dataGridView1.Columns[11].HeaderText = "Text";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.language = comboBox1.SelectedItem?.ToString();
            Properties.Settings.Default.clock_style = comboBox4.SelectedIndex;
            Properties.Settings.Default.timer_style = comboBox3.SelectedIndex;
            if (comboBox1.SelectedItem != null)
            {
                string _msg = _lang == "English" ? $"You are changed application language to {Properties.Settings.Default.language}\nRestart application to apply changes." : "Перезапустите приложение для применения нового языка.";
                if (_lang != Properties.Settings.Default.language)
                {
                    if(MessageBox.Show(_msg, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        Application.Restart();
                    }
                    else
                    {
                        comboBox1.SelectedItem = _lang;
                    }
                }
            }
            Properties.Settings.Default.Save();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }


        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // Строим строку для сохранения
            StringBuilder sb = new StringBuilder();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string[] cells = row.Cells
                                        .Cast<DataGridViewCell>()
                                        .Select(cell => cell.Value?.ToString() ?? string.Empty)
                                        .ToArray();
                    sb.AppendLine(string.Join(",", cells));
                }
            }

            // Сохранение в Properties.Settings.Default
            Properties.Settings.Default.salarm = sb.ToString();
            Properties.Settings.Default.Save();
            DeselectAllCells();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                dataGridView1.Rows.Remove(row);
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            toolStripButton2.Enabled = true;
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            toolStripButton2.Enabled = dataGridView1.RowCount > 0;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem != null)
            {
                ///Themes
                string styletimer = comboBox3.SelectedItem.ToString();
                switch (styletimer)
                {
                    case "Progress":
                        pictureBox3.Visible = false;
                        progressBar1.Visible = true;
                        break;
                    case "Circle":
                        pictureBox3.Visible = true;
                        progressBar1.Visible = false;
                        break;
                    case "Прогресс":
                        pictureBox3.Visible = false;
                        progressBar1.Visible = true;
                        break;
                    case "Колесо":
                        pictureBox3.Visible = true;
                        progressBar1.Visible = false;
                        break;
                    default:
                        pictureBox3.Visible = false;
                        progressBar1.Visible = true;
                        break;
                }
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedItem != null)
            {
                ///Themes
                string styletimer = comboBox4.SelectedItem.ToString();
                switch (styletimer)
                {
                    case "Analog":
                        pictureBox1.Visible = true;
                        panel9.Visible = false;
                        break;
                    case "Digital":
                        pictureBox1.Visible = false;
                        panel9.Visible = true;
                        break;
                    case "Analog and Digital":
                        pictureBox1.Visible = true;
                        panel9.Visible = true;
                        break;
                    case "Аналоговые":
                        pictureBox1.Visible = true;
                        panel9.Visible = false;
                        break;
                    case "Цифровые":
                        pictureBox1.Visible = false;
                        panel9.Visible = true;
                        break;
                    case "Аналоговые и цифровые":
                        pictureBox1.Visible = true;
                        panel9.Visible = true;
                        break;
                    default:
                        pictureBox1.Visible = false;
                        panel9.Visible = true;
                        break;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Строим строку для сохранения
            StringBuilder sb = new StringBuilder();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string[] cells = row.Cells
                                        .Cast<DataGridViewCell>()
                                        .Select(cell => cell.Value?.ToString() ?? string.Empty)
                                        .ToArray();
                    sb.AppendLine(string.Join(",", cells));
                }
            }

            // Сохранение в Properties.Settings.Default
            Properties.Settings.Default.salarm = sb.ToString();
            Properties.Settings.Default.Save();
        }

        private void DeselectAllCells()
        {
            // Сбросить выделение текущей ячейки
            dataGridView1.CurrentCell = null;

            // Сбросить выделение всех ячеек
            dataGridView1.ClearSelection();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            screensaver screensave = new screensaver();
            this.AddOwnedForm(screensave);
            screensave.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            addworldtime addTimeZone = new addworldtime();
            if (addTimeZone.ShowDialog() == DialogResult.OK)
            {
                string selectedTimeZone = addTimeZone.SelectedTimeZone;
                listBox3.Items.Add(selectedTimeZone);
                buttonOK_Click();
            }
        }

        private void buttonOK_Click()
        {
            // Преобразование элементов listBox3 в строку
            var timeZones = string.Join(",", listBox3.Items.Cast<string>().ToArray());
            // Сохранение строки в Properties.Settings
            Properties.Settings.Default.timezone = timeZones;
            Properties.Settings.Default.Save();
        }

        private void LoadTimeZones()
        {
            // Загрузка сохранённых часовых поясов при запуске приложения
            var timeZones = Properties.Settings.Default.timezone;
            if (!string.IsNullOrEmpty(timeZones))
            {
                var timeZoneArray = timeZones.Split(',');
                foreach (var timeZone in timeZoneArray)
                {
                    listBox3.Items.Add(timeZone);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Проверяем, что элемент выбран
            if (listBox3.SelectedItem != null)
            {
                button6.Enabled = true;
                // Удаляем выбранный элемент
                listBox3.Items.Remove(listBox3.SelectedItem);
                // Преобразование элементов listBox3 в строку
                var timeZones = string.Join(",", listBox3.Items.Cast<string>().ToArray());
                // Сохранение строки в Properties.Settings
                Properties.Settings.Default.timezone = timeZones;
                Properties.Settings.Default.Save();
            }
            else
            {
                // Показываем сообщение об ошибке, если элемент не выбран
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "Please select an item to remove." : "Пожалуйста, выберите элемент для удаления.", Properties.Settings.Default.language == "English" ? "Error" : "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
