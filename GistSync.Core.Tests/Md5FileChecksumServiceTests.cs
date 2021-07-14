using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using GistSync.Core.Services;
using NUnit.Framework;

namespace GistSync.Core.Tests
{
    public class Md5FileChecksumServiceTests
    {
        private Md5FileChecksumService _checksumService;
        private IFileSystem _fileSystem;
        private string _filePath;

        [SetUp]
        public void SetUp()
        {
            _filePath = "C:/test.txt";
            _fileSystem = new MockFileSystem();
            _checksumService = new Md5FileChecksumService(_fileSystem, new SynchronizedFileAccessService(_fileSystem));
        }

        [TestCase("Lorem ipsum dolor sit amet, consectetur adipiscing elit", "FC10A08DF7FAFA3871166646609E1C95")]
        [TestCase("ed do eiusmod tempor incididunt ut labore et dolore magna aliqua", "92C085F33AB1428DDD200E6F3EEE7F36")]
        [TestCase("Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat", "DEBD2C4E7A3BB09F6F68EE0606460235")]
        public void Md5FileChecksumService_TextFile_Test(string fileContent, string expected)
        {
            _fileSystem.File.WriteAllText(_filePath, fileContent);

            var checksum = _checksumService.ComputeChecksumByFilePath(_filePath);
            Assert.AreEqual(expected, checksum);
        }
    }
}