#nullable enable
namespace Dispatch.Measures
{
    public sealed class Ounce : WeightUnit
    {
        public Ounce() : base(0.0283495d, "oz")
        {
        }

        public override double GetConversionFactorTo(WeightUnit otherUnit) => otherUnit switch
        {
            Pound _ => 0.0625d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}