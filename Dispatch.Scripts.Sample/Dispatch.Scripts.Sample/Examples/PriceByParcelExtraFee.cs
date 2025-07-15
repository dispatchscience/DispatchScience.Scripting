using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class PriceByParcelExtraFee : IExtraFeeScript
    {
        //Originally created for tenant Mailpak
        //Calculate an extra fee based on all the parcels on the order. Each parcel type can have a different price.
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var sheetValues = await data.GetSheet();

            var priceParcels = sheetValues.GroupBy(x => x.RowName);

            var priceByParcelTypes = new List<(string ParcelTypeId, decimal Price, decimal Extra)>();
            foreach (var parcelTypeValues in priceParcels)
            {
                var parcelTypeId = parcelTypeValues.Key;
                var extra = decimal.Parse(sheetValues.FirstOrDefault(x => x.RowName == parcelTypeId && string.Equals(x.ColumnName, "extra", StringComparison.InvariantCultureIgnoreCase))?.CellValue);
                var price = decimal.Parse(sheetValues.FirstOrDefault(x => x.RowName == parcelTypeId && string.Equals(x.ColumnName, "price", StringComparison.InvariantCultureIgnoreCase))?.CellValue);
                priceByParcelTypes.Add((parcelTypeId, price, extra));
            }

            decimal totalPrice = 0;
            foreach (var item in priceByParcelTypes.OrderByDescending(x => x.Price))
            {
                var orderItemsOfParcelType = order.OrderItemInfos.Where(x => string.Equals(x.ParcelTypeId, item.ParcelTypeId, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (orderItemsOfParcelType.Any())
                {
                    if (totalPrice == 0)
                    {
                        totalPrice = item.Price;
                        totalPrice += item.Extra * (orderItemsOfParcelType.Count - 1);
                    }
                    else
                    {
                        totalPrice += item.Extra * orderItemsOfParcelType.Count;
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
