using Dispatch.Measures;
using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.AddExtraFeeForLongDistance
{
    public class AddExtraFeeForLongDistance : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (!order.PickupAddress.Location.IsValid || !order.DeliveryAddress.Location.IsValid)
            {
                return;
            }

            var pickupZones = await data.GetZones(ScriptZoneType.Script, order.PickupAddress.Location);
            var deliveryZones = await data.GetZones(ScriptZoneType.Script, order.DeliveryAddress.Location);

            var validZones = new string[] { "1_00", "2_00", "3_00", "4_00", "5_00", "6_00", "700_00" };

            var pickupHasValidZone = pickupZones.Any(z => validZones.Contains(z));
            var deliveryHasValidZone = deliveryZones.Any(z => validZones.Contains(z));

            if (pickupHasValidZone && deliveryHasValidZone)
            {
                var over30 = 0m;
                var over40 = 0m;
                var orderDistance = Convert.ToDecimal(order.Distance.Value);

                if (!order.Attributes.Any(x => x.ToLowerInvariant() == "noextramileagefee"))
                {
                    if (orderDistance > 40 && order.Attributes.Any(x => x.ToLowerInvariant() == "billover40"))
                    {
                        over40 = orderDistance;
                    }
                    else if (orderDistance > 30)
                    {
                        over30 = orderDistance;
                    }
                }

                var over40MilesExtra = order.ExtraFees.FirstOrDefault(e => string.Equals(e.ExtraFeeTypeId, "over40miles", StringComparison.InvariantCultureIgnoreCase));
                if (over40MilesExtra is null)
                {
                    if (over40 > 0)
                    {
                        await order.AddExtraFee("Over40Miles", over40);
                    }
                }
                else
                {
                    await order.UpdateExtraFee(over40MilesExtra.ChargeId, over40);
                }

                var over30MilesExtra = order.ExtraFees.FirstOrDefault(e => string.Equals(e.ExtraFeeTypeId, "over30miles", StringComparison.InvariantCultureIgnoreCase));
                if (over30MilesExtra is null)
                {
                    if (over30 > 0)
                    {
                        await order.AddExtraFee("Over30Miles", over30);
                    }
                }
                else
                {
                    await order.UpdateExtraFee(over30MilesExtra.ChargeId, over30);
                }
            }
        }
    }
}
