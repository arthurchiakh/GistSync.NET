using System;
using System.Windows.Forms;

namespace GistSync.Windows
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private void App_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                ShowInTaskbar = true;
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            ShowInTaskbar = false;
            notifyIcon.Visible = false;
        }
    }
}