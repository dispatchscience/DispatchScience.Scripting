#nullable enable
namespace Dispatch.Measures
{
    public sealed class Inch: LengthUnit
    {
        public override double SIConversionFactor => 0.0254d;
        public override string Symbol => "in";

        public override double GetConversionFactorTo(LengthUnit otherUnit) => otherUnit switch
        {
            Foot _ => 1/12d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}