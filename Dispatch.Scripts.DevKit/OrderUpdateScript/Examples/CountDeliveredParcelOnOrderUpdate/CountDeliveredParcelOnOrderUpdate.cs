using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.CountDeliveredParcelOnOrderUpdate
{
    public class CountDeliveredParcelOnOrderUpdate : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var fields = await data.GetAccountFields();
            string barcodeValidation = "";
            if (fields.Any(x => string.Equals(x.Key, "DDUBarcodeValidation", StringComparison.InvariantCultureIgnoreCase)))
            {
                barcodeValidation = fields.First(x => string.Equals(x.Key, "DDUBarcodeValidation", StringComparison.InvariantCultureIgnoreCase)).Value?.Trim();
            }
            else
            {
                return;
            }

            var pricesInfoData = await data.GetScriptData();
            decimal? price = null;
            decimal? minimum = null;

            if (pricesInfoData.ContainsKey($"{order.ServiceLevelTypeId}_Price") && pricesInfoData.ContainsKey($"{order.ServiceLevelTypeId}_Minimum"))
            {
                price = decimal.Parse(pricesInfoData[$"{order.ServiceLevelTypeId}_Price"]);
                minimum = decimal.Parse(pricesInfoData[$"{order.ServiceLevelTypeId}_Minimum"]);
            }
            else
            {
                return;
            }

            var deliveredBarcodes = order.TrackedItems.Where(x => x.Status == OrderItemTrackingStatus.Scanned
                                                                        && x.TrackingType == OrderItemTrackingType.Delivered
                                                                        && order.OrderItems.Any(y => string.Equals(y.BarcodeTemplate?.Trim(), x.BarcodeTemplate?.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                                                        .GroupBy(x => x.BarcodeTemplate?.Trim())
                                                        .Select(x => x.Key)
                                                        .Where(x => Regex.IsMatch(x, barcodeValidation))
                                                        .ToList();

            var manualScans = order.OrderItems.Where(x => x.UserFields.Any(y => string.Equals(y.UserFieldId, "DDUNoDelivery", StringComparison.InvariantCultureIgnoreCase) && 
                                                                                string.Equals(y.UserFieldId, true.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                                                        .GroupBy(x => x.BarcodeTemplate?.Trim())
                                                        .Select(x => x.Key)
                                                        .Where(x => Regex.IsMatch(x, barcodeValidation))
                                                        .ToList();

            foreach (var manualScan in manualScans)
            {
                if (!deliveredBarcodes.Contains(manualScan))
                {
                    deliveredBarcodes.Add(manualScan);
                }
            }

            var deliveredParcelCount = order.ExtraFees.FirstOrDefault(e => e.ExtraFeeTypeId == "DDUItem");
            var total = deliveredBarcodes.Count * price.Value;
            if (total < minimum)
            {
                price = minimum / deliveredBarcodes.Count; 
            }

            if (deliveredParcelCount == null)
            {                
                await order.AddExtraFee("DDUItem", deliveredBarcodes.Count, price);
            }
            else
            {
                await order.UpdateExtraFee(deliveredParcelCount.ChargeId, deliveredBarcodes.Count, price);
            }
        }
    }
}
