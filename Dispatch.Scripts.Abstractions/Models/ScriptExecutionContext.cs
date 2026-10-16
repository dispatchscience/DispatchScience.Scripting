using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public class ScriptExecutionContext
    {
        public string AccountId { get; set; } = default!;

        /// <summary>
        /// The account that placed a Bill To order. Null on a standard order. <see cref="AccountId"/> keeps meaning the billed account.
        /// </summary>
        public string? OrderingAccountId { get; set; }

        public int ScriptId { get; set; }
        public int? ScriptRuleId { get; set; }
        public int? ExtraFeeScheduleId { get; set; }
        public string? ExtraFeeTypeId { get; set; }
        public decimal? ExtraFeeQuantity { get; set; }
    }

    public interface IScriptExecutionContext
    {
        string AccountId { get; }

        /// <summary>
        /// The account that placed a Bill To order. Null on a standard order. <see cref="AccountId"/> keeps meaning the billed account.
        /// </summary>
        string? OrderingAccountId { get; }

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
