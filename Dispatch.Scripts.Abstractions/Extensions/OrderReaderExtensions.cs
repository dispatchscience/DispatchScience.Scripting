using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public static class OrderReaderExtensions
    {
        public static bool IsFirstSegment(this IOrderReader order) => order.IsNthSegment(1);
        public static bool IsSecondSegment(this IOrderReader order) => order.IsNthSegment(2);
        public static bool IsNthSegment(this IOrderReader order, int position) => order.OrderId.EndsWith($".{position}");
        public static bool IsStandardOrder(this IOrderReader order) => order.FulfillmentType == OrderFulfillmentType.Standard;
        public static bool IsMultiSegmentOrder(this IOrderReader order) => order.FulfillmentType == OrderFulfillmentType.MultiSegment;
        public static bool IsSegmentOrder(this IOrderReader order) => order.FulfillmentType == OrderFulfillmentType.Segment;
        public static int? SegmentNumber(this IOrderReader order) => order.IsSegmentOrder() ? order.OrderId.SegmentNumber() : null;

        public static int? SegmentNumber(this string orderId) => orderId.Contains('.') 
            ? (int.TryParse(orderId[(orderId.LastIndexOf('.') + 1)..], out int intValue) ? intValue : null) 
            : null;

        public static async Task<int?> LastSegmentNumber(this IOrderReader order)
        {
            if (!order.IsMultiSegmentOrder())
            {
                return null;
            }

            var segmentOrders = await order.GetSegmentOrders();
            return segmentOrders.Max(x => x.SegmentNumber());
        }

        public static async Task<string?> LastSegmentOrderId(this IOrderReader order)
        {
            var lastSegmentNumber = await order.LastSegmentNumber();
            return lastSegmentNumber.HasValue ? $"{order.OrderId}.{lastSegmentNumber}" : null;
        }
    }
}