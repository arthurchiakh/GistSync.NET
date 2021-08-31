using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using GistSync.Core.Services;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class DefaultAppDataServiceTests
    {
        private DefaultAppDataService _defaultAppDataService;
        private static string _appFolderFullPath = @"./data";
        private IFileSystem _fileSystem;

        [SetUp]
        public void SetUp()
        {
            _fileSystem = new MockFileSystem();
            _defaultAppDataService = new DefaultAppDataService(_fileSystem, _appFolderFullPath);
        }

        [Test]
        public void DefaultAppDataService_CreateDirectory_PreviouslyNotExists()
        {
            _defaultAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderFullPath));
        }

        [Test]
        public void DefaultAppDataService_CreateDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderFullPath);
            _defaultAppDataService.CreateAppDirectory();
            Assert.True(_fileSystem.Directory.Exists(_appFolderFullPath));
        }

        [Test]
        public void DefaultAppDataService_DeleteDirectory_PreviouslyExists()
        {
            _fileSystem.Directory.CreateDirectory(_appFolderFullPath);
            _defaultAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderFullPath));
        }

        [Test]
        public void DefaultAppDataService_DeleteDirectory_PreviouslyNotExists()
        {
            _defaultAppDataService.DeleteAppDirectory();
            Assert.False(_fileSystem.Directory.Exists(_appFolderFullPath));
        }

        [TestCaseSource(nameof(_defaultAppDataServiceGetRelativePathReturnExpectedTestCases))]
        public void DefaultAppDataService_GetRelativePath_ReturnExpected(string[] sections, string expected)
        {
            Assert.AreEqual(expected, _defaultAppDataService.GetAbsolutePath(sections));
        }

        private static IEnumerable<TestCaseData> _defaultAppDataServiceGetRelativePathReturnExpectedTestCases = new[]
            {
                new TestCaseData(new[] {"FolderA"}, Path.Combine(_appFolderFullPath, "FolderA")),
                new TestCaseData(new[] {"FolderA", "FileB.txt"}, Path.Combine(_appFolderFullPath, "FolderA", "FileB.txt")),
                new TestCaseData(new[] {"FolderA", "FolderB/FolderC"}, Path.Combine(_appFolderFullPath, "FolderA", "FolderB/FolderC"))
            };
    }
}
