using System;

namespace Dispatch.Scripts
{
    public class HubInfo
    {
        public string? HubId { get; set; }

        public Address Address { get; set; } = default!;

        public ContactInfo Contact { get; set; } = default!;

        public TimeSpan StopDuration { get; set; }

        public string? StopNotes { get; set; }

        public SegmentOverrideInfo[]? SegmentOverrideInfos { get; set; }
    }
}