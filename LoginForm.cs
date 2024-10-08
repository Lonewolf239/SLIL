﻿using IniReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SLIL
{
    public partial class LoginForm : Form
    {
        public LoginForm() => InitializeComponent();

        public bool CanClose = false;
        public bool DownloadedLocalizationList { get; set; }
        public Dictionary<string, string> SupportedLanguages;
        public int Error { get; set; }
        private string Language;

        private void Hide_show_pas_Click(object sender, EventArgs e)
        {
            nickname_input_label.Focus();
            password_input.UseSystemPasswordChar = !password_input.UseSystemPasswordChar;
            hide_show_pas_c.BackgroundImage = password_input.UseSystemPasswordChar ?
                Properties.Resources.hide_pas :
                Properties.Resources.show_pas;
        }

        private void Create_account_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/SLIL_AccountBOT") { UseShellExecute = true });

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose) e.Cancel = true;
        }

        private void ResizeButtonsInControl(Control control)
        {
            if (control is Button button && !control.Name.EndsWith("_c"))
            {
                button.Size = new Size(0, 0);
                if (button.Name.EndsWith("_r"))
                    button.Left = button.Parent.Width - button.Width - 7;
                else if (button.Name.EndsWith("_l"))
                    button.Left = 7;
                else if (button.Name.EndsWith("_cp"))
                    button.Left = (button.Parent.Width - button.Width) / 2;
            }
            foreach (Control childControl in control.Controls)
                ResizeButtonsInControl(childControl);
        }

        public void SetLanguage()
        {
            if (DownloadedLocalizationList)
            {
                title_label.Text = Loading.Localizations.GetLString(Language, "8-0");
                login_btn_r.Text = Loading.Localizations.GetLString(Language, "8-1");
                create_account_l.Text = Loading.Localizations.GetLString(Language, "8-2");
                status_label.Text = Loading.Localizations.GetLString(Language, "8-3");
                password_label.Text = Loading.Localizations.GetLString(Language, "8-4");
                nickname_input_label.Text = Loading.Localizations.GetLString(Language, "8-5");
                error_label.Text = Loading.Localizations.GetLString(Language, "8-6") + $" {Error}";
                exit_btn_cp.Text = Loading.Localizations.GetLString(Language, "8-7");
                buy_label.Text = Loading.Localizations.GetLString(Language, "8-8");
                buy_btn_cp.Text = Loading.Localizations.GetLString(Language, "8-9");
            }
            else
            {
                title_label.Text = "To start playing, please log in to your account";
                login_btn_r.Text = "Login";
                create_account_l.Text = "Create account";
                status_label.Text = "Incorrect login or password!";
                password_label.Text = "Password:";
                nickname_input_label.Text = "Username:";
                error_label.Text = $"Something went wrong. Error code: {Error}";
                exit_btn_cp.Text = "Exit";
                buy_label.Text = "You have not purchased the SLIL.\nPlease purchase a license to play in the bot and try again";
                buy_btn_cp.Text = "Buy the SLIL";
            }
            ResizeButtonsInControl(this);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Language = INIReader.GetString(Program.iniFolder, "CONFIG", "language", "English");
            if (!SupportedLanguages.Values.Contains(Language)) Language = "English";
            SetLanguage();
        }
    }
}