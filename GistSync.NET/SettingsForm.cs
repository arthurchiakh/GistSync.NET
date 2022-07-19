using GistSync.Core.Services.Contracts;
using GistSync.Core.Utils;
using Microsoft.Extensions.Configuration;

namespace GistSync.NET
{
    public partial class SettingsForm : Form
    {
        private readonly IConfiguration _config;
        private readonly IGistWatcherService _gistWatcherService;

        public SettingsForm(IConfiguration config, IGistWatcherService gistWatcherService)
        {
            InitializeComponent();
            _config = config;
            _gistWatcherService = gistWatcherService;

            tb_StatusRefreshIntervalSeconds.Value = _config.GetOrSet("StatusRefreshIntervalSeconds", 300);
            tb_MaxConcurrentGistStatusCheck.Value = _config.GetOrSet("MaxConcurrentGistStatusCheck", 5);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            _config.Set("StatusRefreshIntervalSeconds", tb_StatusRefreshIntervalSeconds.Value);
            _config.Set("MaxConcurrentGistStatusCheck", tb_MaxConcurrentGistStatusCheck.Value);

            _gistWatcherService.ReloadConfigurationSettings();

            Close();
        }
    }
}
