using System.Diagnostics;
using System.Net;
using GistSync.Core;
using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Utils;

namespace GistSync.NET
{
    public partial class NewTaskForm : Form
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ISyncTaskDataService _syncTaskDataService;
        private readonly GistSyncBackgroundService _gistSyncBackgroundService;
        private Action _newTaskAddedAction;

        public NewTaskForm(IGitHubApiService gitHubApiService, ISyncTaskDataService syncTaskDataService,
            GistSyncBackgroundService gistSyncBackgroundService)
        {
            _gitHubApiService = gitHubApiService;
            _syncTaskDataService = syncTaskDataService;
            _gistSyncBackgroundService = gistSyncBackgroundService;

            InitializeComponent();

            foreach (var strategy in Enum.GetNames(typeof(SyncModeTypes)))
                cb_SyncMode.Items.Add($"{strategy}");

            cb_SyncMode.SelectedIndex = 0;
        }

        public void OnNewTaskAdded(Action onSuccessfulAction)
        {
            _newTaskAddedAction = onSuccessfulAction;
        }

        private void tb_GistId_TextChanged(object sender, EventArgs e)
        {
            Debounce.Do(async () =>
            {
                cbl_Files.Items.Clear();
                btn_Add.Enabled = false;

                if (_gitHubApiService.TryParseGistIdFromText(tb_GistId.Text, out var gistId))
                {
                    try
                    {
                        var gist = await _gitHubApiService.Gist(gistId);

                        foreach (var file in gist.Files)
                            cbl_Files.Items.Add(file.Value.FileName, true);

                        btn_Add.Enabled = true;
                    }
                    catch (HttpRequestException httpRequestEx)
                    {
                        if (httpRequestEx.StatusCode is HttpStatusCode.TooManyRequests)
                            MessageBox.Show("Too many request.");
                        else
                            MessageBox.Show("Can't find gist repo by the entered id/url.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
                else
                    MessageBox.Show("Invalid gist id/url");

            }, TimeSpan.FromSeconds(2), this);
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            tb_Path.Text = folderBrowserDialog1.SelectedPath;
        }

        private async void btn_Add_Click(object sender, EventArgs e)
        {
            if (_gitHubApiService.TryParseGistIdFromText(tb_GistId.Text, out var gistId))
            {
                if (ValidatePath(tb_Path.Text))
                {
                    var newSyncTask = new SyncTask {
                        GistId = gistId,
                        SyncMode = GetSelectedStrategyTypes(),
                        Directory = tb_Path.Text,
                        Files = cbl_Files.CheckedItems.Cast<string>().Select(ci => new SyncTaskFile {FileName = ci})
                            .ToList(),
                        GitHubPersonalAccessToken = tb_PersonalAccessToken.Text,
                        IsEnabled = true
                    };

                    var updateResult = await _syncTaskDataService.AddOrUpdateTask(newSyncTask);

                    if (updateResult > 0)
                    {
                        _gistSyncBackgroundService.StartSyncTask(newSyncTask);
                        _newTaskAddedAction();
                        Close();
                    }
                    else
                        MessageBox.Show("Unable to add new item");
                }
            }
        }

        private bool ValidatePath(string path)
        {
            if (!Path.IsPathFullyQualified(path))
            {
                MessageBox.Show("Invalid path.");
                return false;
            }

            if (!Directory.Exists(tb_Path.Text))
                MessageBox.Show("Folder not found, program will create one for ya.");

            return true;
        }

        private SyncModeTypes GetSelectedStrategyTypes() =>
            cb_SyncMode.SelectedIndex switch
            {
                0 => SyncModeTypes.ActiveSync,
                1 => SyncModeTypes.TwoWaySync,
                _ => throw new Exception("Unable to get selected sync strategy")
            };

        private void ll_PersonalAccessToken_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/settings/tokens",
                UseShellExecute = true
            });
        }
    }
}
