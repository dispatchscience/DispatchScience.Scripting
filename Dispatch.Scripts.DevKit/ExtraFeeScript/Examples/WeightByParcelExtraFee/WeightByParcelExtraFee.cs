using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.ExtraFeeScript.Examples.WeightByParcelExtraFee
{
    public class WeightByParcelExtraFee : IExtraFeeScript
    {        
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var priceSheet = await data.GetSheet();
            if (priceSheet.Length == 0 || order.OrderItemInfos.Count == 0)
            {
                return new ExtraFeeScriptResult { Quantity = 0, UnitPrice = 0 };
            }

            decimal? totalPrice  = 0;

            foreach (var item in order.OrderItemInfos)
            {
                var parcelTypeId = priceSheet.FirstOrDefault(s =>
                    string.Equals(s.ColumnName, "ParcelTypeId", StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(s.RowName , item.ParcelTypeId, StringComparison.InvariantCultureIgnoreCase));

                if (parcelTypeId is null || string.IsNullOrWhiteSpace(parcelTypeId.CellValue))
                {
                    continue;
                }

                var includedWeightValid = double.TryParse(priceSheet.FirstOrDefault(s => string.Equals(s.ColumnName, "IncludedWeight", StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(s.RowName, parcelTypeId.CellValue, StringComparison.InvariantCultureIgnoreCase))?.CellValue, out double includedWeight);

                if (includedWeightValid)
                {
                    var overweightPriceValid = decimal.TryParse(priceSheet.FirstOrDefault(s => string.Equals(s.ColumnName, "OverweightPrice", StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(s.RowName, parcelTypeId.CellValue, StringComparison.InvariantCultureIgnoreCase))?.CellValue, out decimal overweightPrice);

                    if (overweightPriceValid)
                    {
                        var billableWeight = item.Weight.Value - includedWeight;
                        if (billableWeight > 0)
                        {
                            totalPrice += Convert.ToDecimal(billableWeight) * overweightPrice;
                        }
                    }
                }
            }

            return new ExtraFeeScriptResult { Quantity = 1, UnitPrice = totalPrice };
        }
    }
}
