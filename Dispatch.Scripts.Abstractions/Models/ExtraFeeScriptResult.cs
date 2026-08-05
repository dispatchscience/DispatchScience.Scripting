using System.Collections.Generic;

namespace Dispatch.Scripts
{
    public class ExtraFeeScriptResult
    {
        public decimal Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public ExtraFeeScriptResultDetails? Details { get; set; }
    }

    public class ExtraFeeScriptResultDetails
    {
        public string? TitlePrimary { get; set; }
        public string? TitleSecondary { get; set; }
        public string? DescriptionPrimary { get; set; }
        public string? DescriptionSecondary { get; set; }
        public IDictionary<string, object?>? Metadata { get; set; }
        public PriceBreakdownItem[]? PriceBreakdownItems { get; set; }
    }

    public class PriceBreakdownItem
    {
        public string? TitlePrimary { get; set; }
        public string? TitleSecondary { get; set; }
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasurePrimary { get; set; }
        public string? UnitOfMeasureSecondary { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}