using System;
using System.Linq;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    public partial class addworldtime : Form
    {
        public string SelectedTimeZone { get; private set; }

        public addworldtime()
        {
            InitializeComponent();
        }

        private string GetUtcOffsetFormatted(TimeSpan offset)
        {
            int hours = Math.Abs(offset.Hours);
            int minutes = Math.Abs(offset.Minutes);

            return $"{(offset.Hours >= 0 ? "+" : "-")}{hours:D2}:{minutes:D2}";
        }

        private void addworldtime_Load(object sender, EventArgs e)
        {
            foreach (TimeZoneInfo timeZone in TimeZoneInfo.GetSystemTimeZones())
            {
                string offset = GetUtcOffsetFormatted(timeZone.BaseUtcOffset);
                string timeZoneName = $"{timeZone.Id} (UTC {offset})";
                comboBox1.Items.Add(timeZoneName);
            }

            switch (Properties.Settings.Default.language)
            {
                case "Русский":
                    {
                        Text = "Добавить часовой пояс";
                        label1.Text = "Выберите часовой пояс, который вам подходит";
                        button1.Text = "ОК";
                        button2.Text = "Отмена";
                        break;
                    }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверяем, что выбранный элемент существует
            if (comboBox1.SelectedItem != null)
            {
                SelectedTimeZone = comboBox1.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a time zone.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
