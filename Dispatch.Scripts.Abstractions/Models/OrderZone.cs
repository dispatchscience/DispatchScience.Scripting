
namespace Dispatch.Scripts
{
    public class OrderZone
    {
        public string ZoneId { get; set; } = default!;
        public OrderZoneTypes Types { get; set; }
        public StopKind StopKind { get; set; }
    }
}
