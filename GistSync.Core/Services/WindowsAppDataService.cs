using System;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Services.Contracts;
using Microsoft.VisualBasic;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace GistSync.Core.Services
{
    public class WindowsAppDataService : IAppDataService
    {
        public string AppFolderPath { get; }
        private IFileSystem _fileSystem { get; }
        private string _localAppDataDirectory { get; }

        internal WindowsAppDataService(IFileSystem fileSystem, string localAppDataDirectory)
        {
            _fileSystem = fileSystem;
            _localAppDataDirectory = localAppDataDirectory;

            if (string.IsNullOrWhiteSpace(_localAppDataDirectory) || !_fileSystem.Directory.Exists(_localAppDataDirectory))
                throw new DirectoryNotFoundException("Local App Data");

            AppFolderPath = Path.Combine(_localAppDataDirectory, Constants.AppName);
        }

        public WindowsAppDataService() : this(new FileSystem(), Environment.GetEnvironmentVariable("LocalAppData"))
        {
        }

        public void CreateAppDirectory()
        {
            if (!_fileSystem.Directory.Exists(AppFolderPath)) _fileSystem.Directory.CreateDirectory(AppFolderPath);
        }

        public void DeleteAppDirectory()
        {
            if (_fileSystem.Directory.Exists(AppFolderPath)) _fileSystem.Directory.Delete(AppFolderPath);
        }

        public string GetRelativePath(params string[] paths)
        {
            var combinePaths = new string[paths.Length + 1];
            combinePaths[0] = AppFolderPath;
            Array.Copy(paths, 0, combinePaths, 1, paths.Length);

            return Path.Combine(combinePaths);
        }
    }
}
