﻿using System;
using System.Linq;
using System.Threading;
using System.Management;
using System.Globalization;
using System.Windows.Forms;


namespace SLIL
{
    internal static class Program
    {
        public static Mutex mutex;
        public static Cursor SLILCursor;
        public static string current_version = "|1.3.1|";
        public static string iniFolder = "config.ini";

        [STAThread]
        static void Main()
        {
            mutex = new Mutex(true, "SLIL_Unique_Mutex");
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
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
                    if (Check_Language())
                        Console.WriteLine("Внимание: У вас меньше 4 ГБ оперативной памяти. Производительность приложения может быть снижена.");
                    else
                        Console.WriteLine("Warning: You have less than 4GB of RAM. The performance of the application may be reduced.");
                }
                using (var ms = new System.IO.MemoryStream(Properties.Resources.SLILCursor))
                    SLILCursor = new Cursor(ms);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Loading());
                mutex.ReleaseMutex();
            }
        }

        static bool HasEnoughMemory()
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке памяти: {ex.Message}");
            }
            return false;
        }

        static bool Check_Language()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string language = ci.Name.ToLower();
            string[] supportedLanguages = { "ru", "uk", "be", "kk", "ky" };
            return Array.Exists(supportedLanguages, lang => lang.Equals(language) || lang.Equals(language.Substring(0, 2)));
        }

        static bool CheckWindows()
        {
            try
            {
                Version osVersion = Environment.OSVersion.Version;
                if (osVersion.Major >= 10 && osVersion.Build >= 10240)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        static bool IsDirectX10Available()
        {
            try
            {
                var device = new SharpDX.Direct3D10.Device(SharpDX.Direct3D10.DriverType.Hardware, SharpDX.Direct3D10.DeviceCreationFlags.None);
                device.Dispose();
                return true;
            }
            catch (SharpDX.SharpDXException)
            {
                return false;
            }
        }
    }
}