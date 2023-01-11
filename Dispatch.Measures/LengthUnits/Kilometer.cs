#nullable enable
namespace Dispatch.Measures
{
    public sealed class Kilometer: LengthUnit
    {
        public override double SIConversionFactor => 1_000d;
        public override string Symbol => "km";
    }
}