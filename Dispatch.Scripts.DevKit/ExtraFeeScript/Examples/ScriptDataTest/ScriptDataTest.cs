using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Dispatch.Scripts.DevKit.ExtraFeeScript.Examples.ScriptDataTest
{
    public class ScriptDataTest : IExtraFeeScript
    {
        public async Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger)
        {
            var scriptData = await data.GetSheet<ScriptData>("Weight");

            var first = scriptData.FirstOrDefault();
            Debug.Assert(first is not null);
            Debug.Assert(first.RowNumber == 2);

            Debug.Assert(first.NullString is null);
            Debug.Assert(first.NonNullString == string.Empty);

            Debug.Assert(first.NullStringArray is null);
            Debug.Assert(first.NullStringList is null);
            Debug.Assert(first.NonNullStringArray is not null && first.NonNullStringArray.Length == 0);
            Debug.Assert(first.NonNullStringList is not null && first.NonNullStringList.Count == 0);
            Debug.Assert(first.StringArray is not null && first.StringArray.Length == 2);
            Debug.Assert(first.StringList is not null && first.StringList.Count == 2);
            Debug.Assert(first.DoubleArray is not null && first.DoubleArray.Length == 2);
            Debug.Assert(first.DoubleArraySemicolon is not null && first.DoubleArraySemicolon.Length == 2);
            Debug.Assert(first.DoubleList is not null && first.DoubleList.Count == 2);

            return new ExtraFeeScriptResult
            {
                Quantity = 0,
                UnitPrice = 0
            };
        }

        public class ScriptData : IScriptData
        {
            public int RowNumber { get; set; }

            public string? NullString { get; set; }
            public string NonNullString { get; set; } = "";

            public string[]? NullStringArray { get; set; }
            public List<string>? NullStringList { get; set; }
            public string[] NonNullStringArray { get; set; }
            public List<string> NonNullStringList { get; set; } = new();
            public string[] StringArray { get; set; }
            public List<string> StringList { get; set; }
            public double[] DoubleArray { get; set; }
            [MappedProperty(ListSeparator = ';')]
            public double[] DoubleArraySemicolon { get; set; }
            public List<double> DoubleList { get; set; }
        }
    }
}
