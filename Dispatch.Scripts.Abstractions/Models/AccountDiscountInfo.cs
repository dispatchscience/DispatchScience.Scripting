namespace Dispatch.Scripts
{
    public class AccountDiscountInfo
    {
        public bool IsSurcharge { get; set; }
        public string DisplayNamePrimary { get; set; } = default!;
        public string? DisplayNameSecondary { get; set; }
        public string? DescriptionPrimary { get; set; }
        public string? DescriptionSecondary { get; set; }
        public bool IsPercentage { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountPercentage { get; set; }
    }
}
