using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class Md5FileChecksumService : IFileChecksumService
    {
        private readonly IFileSystem _fileSystem;

        internal Md5FileChecksumService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Md5FileChecksumService() : this(new FileSystem())
        {
        }

        public string ComputeFileChecksum(string filePath)
        {
            using var stream = new BufferedStream(_fileSystem.File.OpenRead(filePath), 1200000);
            using var md5 = MD5.Create();
            var checksum = md5.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
    }
}