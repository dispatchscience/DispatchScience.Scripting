using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class AddTimeToDeliverOnOrderUpdate : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (order?.EventDates == null)
            {
                logger.LogInformation("No events");
                return;
            }

            if (!order.EventDates.Delivered.HasValue || !order.EventDates.PickedUp.HasValue)
            {
                logger.LogInformation("No events dates");
                return;
            }

            logger.LogInformation("Calculating time...");
            var delivered = order.EventDates.Delivered.Value;
            var pickup = order.EventDates.PickedUp.Value;

            var timeToDoDelivery = delivered.Subtract(pickup);
            var chargeIncrement = 15;
            var minutesToCharge = Convert.ToDecimal((int)(Math.Ceiling(timeToDoDelivery.TotalMinutes / chargeIncrement) * chargeIncrement));
            logger.LogInformation("Minutes to charge :" + minutesToCharge.ToString());
            if (minutesToCharge > 0)
            {
                var dailyHoursCharge = order.ExtraFees.FirstOrDefault(e => e.ExtraFeeTypeId == "DailyHours");
                if (dailyHoursCharge == null)
                {
                    logger.LogInformation("Added");
                    await order.AddExtraFee("DailyHours", minutesToCharge);
                }
                else
                {
                    logger.LogInformation("Updated");
                    await order.UpdateExtraFee(dailyHoursCharge.ChargeId, minutesToCharge);
                }
            }
        }
    }
}
