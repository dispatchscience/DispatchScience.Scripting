namespace Dispatch.Scripts
{
    public class OrderDeliveryChargeInfo
    {
        public string ChargeId { get; set; } = default!;
        public bool IsReattempt { get; set; }
        public decimal Price { get; set; }
        public decimal FuelSurcharge { get; set; }
        public decimal TotalPrice { get; set; }
        public string? PickupZone { get; set; }
        public string? DeliveryZone { get; set; }
        public bool? IsPriceOverridden { get; set; }
    }
}