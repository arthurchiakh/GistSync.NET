using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading;
using GistSync.Core.Services.Contracts;

namespace GistSync.Core.Services
{
    public class SynchronizedFileAccessService : ISynchronizedFileAccessService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDictionary<string, ReaderWriterLockSlim> _fileStreamReaderWriterLocks;

        internal SynchronizedFileAccessService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _fileStreamReaderWriterLocks = new Dictionary<string, ReaderWriterLockSlim>();
        }

        public SynchronizedFileAccessService() : this(new FileSystem())
        {
        }

        public void SynchronizedReadStream(string filePath, Action<Stream> action)
        {
            var readerWriterLock = AcquireReaderWriterLock(filePath);
            readerWriterLock.EnterReadLock();

            try
            {
                using var fs = CreateReadStream(filePath);
                action?.Invoke(fs);
            }
            finally
            {
                if (readerWriterLock.IsReadLockHeld)
                    readerWriterLock.ExitReadLock();
            }
        }

        public T SynchronizedReadStream<T>(string filePath, Func<Stream, T> func)
        {
            var readerWriterLock = AcquireReaderWriterLock(filePath);
            readerWriterLock.EnterReadLock();

            try
            {
                using var fs = CreateReadStream(filePath);
                return func(fs);
            }
            finally
            {
                if (readerWriterLock.IsReadLockHeld)
                    readerWriterLock.ExitReadLock();
            }
        }

        public void SynchronizedWriteStream(string filePath, FileMode fileMode, Action<Stream> action)
        {
            var readerWriterLock = AcquireReaderWriterLock(filePath);

            readerWriterLock.EnterWriteLock();

            try
            {
                using var fs = CreateWriteStream(filePath, fileMode);
                action?.Invoke(fs);
            }
            finally
            {
                if (readerWriterLock.IsWriteLockHeld)
                    readerWriterLock.ExitWriteLock();
            }
        }

        public T SynchronizedWriteStream<T>(string filePath, FileMode fileMode, Func<Stream, T> func)
        {
            var readerWriterLock = AcquireReaderWriterLock(filePath);

            readerWriterLock.EnterWriteLock();

            try
            {
                using var fs = CreateWriteStream(filePath, fileMode);
                return func(fs);
            }
            finally
            {
                if (readerWriterLock.IsWriteLockHeld)
                    readerWriterLock.ExitWriteLock();
            }
        }

        public string ReadAllText(string filePath)
        {
            var readerWriterLock = AcquireReaderWriterLock(filePath);
            readerWriterLock.EnterReadLock();

            try
            {
                using var fs = CreateReadStream(filePath);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            finally
            {
                if (readerWriterLock.IsReadLockHeld)
                    readerWriterLock.ExitReadLock();
            }
        }

        private Stream CreateReadStream(string filePath) =>
            _fileSystem.FileStream.Create(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        private Stream CreateWriteStream(string filePath, FileMode fileMode) =>
            _fileSystem.FileStream.Create(filePath, fileMode, FileAccess.Write, FileShare.Read);

        private ReaderWriterLockSlim AcquireReaderWriterLock(string filePath)
        {
            var normalizedFilePath = _fileSystem.Path.GetFullPath(filePath);

            if (!_fileStreamReaderWriterLocks.ContainsKey(normalizedFilePath))
                _fileStreamReaderWriterLocks[normalizedFilePath] = new ReaderWriterLockSlim();

            return _fileStreamReaderWriterLocks[normalizedFilePath];
        }
    }
}