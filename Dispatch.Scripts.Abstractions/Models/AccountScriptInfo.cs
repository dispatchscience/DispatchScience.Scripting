namespace Dispatch.Scripts
{
    public class AccountScriptInfo
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Number { get; set; } = default!;
        public int CompanyId { get; set; }
        public string? Notes { get; set; }
        public int? ServiceLevelScheduleId { get; set; }
        public int? ExtraFeeScheduleId { get; set; }
    }
}