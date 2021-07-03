namespace GistSync.Core.Services.Contracts
{
    public interface IFileChecksumService
    {
        string ComputeFileChecksum(string filePath);
    }
}
