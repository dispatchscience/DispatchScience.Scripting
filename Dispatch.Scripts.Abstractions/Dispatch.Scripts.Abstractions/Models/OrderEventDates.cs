using System;

namespace Dispatch.Scripts
{
    public class OrderEventDates
    {
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Dispatched { get; set; }
        public DateTimeOffset? Accepted { get; set; }
        public DateTimeOffset? ArrivedAtPickup { get; set; }
        public DateTimeOffset? PickedUp { get; set; }
        public DateTimeOffset? ArrivedAtDelivery { get; set; }
        public DateTimeOffset? Delivered { get; set; }
        public DateTimeOffset? Cancelled { get; set; }
    }
}