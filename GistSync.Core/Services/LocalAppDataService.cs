using System;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Services.Contracts;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace GistSync.Core.Services
{
    public class LocalAppDataService : IAppDataService
    {
        public string AppFolderPath { get; }
        private IFileSystem _fileSystem { get; }
        private string _localAppDataDirectory { get; }

        internal LocalAppDataService(IFileSystem fileSystem, string localAppDataDirectory = null)
        {
            _fileSystem = fileSystem;
            _localAppDataDirectory = localAppDataDirectory;

            if (string.IsNullOrWhiteSpace(_localAppDataDirectory) || !_fileSystem.Directory.Exists(_localAppDataDirectory))
                throw new DirectoryNotFoundException("Local Application Data");

            AppFolderPath = Path.Combine(_localAppDataDirectory, Constants.AppName);
            CreateAppDirectory();
        }

        public LocalAppDataService() : this(new FileSystem(), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
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

        public string GetAbsolutePath(params string[] relativeFilePaths)
        {
            var combinePaths = new string[relativeFilePaths.Length + 1];
            combinePaths[0] = AppFolderPath;
            Array.Copy(relativeFilePaths, 0, combinePaths, 1, relativeFilePaths.Length);

            return Path.Combine(combinePaths);
        }
    }
}