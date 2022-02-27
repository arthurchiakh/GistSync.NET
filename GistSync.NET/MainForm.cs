using System.Diagnostics;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using GistSync.NET.Utils;

namespace GistSync.NET
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISyncTaskDataService _syncTaskDataService;

        public MainForm(IServiceProvider serviceProvider, ISyncTaskDataService syncTaskDataService)
        {
            _serviceProvider = serviceProvider;
            _syncTaskDataService = syncTaskDataService;
            InitializeComponent();

            LoadSyncTasks();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            var newItemForm = _serviceProvider.GetForm<NewTaskForm>();
            newItemForm.StartPosition = FormStartPosition.CenterParent;
            newItemForm.ShowDialog(this);
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
                    await _syncTaskDataService.RemoveTask(syncTask.Id);
                    LoadSyncTasks();
                });

            // Select row
            dgv_SyncTasks.ClearSelection();
            var rowSelected = e.RowIndex;
            dgv_SyncTasks.Rows[rowSelected].Selected = true;

            // Set context menu 
            e.ContextMenuStrip = syncTaskContextMenu;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e) => ShowUpFromTray();

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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
            Environment.Exit(0);
        }
    }
}