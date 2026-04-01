using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public class ScriptExecutionContext
    {
        public string AccountId { get; set; } = default!;
        public int ScriptId { get; set; }
        public int? ScriptRuleId { get; set; }
        public int? ExtraFeeScheduleId { get; set; }
        public string? ExtraFeeTypeId { get; set; }
        public decimal? ExtraFeeQuantity { get; set; }
    }

    public interface IScriptExecutionContext
    {
        string AccountId { get; }
        int ScriptId { get; }

        IOrderUpdateScriptExecutionContext? OrderUpdate { get; }
        IExtraFeeScriptExecutionContext? ExtraFee { get; }
    }

    public interface IOrderUpdateScriptExecutionContext
    {
        int ScriptRuleId { get; }
        public IDictionary<string, string> Data { get; }
    }

    public interface IExtraFeeScriptExecutionContext
    {
        int ExtraFeeScheduleId { get; }
        string ExtraFeeTypeId { get; }
        decimal Quantity { get; }

        /// <summary>
        /// Metadata that was previously set on the extra fee.
        /// </summary>
        Task<IDictionary<string, object?>?> GetExistingMetadata();
    }
}
