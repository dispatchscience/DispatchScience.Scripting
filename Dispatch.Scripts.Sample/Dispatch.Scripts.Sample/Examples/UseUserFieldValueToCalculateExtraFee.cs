using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class UseUserFieldValueToCalculateExtraFee : IExtraFeeScript
    {
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var sheetValues = await data.GetSheet();

            var userField = sheetValues.Where(x => x.ColumnName.ToLower() != "qty").FirstOrDefault();

            if (order.UserFields.Any(uf => uf.UserFieldId.ToLower() == userField.ColumnName.ToLower()))
            {
                var uf = order.UserFields.First(uf => uf.UserFieldId.ToLower() == userField.ColumnName.ToLower());
                if (decimal.TryParse(uf.Value, out decimal cod))
                {
                    decimal price = 0;
                    foreach (var item in sheetValues.Where(x => x.ColumnName.ToLower() == userField.ColumnName.ToLower()).OrderBy(c => int.Parse(c.RowName)))
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
