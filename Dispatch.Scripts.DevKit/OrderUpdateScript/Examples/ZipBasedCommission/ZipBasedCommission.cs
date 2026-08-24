using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.ZipBasedCommission
{
    public class ZipBasedCommission : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var zipCommissions = await data.GetScriptData();
            if (!zipCommissions.Any())
            {
                logger.LogInformation("No commission data is available");
                return;
            }

            if (string.IsNullOrWhiteSpace(order.ReferenceNumber1) || order.ReferenceNumber1.Length != 5)
            {
                logger.LogInformation($"Reference 1 doesn't contain a zip code: '{order.ReferenceNumber1}'");
                return;
            }

            var matchingCommission = zipCommissions.FirstOrDefault(x => string.Equals(x.Key, order.ReferenceNumber1, StringComparison.InvariantCultureIgnoreCase));
            if (matchingCommission.Key is null)
            {
                logger.LogInformation($"Commission not found for zip (ref1) '{order.ReferenceNumber1}'");
                return;
            }

            if (decimal.TryParse(matchingCommission.Value, out decimal commission))
            {
                var extraFeePieces = order.ExtraFees.FirstOrDefault(x => x.ExtraFeeTypeId == "NumberOfPieces");
                if (extraFeePieces != null)
                {
                    var commissionOnExtraFees = extraFeePieces.Price * commission;
                    var commissionOnFuelSurcharge = extraFeePieces.FuelSurcharge * commission;

                    var commissionAmount = commissionOnExtraFees + commissionOnFuelSurcharge;
                    if (order.AssignedDriver is null)
                    {
                        await order.Payout.Delivery.ChangeToFlatRate(0);
                        await order.Payout.ExtraFees.ChangeToFlatRate(commissionOnExtraFees);
                        await order.Payout.FuelSurcharge.ChangeToFlatRate(commissionOnFuelSurcharge);
                    }
                    else
                    {
                        // to manually override the payouts, we have to give a value to each driver that are part of the payout
                        var manualPayouts = order.Payout.PayoutInfo.DriverPayouts
                            .Select(x => new ManualPayout
                            {
                                DriverId = x.DriverId,
                                Amount = x.DriverId == order.AssignedDriver.Id ? commissionAmount : 0
                            })
                            .ToArray();
                        await order.Payout.OverrideManually(manualPayouts);
                    }
                }
            }
        }
    }
}
