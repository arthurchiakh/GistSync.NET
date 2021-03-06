using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using GistSync.Core.Services.Contracts;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace GistSync.Core.Services
{
    public class DefaultAppDataService : IAppDataService
    {
        private readonly string _appDataDirectory;
        private readonly IFileSystem _fileSystem;

        internal DefaultAppDataService(IFileSystem fileSystem, string appDataDirectory = "./data/")
        {
            _fileSystem = fileSystem;

            if (string.IsNullOrWhiteSpace(appDataDirectory))
                throw new ArgumentNullException(nameof(appDataDirectory));

            _appDataDirectory = appDataDirectory;

            CreateAppDirectory();
        }

        public DefaultAppDataService() : this(new FileSystem(), "./data/")
        {
        }

        public void CreateAppDirectory()
        {
            if (!_fileSystem.Directory.Exists(_appDataDirectory)) _fileSystem.Directory.CreateDirectory(_appDataDirectory);
        }

        public void DeleteAppDirectory()
        {
            if (_fileSystem.Directory.Exists(_appDataDirectory)) _fileSystem.Directory.Delete(_appDataDirectory);
        }

        public string GetAbsolutePath(params string[] relativeFilePaths)
        {
            var combinePaths = new string[relativeFilePaths.Length + 1];
            combinePaths[0] = _appDataDirectory;
            Array.Copy(relativeFilePaths, 0, combinePaths, 1, relativeFilePaths.Length);

            return _fileSystem.Path.Combine(combinePaths);
        }
    }
}