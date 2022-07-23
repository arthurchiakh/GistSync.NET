using System.Diagnostics;
using GistSync.Core;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace GistSync.NET
{
    public partial class MainForm : Form
    {
        private readonly ILogger<MainForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly GistSyncBackgroundService _gistSyncBackgroundService;
        private readonly IOptions<MainFormOptions> _startUpOptions;

        public MainForm(ILogger<MainForm> logger, ISyncTaskDataService syncTaskDataService, IServiceProvider serviceProvider,
            GistSyncBackgroundService gistSyncBackgroundService, IOptions<MainFormOptions> startUpOptions)
        {
            InitializeComponent();
            _syncTaskDataService = syncTaskDataService;
            _serviceProvider = serviceProvider;
            _gistSyncBackgroundService = gistSyncBackgroundService;
            _startUpOptions = startUpOptions;
            _logger = logger;
        }

        protected override void OnLoad(EventArgs e)
        {
            ActivityLogger.Connect(SynchronizationContext.Current!, rtb_ActivityLog);
            LoadSyncTasks();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(!_startUpOptions.Value.LaunchOnStartup && value);
        }

        #region Tab - Tasks

        private void btn_Add_Click(object sender, EventArgs e)
        {
            var newTaskForm = _serviceProvider.GetForm<NewTaskForm>();
            newTaskForm.OnNewTaskAdded(LoadSyncTasks);
            newTaskForm.ShowDialog(this);
        }

        private async void LoadSyncTasks()
        {
            var syncTasks = await _syncTaskDataService.GetAllTasks();
            dgv_SyncTasks.AutoGenerateColumns = false;
            dgv_SyncTasks.DataSource = syncTasks;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadSyncTasks();
        }

        private void dgv_SyncTasks_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1 || dgv_SyncTasks.Rows[e.RowIndex].DataBoundItem is not SyncTask syncTask) return;

            // Create context menu 
            var syncTaskContextMenu = new ContextMenuStrip();

            // ItemMenu: Open Folder in File Explorer
            syncTaskContextMenu.Items.Add("Open Folder In File Explorer", null,
                (_, _) => Process.Start("explorer.exe", @$"/select,""{Path.Combine(syncTask.Directory, syncTask.Files.First().FileName)}\"""));

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
                        await _gistSyncBackgroundService.StopSyncTask(syncTask.Id);
                        await _syncTaskDataService.RemoveTask(syncTask.Id);
                        LoadSyncTasks();
                    }
                });

            // Item Menu: Disable/Enable
            if (syncTask.IsEnabled)
                syncTaskContextMenu.Items.Add("Disable", null, async (_, _) =>
                    {
                        var dialogResult = MessageBox.Show("Are you sure to disable?", "Disable Task", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            await _gistSyncBackgroundService.StopSyncTask(syncTask.Id);
                            await _syncTaskDataService.DisableTask(syncTask.Id);
                            LoadSyncTasks();
                        }
                    });
            else
                syncTaskContextMenu.Items.Add("Enable", null, async (_, _) =>
                    {
                        var dialogResult = MessageBox.Show("Are you sure to enable?", "Enable Task", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            await _gistSyncBackgroundService.StartSyncTask(syncTask.Id);
                            await _syncTaskDataService.EnableTask(syncTask.Id);
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

        private void rtb_ActivityLog_ContextMenu_Clear_Click(object sender, EventArgs e)
        {
            rtb_ActivityLog.Clear();
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
            LoadSyncTasks();
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

        private void settingsToolStripMenuItem_Settings_DropDownOpening(object sender, EventArgs e)
        {
            const string appName = "GistSync.NET";
            var rk = Registry.CurrentUser.OpenSubKey
                (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            var isRegistered = rk?.GetValueNames().Contains(appName);

            if (isRegistered.GetValueOrDefault())
            {
                menuItem_LaunchOnStartup.CheckState = CheckState.Checked;
                menuItem_LaunchOnStartup.Click += (_, _) => { rk?.DeleteValue(appName, false); };
            }
            else
            {
                menuItem_LaunchOnStartup.CheckState = CheckState.Unchecked;
                menuItem_LaunchOnStartup.Click += (_, _) => { rk.SetValue(appName, $"{Application.ExecutablePath} --startup"); };
            }
        }

        #endregion
    }
}