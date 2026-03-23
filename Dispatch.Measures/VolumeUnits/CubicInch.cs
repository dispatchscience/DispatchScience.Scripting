#nullable enable
namespace Dispatch.Measures
{
    public sealed class CubicInch: VolumeUnit
    {
        public CubicInch(): base(1/61023.7, "in³")
        {
        }

        public override double GetConversionFactorTo(VolumeUnit otherUnit) => otherUnit switch
        {
            CubicFoot _ => 1/1728d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}