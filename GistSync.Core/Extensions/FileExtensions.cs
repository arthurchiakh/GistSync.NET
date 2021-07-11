using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace GistSync.Core.Extensions
{
    public static class FileExtensions
    {
        public static Stream OpenFileStreamInReadOnlyMode(this IFile file, string filePath)
        {
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public static async Task<string> ReadAllTextInReadOnlyModeAsync(this IFile file, string filePath)
        {
            await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var textReader = new StreamReader(fs);
            var text = await textReader.ReadToEndAsync();
            await fs.DisposeAsync();
            return text;
        }

        public static string ReadAllTextInReadOnlyMode(this IFile file, string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var textReader = new StreamReader(fs);
            var text = textReader.ReadToEnd();
            fs.Dispose();
            return text;
        }
    }
}