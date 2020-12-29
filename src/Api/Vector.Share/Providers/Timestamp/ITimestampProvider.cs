using System;

namespace Vector.Share.Providers.Timestamp
{
    public interface ITimestampProvider
    {
        public DateTime Now { get; }
        public DateTime UtcNow { get; }
    }
}