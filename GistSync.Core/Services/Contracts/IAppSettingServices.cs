using GistSync.Core.Models.Settings;

namespace GistSync.Core.Services.Contracts
{
    public interface IAppSettingService
    {
        AppSettings Settings();
    }
}