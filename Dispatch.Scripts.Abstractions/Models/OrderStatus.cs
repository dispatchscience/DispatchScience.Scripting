namespace Dispatch.Scripts
{
    public enum OrderStatus
    {
        Draft = -1,
        Received = 0,
        Assigned = 1,
        PickedUp = 2,
        Delivered = 4,
        Cancelled = 91
    }
}