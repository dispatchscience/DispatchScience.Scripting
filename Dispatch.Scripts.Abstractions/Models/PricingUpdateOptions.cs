namespace Dispatch.Scripts
{
    public class PricingUpdateOptions
    {
        public static PricingUpdateOptions Default => new PricingUpdateOptions();

        public bool UpdateDeliveryCharge { get; set; } = true;
        public bool UpdateWeightExtraFee { get; set; } = true;
        public bool UpdateNumberOfPiecesExtraFee { get; set; } = true;
        public bool UpdateMileageAffectedExtraFees { get; set; } = true;
        public bool UpdateVehicleAffectedExtraFees { get; set; } = true;
        public bool UpdateScriptedExtraFees { get; set; } = true;
    }
}