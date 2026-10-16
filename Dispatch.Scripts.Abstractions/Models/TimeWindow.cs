using System;

namespace Dispatch.Scripts
{
    public struct TimeWindow
    {
        public TimeWindow(DateTimeOffset from, DateTimeOffset to)
        {
            From = from;
            To = to;
        }

        public DateTimeOffset From { get; }
        public DateTimeOffset To { get; }
    }
}