using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;

namespace SLIL
{
    public partial class Downloading : Form
    {
        public Downloading()
        {
            InitializeComponent();
        }

        private readonly string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\Setup_SLIL.exe";
        private bool complete = false, canceled = false;
        public bool update = true;
        public bool language = true;
        private readonly WebClient client = new WebClient();
        private long lastBytesReceived = 0;
        private DateTime lastUpdateTime = DateTime.Now;

        private void Downloading_Load(object sender, EventArgs e)
        {
            if (!language)
            {
                Text = "Downloading...";
                if (update)
                    title.Text = "Downloading update...";
                else
                    title.Text = "Downloading installer...";
                cancel.Text = "Cancel";
            }
            else
            {
                if (update)
                    title.Text = "Загрузка обновления...";
            }
            if (File.Exists(downloadsPath))
                File.Delete(downloadsPath);
            try
            {
                client.DownloadFileAsync(new Uri("https://base-escape.ru/downloads/Setup_SLIL.exe"), downloadsPath);
            }
            catch (Exception error)
            {
                Error(error);
            }
            client.DownloadProgressChanged += (Sender, E) =>
            {
                long currentBytesReceived = E.BytesReceived;
                TimeSpan elapsedTime = DateTime.Now - lastUpdateTime;
                if (elapsedTime.TotalSeconds > 0)
                {
                    long bytesReceived = currentBytesReceived - lastBytesReceived;
                    double speed = bytesReceived / elapsedTime.TotalSeconds / 1024;
                    string type = "KB";
                    download_progress.Width = (int)((double)E.BytesReceived / E.TotalBytesToReceive * download_progress_panel.Width);
                    progress.Text = $"{(Convert.ToDouble(E.BytesReceived) / E.TotalBytesToReceive) * 100:0.#}%";
                    size_label.Text = $"{Convert.ToDouble(E.BytesReceived) / 1024 / 1024:0.##}MB / {Convert.ToDouble(E.TotalBytesToReceive) / 1024 / 1024:0.##}MB";
                    if(speed >= 1024)
                    {
                        type = "MB";
                        speed /= 1024;
                    }
                    speed_label.Text = $"{speed:0.##} {type}/s";
                    lastBytesReceived = currentBytesReceived;
                    lastUpdateTime = DateTime.Now;
                }
            };
            client.DownloadFileCompleted += (Sender, E) =>
            {
                if (!canceled)
                {
                    try
                    {
                        complete = true;
                        Close();
                        Process.Start(new ProcessStartInfo(downloadsPath) { UseShellExecute = true });
                        Application.Exit();
                    }
                    catch (Exception error)
                    {
                        Error(error);
                    }
                }
            };
        }

        private void Error(Exception error)
        {
            string message = $"Во время загрузки обновления произошла ошибка! {error.Message}.", title = $"Ошибка {error.HResult}!";
            if (!language)
            {
                message = $"An error occurred while downloading the update! {error.Message}.";
                title = $"Error {error.HResult}!";
            }
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            complete = true;
            Application.Exit();
        }

        private void Downloading_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!complete)
                e.Cancel = true;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            canceled = complete = true;
            client.CancelAsync();
            Application.Exit();
        }
    }
}