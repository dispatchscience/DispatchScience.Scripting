using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class AddExtraFeeForLongDistance : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (order.PickupAddress == null || order.DeliveryAddress == null)
            {
                return;
            }
            var pickupZone = await data.GetZones(ScriptZoneType.Script, order.PickupAddress.Location);
            var deliveryZone = await data.GetZones(ScriptZoneType.Script, order.DeliveryAddress.Location);
            var validZones = new string[] { "1_00", "2_00", "3_00", "4_00", "5_00", "6_00", "700_00" };
            decimal over40 = 0;
            decimal over30 = 0;
            if (validZones.Contains(pickupZone.FirstOrDefault()) && validZones.Contains(deliveryZone.FirstOrDefault()))
            {
                if (!order.Attributes.Any(x => x.ToLower() == "noextramileagefee"))
                {
                    if (order.Distance.Value > 40 && order.Attributes.Any(x => x.ToLower() == "billover40"))
                    {
                        over40 = 1;
                    }
                    else if (order.Distance.Value > 30)
                    {
                        over30 = 1;
                        return;
                    }
                }

                var over40MilesExtra = order.ExtraFees.FirstOrDefault(e => string.Equals(e.ExtraFeeTypeId, "over40miles", StringComparison.InvariantCultureIgnoreCase));
                if (over40MilesExtra == null)
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
                if (over30MilesExtra == null)
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

                await order.UpdateExtraFee(over30MilesExtra.ChargeId, Convert.ToDecimal(order.Distance.Value));
                await order.UpdateExtraFee(over40MilesExtra.ChargeId, Convert.ToDecimal(order.Distance.Value));
            }
        }
    }
}