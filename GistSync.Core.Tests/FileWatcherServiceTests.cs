using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using GistSync.Core.Models;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Moq;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class FileWatcherServiceTests
    {
        private Mock<IFileSystemWatcher> _fileSystemWatcherMock;
        private IFileSystem _fileSystem;
        private IFileWatcherService _fileWatcherService;
        private ISyncTaskDataService _syncTaskDataService;

        private const string FilePath = "C:/test.txt";

        [SetUp]
        public void SetUp()
        {
            // Mock file system watcher
            _fileSystemWatcherMock = new Mock<FileSystemWatcher> { CallBase = true }.As<IFileSystemWatcher>();

            // Mock file system
            var fileSystemMock = new Mock<MockFileSystem> { CallBase = true }.As<IFileSystem>();
            fileSystemMock.Setup(c => c.FileSystemWatcher.CreateNew(It.IsAny<string>()))
                .Returns(() => _fileSystemWatcherMock.Object);
            _fileSystem = fileSystemMock.Object;

            // Prepare text file
            _fileSystem.File.WriteAllText(FilePath,
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua");

            // Mock file checksum service
            var fileChecksumService = new Md5FileChecksumService(_fileSystem, new SynchronizedFileAccessService(_fileSystem));

            // Mock SyncTaskDataService
            var syncTaskDataServiceMock = new Mock<ISyncTaskDataService>();
            syncTaskDataServiceMock.Setup(s => s.GetTask(It.IsAny<int>()))
                .ReturnsAsync(new SyncTask
                {
                    Id = 1,
                    Directory = Path.GetDirectoryName(FilePath)!,
                    Files = new[] { new SyncTaskFile { Id = 1, FileName = Path.GetFileName(FilePath)!,
                        FileChecksum = fileChecksumService.ComputeChecksumByFilePath(FilePath!) } }
                });
            _syncTaskDataService = syncTaskDataServiceMock.Object;

            // Create FileWatcherService
            _fileWatcherService = new FileWatcherService(_fileSystem, fileChecksumService, _syncTaskDataService);
        }

        [Test]
        public async Task FileWatcherService_WriteToFile_ContentNoChange_ExpectNoTriggerEvent()
        {
            var triggerFlag = false;

            var syncTask = await _syncTaskDataService.GetTask(1);

            await _fileWatcherService.Subscribe(1, (filePath, checksum) =>
            {
                if (syncTask.Files.FirstOrDefault(f => f.FileName == Path.GetFileName(filePath))?.FileChecksum !=
                    checksum)
                    triggerFlag = true;
            });
            await _fileSystem.File.AppendAllTextAsync(FilePath, string.Empty);

            // Trigger event
            _fileSystemWatcherMock.Raise(w => w.Changed += null,
                null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(FilePath)!, Path.GetFileName(FilePath)));

            await Task.Delay(100);

            Assert.IsFalse(triggerFlag);
        }

        [Test]
        public async Task FileWatcherService_WriteToFile_ContentChanged_ExpectTriggerEvent()
        {
            var triggerFlag = false;

            var syncTask = await _syncTaskDataService.GetTask(1);

            await _fileWatcherService.Subscribe(1, (filePath, checksum) =>
            {
                if (syncTask.Files.FirstOrDefault(f => f.FileName == Path.GetFileName(filePath))?.FileChecksum !=
                    checksum)
                    triggerFlag = true;
            });
            await _fileSystem.File.AppendAllTextAsync(FilePath, "appended content");

            // Trigger event
            _fileSystemWatcherMock.Raise(w => w.Changed += null,
                null, new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(FilePath)!, Path.GetFileName(FilePath)));

            await Task.Delay(100);

            Assert.IsTrue(triggerFlag);
        }
    }
}