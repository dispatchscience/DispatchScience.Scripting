#nullable enable
namespace Dispatch.Measures
{
    public sealed class Foot: LengthUnit
    {
        public override double SIConversionFactor => 0.3048d;
        public override string Symbol => "ft";

        public override double GetConversionFactorTo(LengthUnit otherUnit) => otherUnit switch
        {
            Inch _ => 12d,
            Mile _ => 1 / 5_280d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}