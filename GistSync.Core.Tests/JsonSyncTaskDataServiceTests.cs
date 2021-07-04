﻿using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using GistSync.Core.Models;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Moq;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class JsonSyncTaskDataServiceTests
    {
        private IFileSystem _fileSystem;
        private IAppDataService _appDataService;
        private JsonSyncTaskDataService _jsonSyncTaskDataService;
        private string _testGuid;
        private string _mockDataFilePath;
        private string _mockSwapDataFilePath;

        [SetUp]
        public void SetUp()
        {
            _testGuid = Guid.NewGuid().ToString();
            _mockDataFilePath = "C:/data.json";
            _mockSwapDataFilePath = "C:/data.json.swap";
        }

        [Test]
        public async Task JsonSyncTaskDataService_ReadData_ReturnCorrectData()
        {
            // Mock file
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {_mockDataFilePath, new MockFileData(
                    $"[{{\"Guid\":\"{_testGuid}\",\"SyncStrategyType\":0,\"GistId\":\"test-gist-id\",\"GistUpdatedAt\":\"2021-07-04T15:05:02.1709837Z\",\"GistFileName\":\"filename.txt\",\"MappedLocalFilePath\":\"C:/mapped.txt\",\"GitHubPersonalAccessToken\":null}}]")}
            });

            // Mock app data service
            var appDataServiceMock = new Mock<IAppDataService>();
            appDataServiceMock.Setup(s => s.GetAbsolutePath(It.IsAny<string>()))
                .Returns(_mockDataFilePath);
            _appDataService = appDataServiceMock.Object;

            _jsonSyncTaskDataService = new JsonSyncTaskDataService(_fileSystem, _appDataService);

            // Action
            var allTasks = await _jsonSyncTaskDataService.GetAllTasks();

            // Assert
            Assert.AreEqual(1, allTasks.Length);
            Assert.AreEqual(_testGuid, allTasks[0].Guid);
        }

        [Test]
        public async Task JsonSyncTaskDataService_AddTask()
        {
            // Mock file
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {_mockDataFilePath, new MockFileData(
$"[{{\"Guid\":\"{_testGuid}\",\"SyncStrategyType\":0,\"GistId\":\"test-gist-id\",\"GistUpdatedAt\":\"2021-07-04T15:05:02.1709837Z\",\"GistFileName\":\"filename.txt\",\"MappedLocalFilePath\":\"C:/mapped.txt\",\"GitHubPersonalAccessToken\":null}}]")}
            });


            // Mock app data service
            var appDataServiceMock = new Mock<IAppDataService>();
            appDataServiceMock.SetupSequence(s => s.GetAbsolutePath(It.IsAny<string>()))
                .Returns(_mockDataFilePath)
                .Returns(_mockSwapDataFilePath)
                .Returns(_mockDataFilePath)
                .Returns(_mockDataFilePath);
            _appDataService = appDataServiceMock.Object;

            _jsonSyncTaskDataService = new JsonSyncTaskDataService(_fileSystem, _appDataService);

            // Action
            var newGuid = Guid.NewGuid().ToString();
            await _jsonSyncTaskDataService.AddTask(new SyncTask
            {
                Guid = newGuid,
                GistId = "test-gist-id",
                GistFileName = "filename.txt",
                GistUpdatedAt = DateTime.UtcNow,
                MappedLocalFilePath = "C:/mapped.txt",
                SyncStrategyType = SyncStrategyTypes.ActiveSync
            });


            var allTasks = await _jsonSyncTaskDataService.GetAllTasks();

            // Assert
            Assert.AreEqual(2, allTasks.Length);
            Assert.AreEqual(_testGuid, allTasks[0].Guid);
            Assert.AreEqual(newGuid, allTasks[1].Guid);
        }

        [Test]
        public async Task JsonSyncTaskDataService_RemoveTask()
        {
            // Mock file
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {_mockDataFilePath, new MockFileData(
                    $"[{{\"Guid\":\"{_testGuid}\",\"SyncStrategyType\":0,\"GistId\":\"test-gist-id\",\"GistUpdatedAt\":\"2021-07-04T15:05:02.1709837Z\",\"GistFileName\":\"filename.txt\",\"MappedLocalFilePath\":\"C:/mapped.txt\",\"GitHubPersonalAccessToken\":null}}]")}
            });


            // Mock app data service
            var appDataServiceMock = new Mock<IAppDataService>();
            appDataServiceMock.SetupSequence(s => s.GetAbsolutePath(It.IsAny<string>()))
                .Returns(_mockDataFilePath)
                .Returns(_mockSwapDataFilePath)
                .Returns(_mockDataFilePath)
                .Returns(_mockDataFilePath);
            _appDataService = appDataServiceMock.Object;

            _jsonSyncTaskDataService = new JsonSyncTaskDataService(_fileSystem, _appDataService);

            // Action
            await _jsonSyncTaskDataService.RemoveTask(_testGuid);

            var allTasks = await _jsonSyncTaskDataService.GetAllTasks();

            // Assert
            Assert.AreEqual(0, allTasks.Length);
        }

    }
}