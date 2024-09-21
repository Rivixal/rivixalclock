using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Principal;


namespace Rivixal_Clock
{
    public partial class updater : Form
    {
        private bool updateFound = false;

        public updater()
        {
            InitializeComponent();
            buttonInstallUpdate.Enabled = false;

            switch (Properties.Settings.Default.language)
            {
                case "Русский":
                    {
                        Text = "Проверка обновлений";
                        label2.Text = "Текущая версия: 2.0.2.0";
                        label3.Text = "Обновления не найдены";
                        buttonCheckUpdate.Text = "Проверить обновления";
                        buttonInstallUpdate.Text = "Установить обновление";
                        button1.Text = "Остановить";
                        break;
                    }
            }

        }

        private void buttonCheckUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        private async void CheckForUpdate()
        {
            progressBar1.Style = ProgressBarStyle.Marquee; // Устанавливаем стиль прогресс-бара на бесконечную анимацию
            progressBar1.MarqueeAnimationSpeed = 30;

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072; // Используем Tls12
                    webClient.Headers.Add("User-Agent", "Anything");

                    string response = await webClient.DownloadStringTaskAsync("https://api.github.com/repos/Rivixal/rivixalclock/releases"); // Асинхронная загрузка

                    JArray releases = JArray.Parse(response);

                    if (releases.Count > 0)
                    {
                        progressBar1.Style = ProgressBarStyle.Blocks; // Возвращаем прогресс-бар в обычный стиль
                        JObject latestRelease = (JObject)releases[0];

                        string latestVersion = latestRelease["tag_name"]?.ToString();

                        if (!string.IsNullOrEmpty(latestVersion) && Version.TryParse(latestVersion, out Version version))
                        {
                            if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(version) < 0)
                            {
                                if (Properties.Settings.Default.language == "English")
                                {
                                    label2.Text = "New version available!";
                                    label3.Text = $"Latest version: {latestVersion}";
                                }
                                else if (Properties.Settings.Default.language == "Русский")
                                {
                                    label2.Text = "Доступна новая версия!";
                                    label3.Text = $"Последняя версия: {latestVersion}";
                                }
                                richTextBox1.Text = latestRelease["body"]?.ToString();
                                buttonInstallUpdate.Enabled = true;
                                return;
                            }
                            else
                            {
                                if (Properties.Settings.Default.language == "English")
                                {
                                    MessageBox.Show("Updates not found. Your version is latest!", Properties.Settings.Default.language == "English" ? "Checking Updates" : "Проверка обновлений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    label2.Text = "Your version is up to date.";
                                    label3.Text = "Current version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                                }
                                else if (Properties.Settings.Default.language == "Русский")
                                {
                                    MessageBox.Show("Обновления не найдены!", "Проверить обновления", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    label2.Text = "Ваша версия последняя";
                                    label3.Text = "Текущая версия: " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                                }
                                buttonInstallUpdate.Enabled = false;
                                return;
                            }
                        }
                    }
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? "No release information found." : "Не удалось найти информацию.", Properties.Settings.Default.language == "English" ? "Checking Updates" : "Проверка обновлений", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "Failed to connect to update server. Check your Internet connection and try again" : "Произошла ошибка при попытке подключиться к серверу. Проверьте подключение к Интернету и попробуйте еще раз", Properties.Settings.Default.language == "English" ? "Connection failed..." : "Ошибка подключения...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            catch (Exception ex)
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                MessageBox.Show(Properties.Settings.Default.language == "English" ? $"An error occurred: {ex.Message}" : $"Возникла ошибка: {ex.Message}", Properties.Settings.Default.language == "English" ? "Error" : "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Style = ProgressBarStyle.Blocks; // Возвращаем прогресс-бар в обычный стиль после завершения операции
            }
        }




        private void buttonInstallUpdate_Click(object sender, EventArgs e)
        {

        }

        private void buttonInstallUpdate_Click_1(object sender, EventArgs e)
        {
            DownloadLatestRelease();
        }

        private WebClient webClient;

        private void DownloadLatestRelease()
        {
            try
            {
                webClient = new WebClient();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                webClient.Headers.Add("User-Agent", "Anything");

                string response = webClient.DownloadString("https://api.github.com/repos/Rivixal/rivixalclock/releases");
                JArray releases = JArray.Parse(response);

                if (releases.Count > 0)
                {
                    string downloadUrl = releases[0]["assets"][0]["browser_download_url"].ToString();
                    string savePath = "update.exe";

                    webClient.DownloadProgressChanged += (s, e) =>
                    {
                        progressBar1.Value = e.ProgressPercentage;
                        label5.Text = Properties.Settings.Default.language == "English"
                            ? $"Update process {e.ProgressPercentage}% {e.BytesReceived}/{e.TotalBytesToReceive} Bytes"
                            : $"Скачивание обновления: {e.ProgressPercentage}% {e.BytesReceived}/{e.TotalBytesToReceive} байтов";
                    };

                    webClient.DownloadFileCompleted += (s, e) =>
                    {
                        if (e.Error == null)
                        {
                            try
                            {
                                ProcessStartInfo processStartInfo = new ProcessStartInfo(savePath)
                                {
                                    UseShellExecute = true,
                                    Verb = "runas"  // Запуск от имени администратора
                                };
                                Process.Start(processStartInfo);
                                Application.Exit();  // Закрыть приложение после запуска обновления
                            }
                            catch (Win32Exception)
                            {
                                MessageBox.Show(Properties.Settings.Default.language == "English"
                                    ? "The update requires administrative privileges to install."
                                    : "Для установки обновления требуются права администратора.");
                            }
                        }
                        else
                        {
                            richTextBox1.Text = Properties.Settings.Default.language == "English"
                                ? $"Error downloading the update: {e.Error.Message}"
                                : $"Ошибка скачивания обновления: {e.Error.Message}";
                        }
                    };

                    webClient.DownloadFileAsync(new Uri(downloadUrl), savePath);
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? "Update download started." : "Скачивание обновления началось.");
                }
                else
                {
                    richTextBox1.Text = Properties.Settings.Default.language == "English"
                        ? "Failed to retrieve data about the latest release."
                        : "Не удалось получить данные о последнем релизе.";
                }
            }
            catch (WebException)
            {
                richTextBox1.Text = Properties.Settings.Default.language == "English"
                    ? "Error downloading the update. Check your internet connection and try again."
                    : "Произошла ошибка при попытке скачивания обновления. Проверьте ваше Интернет подключение и повторите попытку еще раз.";
            }
            catch (Exception ex)
            {
                richTextBox1.Text = Properties.Settings.Default.language == "English"
                    ? $"An error occured: {ex.Message}"
                    : $"Возникла ошибка: {ex.Message}";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Check if WebClient is not null and currently downloading
            if (webClient != null && webClient.IsBusy)
            {
                // Cancel the asynchronous download operation
                webClient.CancelAsync();
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "Downloading was cancelled." : "Скачивание обновления отменено.");
                progressBar1.Value = 0;
                labelpercent.Text = "";
            }
            else
            {
                MessageBox.Show(Properties.Settings.Default.language == "English" ? "No download in progress to cancel." : "Операция не была отменена.", Properties.Settings.Default.language == "English" ? "Information" : "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void updater_Load(object sender, EventArgs e)
        {
            // Проверка прав администратора
            if (!IsUserAdministrator())
            {
                // Если не запущено с правами администратора — запросить перезапуск с ними
                DialogResult result = MessageBox.Show(
                    Properties.Settings.Default.language == "English" ? "This action requires administrator rights. Do you want to restart the program as an administrator?" : "Это действие требует прав администратора. Хотите перезапустить программу с правами администратора?",
                    Properties.Settings.Default.language == "English" ? "Administrator Required" : "Требуется администратор",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    RestartAsAdmin();
                }
                else
                {
                    this.Close(); // Закрыть форму, если права не были получены
                }
            }
        }

        private bool IsUserAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RestartAsAdmin()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.UseShellExecute = true;
                processInfo.WorkingDirectory = Environment.CurrentDirectory;
                processInfo.FileName = Application.ExecutablePath;
                processInfo.Verb = "runas"; // Запрос прав администратора

                try
                {
                    Process.Start(processInfo);
                    Application.Exit(); // Завершить текущий процесс
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Properties.Settings.Default.language == "English" ? $"Failed to restart as administrator: {ex.Message}" : $"Не удалось перезапустить с правами администратора: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Settings.Default.language == "English" ? $"An error occurred: {ex.Message}" : $"Возникла ошибка: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
