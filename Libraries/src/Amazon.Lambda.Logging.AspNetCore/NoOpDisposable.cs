using System;

namespace Microsoft.Extensions.Logging
{
    internal class NoOpDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}