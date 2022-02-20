﻿using GistSync.Core.Models;
using GistSync.Core.Services.Contracts;
using GistSync.NET.Utils;

namespace GistSync.NET
{
    public partial class NewTaskForm : Form
    {
        private readonly IGitHubApiService _gitHubApiService;
        private readonly ISyncTaskDataService _syncTaskDataService;

        public NewTaskForm(IGitHubApiService gitHubApiService, ISyncTaskDataService syncTaskDataService)
        {
            _gitHubApiService = gitHubApiService;
            _syncTaskDataService = syncTaskDataService;

            InitializeComponent();

            foreach (var strategy in Enum.GetNames(typeof(SyncModeTypes)))
                cb_SyncMode.Items.Add($"{strategy} Sync");

            cb_SyncMode.SelectedIndex = 0;
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
                    catch (Exception ex)
                    {
                        MessageBox.Show("Can't find gist repo by the entered id/url.");
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
                    var updateResponse = await _syncTaskDataService.AddOrUpdateTask(new SyncTask
                    {
                        GistId = gistId,
                        SyncMode = GetSelectedStrategyTypes(),
                        Directory = tb_Path.Text,
                        Files = cbl_Files.CheckedItems.Cast<string>().Select(ci => new SyncTaskFile { FileName = ci }).ToList()
                    });

                    if (updateResponse == 0)
                        Close();
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
    }
}
