using System.IO.Abstractions;
using System.Text.Json;
using GistSync.Core.Models.Settings;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class JsonAppSettingService : IAppSettingService
    {
        private const string SETTING_FILE_NAME = "settings.json";

        private readonly IAppDataService _appDataService;
        private readonly IFileSystem _fileSystem;


        internal JsonAppSettingService(IFileSystem fileSystem, IAppDataService appDataService)
        {
            _fileSystem = fileSystem;
            _appDataService = appDataService;
        }

        public JsonAppSettingService(IAppDataService appDataService) : this(new FileSystem(), appDataService)
        {
        }

        public AppSettings Settings()
        {
            _appDataService.CreateAppDirectory();

            var settingFilePath = _appDataService.GetAbsolutePath(SETTING_FILE_NAME);

            if (_fileSystem.File.Exists(settingFilePath))
                return JsonSerializer.Deserialize<AppSettings>(_fileSystem.File.ReadAllText(settingFilePath));
            else
            {
                var setting = new AppSettings();
                _fileSystem.File.WriteAllText(settingFilePath, JsonSerializer.Serialize(setting));
                return setting;
            }
        }
    }
}
