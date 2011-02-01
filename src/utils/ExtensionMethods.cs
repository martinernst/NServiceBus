using System;
using System.Threading;

namespace NServiceBus.Utils
{
    public static class ExtensionMethods
    {
        public static IDisposable ReadLock(this ReaderWriterLockSlim self)
        {
            self.EnterReadLock();
            return new RunOnDispose(self.ExitReadLock);
        }

        public static IDisposable WriteLock(this ReaderWriterLockSlim self)
        {
            self.EnterWriteLock();
            return new RunOnDispose(self.ExitWriteLock);
        }
    }
}