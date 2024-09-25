using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SLIL
{
    public partial class LoginForm : Form
    {
        public LoginForm() => InitializeComponent();

        private void Hide_show_pas_Click(object sender, EventArgs e)
        {
            nickname_input_label.Focus();
            password_input.UseSystemPasswordChar = !password_input.UseSystemPasswordChar;
        }

        private void Create_account_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://t.me/SLIL_AccountBOT") { UseShellExecute = true });
            Application.Exit();
        }
    }
}