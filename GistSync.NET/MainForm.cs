using System.Diagnostics;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Utils;
using Microsoft.Extensions.Logging;
using WindowsFormsLifetime;

namespace GistSync.NET
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISyncTaskDataService _syncTaskDataService;

        public MainForm(ILogger<MainForm> logger, ISyncTaskDataService syncTaskDataService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _syncTaskDataService = syncTaskDataService;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override void OnLoad(EventArgs e)
        {
            ActivityLogger.Connect(rtb_ActivityLog);
            LoadSyncTasks();
        }

        #region Tab - Tasks

        private void btn_Add_Click(object sender, EventArgs e)
        {
            _serviceProvider.GetForm<NewTaskForm>().ShowDialog(this);
        }

        private void LoadSyncTasks()
        {
            var syncTasks = _syncTaskDataService.GetAllTasks();
            dgv_SyncTasks.AutoGenerateColumns = false;
            dgv_SyncTasks.DataSource = syncTasks;
        }

        private void dgv_SyncTasks_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1 || dgv_SyncTasks.Rows[e.RowIndex].DataBoundItem is not SyncTask syncTask) return;

            // Create context menu 
            var syncTaskContextMenu = new ContextMenuStrip();

            // ItemMenu: Open Folder in File Explorer
            syncTaskContextMenu.Items.Add("Open Folder In File Explorer", null,
                (_, _) => { Process.Start("explorer.exe", syncTask.Directory); });

            // ItemMenu: View Gist
            syncTaskContextMenu.Items.Add("View Gist", null,
                (_, _) =>
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = $"https://gist.github.com/{syncTask.GistId}",
                        UseShellExecute = true
                    });
                });

            // Item Menu: Remove task
            syncTaskContextMenu.Items.Add("Remove", null, async (_, _) =>
                {
                    var dialogResult = MessageBox.Show("Are you sure to remove?", "Remove Task", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        await _syncTaskDataService.RemoveTask(syncTask.Id);
                        LoadSyncTasks();
                    }
                });

            // Select row
            dgv_SyncTasks.ClearSelection();
            var rowSelected = e.RowIndex;
            dgv_SyncTasks.Rows[rowSelected].Selected = true;

            // Set context menu 
            e.ContextMenuStrip = syncTaskContextMenu;
        }

        #endregion

        #region  Tab - Activity Log

        private void btn_SaveToFile_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = $"GistSync.NET-{DateTime.Now:yyyyMMdd-HHmmss}";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Filter = "Text files (*.txt)|*.txt";

            var dialogResult = saveFileDialog.ShowDialog(this);

            if (dialogResult == DialogResult.OK)
                rtb_ActivityLog.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.UnicodePlainText);
        }

        #endregion

        #region Notify Icon

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowUpFromTray();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideToTray();
        }

        public void HideToTray()
        {
            Hide();
        }

        public void ShowUpFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            Environment.Exit(0);
        }

        #endregion

        #region Menu Strip

        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void menuItem_Settings_Click(object sender, EventArgs e)
        {
            _serviceProvider.GetForm<SettingsForm>().ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _serviceProvider.GetForm<AboutForm>().ShowDialog(this);
        }

        #endregion

    }
}