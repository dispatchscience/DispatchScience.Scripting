using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.ExtraFeeScript.Examples.PriceByParcelExtraFee
{
    public class PriceByParcelExtraFee : IExtraFeeScript
    {
        //Calculate an extra fee based on all the parcels on the order. Each parcel type can have a different price.
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var sheetValues = await data.GetSheet();

            var priceParcels = sheetValues.GroupBy(x => x.RowName);

            var priceByParcelTypes = new List<(string ParcelTypeId, decimal Price, decimal Extra)>();
            foreach (var parcelTypeValues in priceParcels)
            {
                var parcelTypeId = parcelTypeValues.Key;
                if (parcelTypeId is null)
                {
                    continue;
                }

                var valuesForParcelType = sheetValues.Where(x => x.RowName == parcelTypeId).ToArray();

                var extraCellValue = valuesForParcelType.FirstOrDefault(x => string.Equals(x.ColumnName, "extra", StringComparison.InvariantCultureIgnoreCase))?.CellValue;
                var priceCellValue = valuesForParcelType.FirstOrDefault(x => string.Equals(x.ColumnName, "price", StringComparison.InvariantCultureIgnoreCase))?.CellValue;

                decimal.TryParse(extraCellValue, out var extra);
                decimal.TryParse(priceCellValue, out var price);

                priceByParcelTypes.Add((parcelTypeId, price, extra));
            }

            var totalPrice = 0m;
            foreach (var item in priceByParcelTypes.OrderByDescending(x => x.Price))
            {
                var orderItemsOfParcelType = order.OrderItemInfos.Where(x => string.Equals(x.ParcelTypeId, item.ParcelTypeId, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                if (orderItemsOfParcelType.Any())
                {
                    if (totalPrice == 0)
                    {
                        totalPrice = item.Price;
                        totalPrice += item.Extra * (orderItemsOfParcelType.Length - 1);
                    }
                    else
                    {
                        totalPrice += item.Extra * orderItemsOfParcelType.Length;
                    }
                }
            }

            return new ExtraFeeScriptResult
            {
                Quantity = 1,
                UnitPrice = totalPrice
            };
        }
    }
}
