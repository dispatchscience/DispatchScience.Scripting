namespace Dispatch.Scripts
{
    public class ScriptExecutionContext
    {
        public string AccountId { get; set; } = default!;
        public int ScriptId { get; set; }
        public int? ScriptRuleId { get; set; }
        public int? ExtraFeeScheduleId { get; set; }
        public string? ExtraFeeTypeId { get; set; }
    }
}
