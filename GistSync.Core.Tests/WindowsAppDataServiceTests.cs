using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using GistSync.Core.Services;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class WindowsAppDataServiceTests
    {
        private WindowsAppDataService _windowsAppDataService;
        private static string _localAppDataDirectory = @"C:\Users\Arthur\AppData\Local";
        private static string _appFolderPath = Path.Combine(@"C:\Users\Arthur\AppData\Local\", Constants.AppName);
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.Directory.CreateDirectory(_localAppDataDirectory);

            _windowsAppDataService = new WindowsAppDataService(_fileSystem, _localAppDataDirectory);
        }

        [Test]
        public void WindowsAppDataService_DirectoryMatch()
        {
            Assert.AreEqual(_appFolderPath, _windowsAppDataService.AppFolderPath);
        }

        [Test]
        public void WindowsAppDataService_CreateDirectory_PreviouslyNotExists()
        {
            _windowsAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void WindowsAppDataService_CreateDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderPath);
            _windowsAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void WindowsAppDataService_DeleteDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderPath);
            _windowsAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void WindowsAppDataService_DeleteDirectory_PreviouslyNotExists()
        {
            _windowsAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [TestCaseSource(nameof(_windowsAppDataServiceGetRelativePathReturnExpectedTestCases))]
        public void WindowsAppDataService_GetRelativePath_ReturnExpected(string[] sections, string expected)
        {
            Assert.AreEqual(expected, _windowsAppDataService.GetAbsolutePath(sections));
        }

        private static IEnumerable<TestCaseData> _windowsAppDataServiceGetRelativePathReturnExpectedTestCases = new[]
            {
                new TestCaseData(new[] {"FolderA"}, Path.Combine(_appFolderPath, "FolderA")),
                new TestCaseData(new[] {"FolderA", "FileB.txt"}, Path.Combine(_appFolderPath, "FolderA", "FileB.txt")),
                new TestCaseData(new[] {"FolderA", "FolderB/FolderC"}, Path.Combine(_appFolderPath, "FolderA", "FolderB/FolderC"))
            };
    }
}
