using System;
using System.IO;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string fileName = Path.GetFileNameWithoutExtension(Application.ExecutablePath).ToLower();

            if (fileName.EndsWith(".scr") || args.Length > 0)
            {
                if (args.Length > 0)
                {
                    string arg = args[0].ToLower().Trim();
                    if (arg == "/s" || arg == "/S")
                    {
                        Application.Run(new screensaver());
                    }
                    else if (arg == "/c" || arg == "/C")
                    {
                        MessageBox.Show("Настройки заставки", "Заставка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (arg.StartsWith("/p") || arg.StartsWith("/P"))
                    {
                        string handleStr = args[1];
                        IntPtr previewWndHandle = new IntPtr(long.Parse(handleStr));
                        Application.Run(new screensaver(previewWndHandle));
                    }
                    else if (arg.StartsWith("/debugmode-alarm") || arg.StartsWith("/DBGALARM"))
                    {
                        Application.Run(new Alarm("default", "Standard", "TEST MODE"));
                    }
                    else if (arg.StartsWith("/debugmode-timeout") || arg.StartsWith("/DBGALARM"))
                    {
                        Application.Run(new timeout());
                    }
                    else if (arg.StartsWith("/homecomingrocktest") || arg.StartsWith("/HRT"))
                    {
                        Application.Run(new Alarm("default", "Homecomingremixsound", "TEST MODE HOMECOMING ROCK TEST"));
                    }
                    else if (arg.StartsWith("/update") || arg.StartsWith("/UPD"))
                    {
                        Application.Run(new updater());
                    }
                    else if (arg.StartsWith("/artosik") || arg.StartsWith("/ARTOSIK"))
                    {
                        Application.Run(new artosik());
                    }
                    else
                    {
                        Application.Run(new Form1());
                    }
                }
                else
                {
                    Application.Run(new screensaver());
                }
            }
            else
            {
                Application.Run(new Form1());
            }
        }
    }
}
