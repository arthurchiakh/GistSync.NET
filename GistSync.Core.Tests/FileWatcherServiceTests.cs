using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using GistSync.Core.Factories;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Moq;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class FileWatcherServiceTests
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<IFileSystemWatcher> _fileSystemWatcherMock;
        private IFileSystem _fileSystem;
        private IFileWatchFactory _fileWatchFactory;
        private IFileWatcherService _fileWatcherService;
        private IFileChecksumService _fileChecksumService;

        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _filePath = "C:/test.txt";

            // Mock file system watcher
            _fileSystemWatcherMock = new Mock<FileSystemWatcher> { CallBase = true }.As<IFileSystemWatcher>();
            // Mock file system
            _fileSystemMock = new Mock<MockFileSystem> { CallBase = true }.As<IFileSystem>();
            _fileSystemMock.Setup(c => c.FileSystemWatcher.CreateNew(It.IsAny<string>()))
                .Returns(() => _fileSystemWatcherMock.Object);
            _fileSystem = _fileSystemMock.Object;

            // Prepare text file
            _fileSystem.File.WriteAllText(_filePath,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua");

            _fileChecksumService = new Md5FileChecksumService(_fileSystem);
            _fileWatchFactory = new FileWatchFactory(_fileSystem, _fileChecksumService);
            _fileWatcherService = new FileWatcherService(_fileSystem, _fileChecksumService);
        }

        [Test]
        public void FileWatcherService_WriteToFile_ContentNoChange_ExpectNoTriggerEvent()
        {
            var triggerFlag = false;

            var watch = _fileWatchFactory.Create(_filePath);
            watch.FileContentChangedEvent += fileWatch => triggerFlag = true;

            _fileWatcherService.AddWatch(watch);
            _fileSystem.File.AppendAllText(_filePath, string.Empty);

            // Trigger event
            _fileSystemWatcherMock.Raise(w => w.Changed += null,
                null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(_filePath), Path.GetFileName(_filePath)));

            Assert.IsFalse(triggerFlag);
        }

        [Test]
        public void FileWatcherService_WriteToFile_ContentNoChange_ExpectTriggerEvent()
        {
            var triggerFlag = false;

            var watch = _fileWatchFactory.Create(_filePath);
            watch.FileContentChangedEvent += fileWatch => triggerFlag = true;
            _fileWatcherService.AddWatch(watch);
            _fileSystem.File.AppendAllText(_filePath, "appended content");

            // Trigger event
            _fileSystemWatcherMock.Raise(w => w.Changed += null,
                null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(_filePath), Path.GetFileName(_filePath)));

            Assert.IsTrue(triggerFlag);
        }
    }
}