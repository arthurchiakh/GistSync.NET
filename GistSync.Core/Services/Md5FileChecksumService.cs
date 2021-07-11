using System;
using System.Data;
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

        internal Md5FileChecksumService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Md5FileChecksumService() : this(new FileSystem())
        {
        }

        public string ComputeChecksumByFilePath(string filePath)
        {
            using var md5 = MD5.Create();
            using var fs = _fileSystem.File.OpenFileStreamInReadOnlyMode(filePath);
            using var stream = new BufferedStream(fs, 1200000);
            var checksum = md5.ComputeHash(stream);
            fs.Dispose();
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
        }

        public async Task<string> ComputeChecksumByFilePathAsync(string filePath, CancellationToken ct = default)
        {
            using var md5 = MD5.Create();
            await using var fs = _fileSystem.File.OpenFileStreamInReadOnlyMode(filePath);
            await using var stream = new BufferedStream(fs, 1200000);
            var checksum = await md5.ComputeHashAsync(stream, ct);
            await fs.DisposeAsync();
            return BitConverter.ToString(checksum).Replace("-", string.Empty);
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