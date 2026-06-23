using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.AddTimeToDeliverOnOrderUpdate
{
    public class AddTimeToDeliverOnOrderUpdate : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var pickedUpDate = order.EventDates.PickedUp;
            var deliveredDate = order.EventDates.Delivered;

            if (!pickedUpDate.HasValue || !deliveredDate.HasValue)
            {
                logger.LogInformation("No events dates");
                return;
            }

            logger.LogInformation("Calculating time...");
            var pickup = pickedUpDate.Value;
            var delivered = deliveredDate.Value;

            var timeToDoDelivery = delivered.Subtract(pickup);
            var chargeIncrement = 15;
            var minutesToCharge = Convert.ToDecimal((int)(Math.Ceiling(timeToDoDelivery.TotalMinutes / chargeIncrement) * chargeIncrement));
            logger.LogInformation($"Minutes to charge: {minutesToCharge}");
            if (minutesToCharge > 0)
            {
                var dailyHoursCharge = order.ExtraFees.FirstOrDefault(e => e.ExtraFeeTypeId == "DailyHours");
                if (dailyHoursCharge is null)
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
