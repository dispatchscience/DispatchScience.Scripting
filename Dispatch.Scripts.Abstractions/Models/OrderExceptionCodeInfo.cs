namespace Dispatch.Scripts
{
    public class OrderExceptionCodeInfo
    {
        public string ExceptionCodeId { get; set; }
        public string ItemId { get; set; }
        public string ItemBarcode { get; set; }
        public string SegmentOrderId { get; set; }
        public OrderExceptionCodeAddedAt AddedAt { get; set; }
    }
}
