#nullable enable
namespace Dispatch.Measures
{
    public sealed class Meter: LengthUnit
    {
        public override double SIConversionFactor => 1d;
        public override string Symbol => "m";
    }
}