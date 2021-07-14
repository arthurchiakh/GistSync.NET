using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class Md5FileChecksumService : IFileChecksumService
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISynchronizedFileAccessService _synchronizedFileAccessService;

        internal Md5FileChecksumService(IFileSystem fileSystem, ISynchronizedFileAccessService synchronizedFileAccessService)
        {
            _fileSystem = fileSystem;
            _synchronizedFileAccessService = synchronizedFileAccessService;
        }

        public Md5FileChecksumService(ISynchronizedFileAccessService synchronizedFileAccessService)
            : this(new FileSystem(), synchronizedFileAccessService)
        {
        }

        public string ComputeChecksumByFilePath(string filePath)
        {
            return _synchronizedFileAccessService.SynchronizedReadStream(filePath, stream =>
            {
                using var md5 = MD5.Create();
                using var bufferedStream = new BufferedStream(stream, 1200000);
                var checksum = md5.ComputeHash(bufferedStream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty);
            });
        }

        public string ComputeChecksumByFileContent(byte[] fileContent)
        {
            using var md5 = MD5.Create();
            var checksum = md5.ComputeHash(fileContent);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }

        public async Task<string> ComputeChecksumByFileContentAsync(byte[] fileContent, CancellationToken ct = default)
        {
            await using var ms = new MemoryStream(fileContent);
            using var md5 = MD5.Create();
            var checksum = await md5.ComputeHashAsync(ms, ct);
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }

        public string ComputeChecksumByFileContent(string fileContent)
        {
            return ComputeChecksumByFileContent(Encoding.UTF8.GetBytes(fileContent));
        }

        public Task<string> ComputeChecksumByFileContentAsync(string fileContent, CancellationToken ct = default)
        {
            return ComputeChecksumByFileContentAsync(Encoding.UTF8.GetBytes(fileContent), ct);
        }
    }
}