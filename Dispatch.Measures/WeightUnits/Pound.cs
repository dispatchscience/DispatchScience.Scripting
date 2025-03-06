#nullable enable
namespace Dispatch.Measures
{
    public sealed class Pound: WeightUnit
    {
        public Pound() : base(0.45359237d, "lb")
        {
        }

        public override double GetConversionFactorTo(WeightUnit otherUnit) => otherUnit switch
        {
            Ounce _ => 16d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}