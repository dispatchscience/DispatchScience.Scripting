#nullable enable
namespace Dispatch.Measures
{
    public sealed class Centimeter: LengthUnit
    {
        public override double SIConversionFactor => 0.01d;
        public override string Symbol => "cm";
    }
}