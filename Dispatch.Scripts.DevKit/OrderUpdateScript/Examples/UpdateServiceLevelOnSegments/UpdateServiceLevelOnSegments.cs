using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.UpdateServiceLevelOnSegments
{
    public class UpdateServiceLevelOnSegments : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (order.FulfillmentType == OrderFulfillmentType.Standard)
            {
                return;
            }

            IOrderUpdater multiSegmentOrder;
            if (order.FulfillmentType == OrderFulfillmentType.MultiSegment)
            {
                multiSegmentOrder = order;
            }
            else
            {
                multiSegmentOrder = await order.GetMultiSegmentOrder();
                await multiSegmentOrder.UpdateServiceLevel(order.ServiceLevelTypeId);
            }

            var segmentOrders = await multiSegmentOrder.GetSegmentOrders();
            foreach (var segmentOrder in segmentOrders)
            {
                await segmentOrder.UpdateServiceLevel(multiSegmentOrder.ServiceLevelTypeId);
            }
        }
    }
}
