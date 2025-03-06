#nullable enable
namespace Dispatch.Measures
{
    public sealed class CubicFoot: VolumeUnit
    {
        public CubicFoot(): base(0.0283168d, "ft³")
        {
        }

        public override double GetConversionFactorTo(VolumeUnit otherUnit) => otherUnit switch
        {
            CubicInch _ => 1728d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}