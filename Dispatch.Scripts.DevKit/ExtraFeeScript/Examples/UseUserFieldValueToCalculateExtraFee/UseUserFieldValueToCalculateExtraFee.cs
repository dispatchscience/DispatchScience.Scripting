using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.ExtraFeeScript.Examples.UseUserFieldValueToCalculateExtraFee
{
    public class UseUserFieldValueToCalculateExtraFee : IExtraFeeScript
    {
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var sheetValues = await data.GetSheet();

            var userField = sheetValues.Where(x => x.ColumnName is not null && x.ColumnName.ToLowerInvariant() != "qty").FirstOrDefault();

            var uf = userField is not null 
                ? order.UserFields.FirstOrDefault(uf => uf.UserFieldId.ToLowerInvariant() == userField.ColumnName!.ToLowerInvariant())
                : default;

            if (uf.Value is not null)
            {
                if (decimal.TryParse(uf.Value, out decimal cod))
                {
                    decimal price = 0;
                    foreach (var item in sheetValues.Where(x => x.ColumnName?.ToLowerInvariant() == userField!.ColumnName?.ToLowerInvariant()).OrderBy(c => int.Parse(c.RowName!)))
                    {
                        if (int.Parse(item.RowName) <= cod)
                        {
                            price = decimal.Parse(item.CellValue);
                        }
                    }
                    return new ExtraFeeScriptResult { Quantity = cod, UnitPrice = price };
                }
            }

            return new ExtraFeeScriptResult { Quantity = 0, UnitPrice = 0 };
        }
    }
}
