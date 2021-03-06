using System.Threading;
using System.Threading.Tasks;

namespace GistSync.Core.Services.Contracts
{
    public interface IFileChecksumService
    {
        string ComputeChecksumByFilePath(string filePath);
        string ComputeChecksumByFileContent(byte[] fileContent);
        Task<string> ComputeChecksumByFileContentAsync(byte[] fileContent, CancellationToken ct = default);
        string ComputeChecksumByFileContent(string fileContent);
        Task<string> ComputeChecksumByFileContentAsync(string fileContent, CancellationToken ct = default);
    }
}
