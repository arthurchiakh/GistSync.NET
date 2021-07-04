using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using GistSync.Core.Services;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class LocalAppDataServiceTests
    {
        private LocalAppDataService _localAppDataService;
        private static string _localAppDataDirectory = @"C:\Users\Arthur\AppData\Local";
        private static string _appFolderPath = Path.Combine(@"C:\Users\Arthur\AppData\Local\", Constants.AppName);
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _fileSystem.Directory.CreateDirectory(_localAppDataDirectory);

            _localAppDataService = new LocalAppDataService(_fileSystem, _localAppDataDirectory);
        }

        [Test]
        public void LocalAppDataService_DirectoryMatch()
        {
            Assert.AreEqual(_appFolderPath, _localAppDataService.AppFolderPath);
        }

        [Test]
        public void LocalAppDataService_CreateDirectory_PreviouslyNotExists()
        {
            _localAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void LocalAppDataService_CreateDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderPath);
            _localAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void LocalAppDataService_DeleteDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderPath);
            _localAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [Test]
        public void LocalAppDataService_DeleteDirectory_PreviouslyNotExists()
        {
            _localAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderPath));
        }

        [TestCaseSource(nameof(_localAppDataServiceGetRelativePathReturnExpectedTestCases))]
        public void LocalAppDataService_GetRelativePath_ReturnExpected(string[] sections, string expected)
        {
            Assert.AreEqual(expected, _localAppDataService.GetAbsolutePath(sections));
        }

        private static IEnumerable<TestCaseData> _localAppDataServiceGetRelativePathReturnExpectedTestCases = new[]
            {
                new TestCaseData(new[] {"FolderA"}, Path.Combine(_appFolderPath, "FolderA")),
                new TestCaseData(new[] {"FolderA", "FileB.txt"}, Path.Combine(_appFolderPath, "FolderA", "FileB.txt")),
                new TestCaseData(new[] {"FolderA", "FolderB/FolderC"}, Path.Combine(_appFolderPath, "FolderA", "FolderB/FolderC"))
            };
    }
}
