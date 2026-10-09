using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.ExtraFeeScript
{
    public class DefaultExtraFeeScript : IExtraFeeScript
    {
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            // This line is needed in case you don't need the file values (otherwise you will get an async/await compilation error.
            await Task.CompletedTask;

            // You can access the file (if you uploaded on in the Extra Fee) using the example below.
            // If your file contains a single sheet, you can get the values using the following method:
            //var sheetValues = await data.GetSheet();

            // Otherwise, you need to specify the sheet name:
            //var sheetValues = await data.GetSheet(""my sheet"");
            //var cellsForColumn = sheetValues.Where(x => x.ColumnName == ""Car"");
            //var cell = cellsForColumn.FirstOrDefault(x => x.RowName == ""5"");
            //if (cell != null)
            //{
            //    return new ExtraFeeScriptResult
            //    {
            //        Quantity = 2,
            //        UnitPrice = cell.CellValue
            //    };
            //}

            return new ExtraFeeScriptResult
            {
                Quantity = 1,
                UnitPrice = 0
            };
        }
    }
}
