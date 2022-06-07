using System.Diagnostics;
using System.Reflection;

namespace GistSync.NET
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            lbl_Version.Text = Assembly.GetEntryAssembly()!.GetName().Version!.ToString();
            lbl_ProjectUrl.Text = Assembly.GetEntryAssembly()!.GetCustomAttributes<AssemblyMetadataAttribute>()
                .First(a => a.Key == "RepositoryUrl").Value;
        }

        private void lbl_ProjectUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = lbl_ProjectUrl.Text, UseShellExecute = true });
        }
    }
}
