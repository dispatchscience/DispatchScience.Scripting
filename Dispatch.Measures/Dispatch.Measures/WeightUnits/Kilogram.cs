#nullable enable
namespace Dispatch.Measures
{
    public sealed class Kilogram: WeightUnit
    {
        public override double SIConversionFactor => 1d;
        public override string Symbol => "kg";
    }
}