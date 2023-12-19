using Mclock.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Mclock
{
    public partial class Form5 : Form
    {
        Form1 settings;
        public Form5()
        {
            InitializeComponent();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            settings = (Form1)this.Owner;

            if (comboBox4.SelectedItem != null)
            {
                ///Themes
                string theme = comboBox4.SelectedItem.ToString();
                settings = (Form1)this.Owner;
                switch (theme)
                {
                    case "Cветлая":
                        theme0();
                        break;
                    case "Темная":
                        theme1();
                        break;
                    default:
                        theme0();
                        break;
                }

                ///Saving
                Properties.Settings.Default.settings = comboBox4.SelectedItem.ToString();
                Properties.Settings.Default.Save();
            }
            else
            {
                // Обязательные элементы не выбраны, вывод ошибки
                MessageBox.Show("Пожалуйста, выберите все обязательные элементы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        //Themes
        public void theme0()
        {
            settings = (Form1)this.Owner;
            settings.tabPage1.BackColor = Color.White;
            settings.tabPage2.BackColor = Color.White;
            settings.tabPage3.BackColor = Color.White;
            settings.tabPage4.BackColor = Color.White;
            settings.tabPage5.BackColor = Color.White;
            // settings.tabPage6.BackColor = Color.White;
            settings.label4.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label5.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label6.ForeColor = Color.Black;
            settings.label9.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label10.ForeColor = Color.Black;
            settings.label11.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label12.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label14.ForeColor = Color.Black;
            settings.label15.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label16.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label17.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label18.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label19.ForeColor = Color.FromArgb(192, 0, 192);
            settings.label20.ForeColor = Color.FromArgb(192, 0, 192);
            settings.button2.ForeColor = Color.Black;
            settings.button3.ForeColor = Color.Black;
            settings.button4.ForeColor = Color.Black;
            settings.button9.ForeColor = Color.Black;
            settings.button10.ForeColor = Color.Black;
            // settings.button11.ForeColor = Color.Black;
            settings.button12.ForeColor = Color.Black;
            settings.button13.ForeColor = Color.Black;
            settings.button14.ForeColor = Color.Black;
            settings.button15.ForeColor = Color.Black;
            settings.button16.ForeColor = Color.Black;
            settings.button17.ForeColor = Color.Black;
            settings.button18.ForeColor = Color.Black;
            settings.button19.ForeColor = Color.Black;
            settings.button20.ForeColor = Color.Black;
            settings.button22.ForeColor = Color.Black;
            settings.maskedTextBox1.ForeColor = Color.Black;
            settings.maskedTextBox1.BackColor = Color.White;
            settings.maskedTextBox2.ForeColor = Color.Black;
            settings.maskedTextBox2.BackColor = Color.White;
            settings.checkedListBox1.BackColor = Color.White;
            settings.checkedListBox1.ForeColor = Color.Black;
            settings.listBox1.BackColor = Color.White;
            settings.listBox1.ForeColor = Color.Black;
            settings.listBox2.BackColor = Color.White;
            settings.listBox2.ForeColor = Color.Black;
            settings.listBox3.BackColor = Color.White;
            settings.listBox3.ForeColor = Color.Black;
            settings.listBox4.BackColor = Color.White;
            settings.listBox4.ForeColor = Color.Black;
            settings.groupBox2.ForeColor = Color.Black;
        }

        public void theme1()
        {
            settings = (Form1)this.Owner;
            settings.tabPage1.BackColor = Color.Black;
            settings.tabPage2.BackColor = Color.Black;
            settings.tabPage3.BackColor = Color.Black;
            settings.tabPage4.BackColor = Color.Black;
            settings.tabPage5.BackColor = Color.Black;
            // settings.tabPage6.BackColor = Color.Black;
            settings.label4.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label5.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label6.ForeColor = Color.White;
            settings.label9.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label10.ForeColor = Color.White;
            settings.label11.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label12.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label14.ForeColor = Color.White;
            settings.label15.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label16.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label17.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label18.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label19.ForeColor = Color.FromArgb(255, 128, 255);
            settings.label20.ForeColor = Color.FromArgb(255, 128, 255);
            settings.button2.ForeColor = Color.White;
            settings.button3.ForeColor = Color.White;
            settings.button4.ForeColor = Color.White;
            settings.button9.ForeColor = Color.White;
            settings.button10.ForeColor = Color.White;
            // settings.button11.ForeColor = Color.White;
            settings.button12.ForeColor = Color.White;
            settings.button13.ForeColor = Color.White;
            settings.button14.ForeColor = Color.White;
            settings.button15.ForeColor = Color.White;
            settings.button16.ForeColor = Color.White;
            settings.button17.ForeColor = Color.White;
            settings.button18.ForeColor = Color.White;
            settings.button19.ForeColor = Color.White;
            settings.button20.ForeColor = Color.White;
            settings.button22.ForeColor = Color.White;
            settings.maskedTextBox1.ForeColor = Color.White;
            settings.maskedTextBox1.BackColor = Color.Black;
            settings.maskedTextBox2.ForeColor = Color.White;
            settings.maskedTextBox2.BackColor = Color.Black;
            settings.checkedListBox1.BackColor = Color.Black;
            settings.checkedListBox1.ForeColor = Color.White;
            settings.listBox1.BackColor = Color.Black;
            settings.listBox1.ForeColor = Color.White;
            settings.listBox2.BackColor = Color.Black;
            settings.listBox2.ForeColor = Color.White;
            settings.listBox3.BackColor = Color.Black;
            settings.listBox3.ForeColor = Color.White;
            settings.listBox4.BackColor = Color.Black;
            settings.listBox4.ForeColor = Color.White;
            settings.groupBox2.ForeColor = Color.White;
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                panel2.Visible = true;
                int width = 424;
                int height = 463;
                this.Size = new Size (width, height);
            }
            else
            {
                panel2.Visible = false;
                int width = 424;
                int height = 144;
                this.Size = new Size(width, height);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            checkBox1.Visible = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        //End-themes
    }
}