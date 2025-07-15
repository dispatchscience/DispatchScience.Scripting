namespace Dispatch.Scripts
{
    public class OrderExtraFeeInfo
    {
        public string ChargeId { get; set; } = default!;
        public string ExtraFeeTypeId { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Price { get; set; }
        public decimal FuelSurcharge { get; set; }
        public decimal TotalPrice { get; set; }
    }
}