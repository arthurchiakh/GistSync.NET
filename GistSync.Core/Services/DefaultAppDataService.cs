using System;
using System.IO;
using System.IO.Abstractions;
using GistSync.Core.Services.Contracts;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace GistSync.Core.Services
{
    public class DefaultAppDataService : IAppDataService
    {
        private readonly string _appFolderFullPath;
        private readonly IFileSystem _fileSystem;

        internal DefaultAppDataService(IFileSystem fileSystem, string appDataDirectory = null)
        {
            _fileSystem = fileSystem;

            if (string.IsNullOrWhiteSpace(appDataDirectory))
                throw new DirectoryNotFoundException("Local Application Data");

            _appFolderFullPath = appDataDirectory;
            CreateAppDirectory();
        }

        public DefaultAppDataService() : this(new FileSystem(), "./data/")
        {
        }

        public void CreateAppDirectory()
        {
            if (!_fileSystem.Directory.Exists(_appFolderFullPath)) _fileSystem.Directory.CreateDirectory(_appFolderFullPath);
        }

        public void DeleteAppDirectory()
        {
            if (_fileSystem.Directory.Exists(_appFolderFullPath)) _fileSystem.Directory.Delete(_appFolderFullPath);
        }

        public string GetAbsolutePath(params string[] relativeFilePaths)
        {
            var combinePaths = new string[relativeFilePaths.Length + 1];
            combinePaths[0] = _appFolderFullPath;
            Array.Copy(relativeFilePaths, 0, combinePaths, 1, relativeFilePaths.Length);

            return Path.Combine(combinePaths);
        }
    }
}