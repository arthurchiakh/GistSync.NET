using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GistSync.Core.Services.Contracts
{
    public interface ISynchronizedFileAccessService
    {
        void SynchronizedReadStream(string filePath, Action<Stream> action);
        T SynchronizedReadStream<T>(string filePath, Func<Stream, T> func);
        void SynchronizedWriteStream(string filePath, FileMode fileMode, Action<Stream> action);
        T SynchronizedWriteStream<T>(string filePath, FileMode fileMode, Func<Stream, T> func);
        string ReadAllText(string filePath);
    }
}
