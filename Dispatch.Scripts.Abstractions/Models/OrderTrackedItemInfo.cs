using System;
using System.Collections.Generic;

namespace Dispatch.Scripts
{
    public class OrderTrackedItemInfo
    {
        public string Id { get; set; } = default!;  
        public string BatchId { get; set; } = default!;
        public string ItemId { get; set; } = default!;
        public int ItemNumber { get; set; }
        public OrderItemTrackingType TrackingType { get; set; }
        public DateTimeOffset TrackedTime { get; set; }
        public LatLng TrackedLocation { get; set; }
        public string CheckpointId { get; set; } = default!;  
        public OrderItemTrackingStatus Status { get; set; }
        public bool Damaged { get; set; }
        public string BarcodeTemplate { get; set; } = default!;
        public string? SegmentOrderId { get; set; }
        public List<string> ExceptionCodeIds { get; set; } = new List<string>();
    }
}
