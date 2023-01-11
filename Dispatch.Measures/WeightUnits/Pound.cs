#nullable enable
namespace Dispatch.Measures
{
    public sealed class Pound: WeightUnit
    {
        public override double SIConversionFactor => 0.45359237d;
        public override string Symbol => "lb";

        public override double GetConversionFactorTo(WeightUnit otherUnit) => otherUnit switch
        {
            Ounce _ => 16d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}