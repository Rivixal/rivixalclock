using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    public partial class Manythanks : Form
    {
        public Manythanks()
        {
            InitializeComponent();
        }

        private void Manythanks_Load(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.language)
            {
                case "Русский":
                    {
                        Text = "Огромное спасибо";
                        label2.Text = "За помощь в созданий иконки";
                        label3.Text = "За помощь в реализаций перевода и с сохранением настроек";
                        label6.Text = "Бета-тестерам";
                        label7.Text = "";
                        break;
                    }
            }
        }
    }
}
