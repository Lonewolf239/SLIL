using System;
using System.IO;
using IniReader;
using System.Linq;
using System.Threading;
using System.Management;
using System.Globalization;
using System.Windows.Forms;


namespace SLIL
{
    internal static class Program
    {
        internal static Mutex mutex;
        internal static Cursor SLILCursorDefault, SLILCursorHelp;
        internal static string SLILFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SLIL\\";
        internal static string current_version = "|1.3.1|";
        internal static INIReader iniReader;

        [STAThread]
        internal static void Main()
        {
            mutex = new Mutex(true, "SLIL_Unique_Mutex");
            Directory.CreateDirectory(SLILFolder);
            if (File.Exists("config.ini")) File.Move("config.ini", $"{SLILFolder}config.ini");
            if (File.Exists("data.enc")) File.Delete("data.enc");
            MoveFiles("saves");
            MoveFiles("screenshots");
            iniReader = new INIReader($"{SLILFolder}config.ini");
            if (!mutex.WaitOne(TimeSpan.Zero, true)) return;
            if (!CheckWindows())
            {
                string message = "Эта игра доступна только на Windows 10 или более новых версиях операционной системы. Пожалуйста, обновите вашу систему или установите игру на компьютере с более новой версией Windows.";
                string title = "Требования к операционной системе";
                if (!Check_Language())
                {
                    message = "This game is only available on Windows 10 or newer operating systems. Please update your system or install the game on a computer with a newer version of Windows.";
                    title = "Operating System Requirements";
                }
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!IsDirectX10Available())
            {
                string message = "DirectX 10 не доступен. Пожалуйста, обновите свою систему для продолжения работы программы.";
                string title = "Ошибка совместимости";
                if (!Check_Language())
                {
                    message = "DirectX 10 is not available. Please update your system to continue using the program.";
                    title = "Compatibility error";
                }
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!HasEnoughMemory())
            {
                string message = "Внимание: У вас меньше 4 ГБ оперативной памяти. Производительность приложения может быть снижена.";
                string title = "Внимание!";
                if (!Check_Language())
                {

                    message = "Warning: You have less than 4GB of RAM. The performance of the application may be reduced.";
                    title = "Warning!";
                }
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            using (var ms = new MemoryStream(Properties.Resources.SLILCursorDefault))
                SLILCursorDefault = new Cursor(ms);
            using (var ms = new MemoryStream(Properties.Resources.SLILCursorHelp))
                SLILCursorHelp = new Cursor(ms);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Loading());
            mutex.ReleaseMutex();
        }

        private static void MoveFiles(string path)
        {
            if (!Directory.Exists(path)) return;
            foreach (var file in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileName(file);
                bool isScreenRecording = fileName.Contains("screen_recording");
                if (isScreenRecording) Directory.CreateDirectory($"{SLILFolder}records");
                else Directory.CreateDirectory($"{SLILFolder}screenshots");
                File.Move(file, Path.Combine(SLILFolder + (isScreenRecording ? "records" : "screenshots"), fileName));
            }
            Directory.Delete(path, true);
        }

        private static bool HasEnoughMemory()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    ulong totalMemory = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    ulong memoryInGB = totalMemory / 1024 / 1024 / 1024;
                    return memoryInGB >= 4;
                }
            }
            catch { }
            return false;
        }

        private static bool Check_Language()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string language = ci.Name.ToLower();
            string[] supportedLanguages = { "ru", "uk", "be", "kk", "ky" };
            return Array.Exists(supportedLanguages, lang => lang.Equals(language) || lang.Equals(language.Substring(0, 2)));
        }

        private static bool CheckWindows()
        {
            try
            {
                Version osVersion = Environment.OSVersion.Version;
                if (osVersion.Major >= 10 && osVersion.Build >= 10240)
                    return true;
            }
            catch { }
            return false;
        }

        private static bool IsDirectX10Available()
        {
            try
            {
                var device = new SharpDX.Direct3D10.Device(SharpDX.Direct3D10.DriverType.Hardware, SharpDX.Direct3D10.DeviceCreationFlags.None);
                device.Dispose();
                return true;
            }
            catch { return false; }
        }
    }
}