using System;
using System.Windows.Forms;

namespace SLIL
{
    public partial class Loading : Form
    {
        public Loading()
        {
            InitializeComponent();
        }

        private void Start_timer_Tick(object sender, EventArgs e)
        {
            start_timer.Stop();
            MainMenu form = new MainMenu();
            form.FormClosing += new FormClosingEventHandler(MainMenu_FormCLosing);
            form.Show();
            Hide();
        }

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();
    }
}