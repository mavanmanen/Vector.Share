using System;

namespace Vector.Share.Providers.Timestamp
{
    public class TimestampProvider : ITimestampProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
