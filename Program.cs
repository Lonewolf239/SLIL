﻿using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace SLIL
{
    internal static class Program
    {
        public static Mutex mutex;
        public static string current_version = "|1.2.2.1|";

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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Loading());
                mutex.ReleaseMutex();
            }
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
    }
}