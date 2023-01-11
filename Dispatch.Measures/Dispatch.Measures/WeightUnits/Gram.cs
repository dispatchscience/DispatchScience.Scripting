#nullable enable
namespace Dispatch.Measures
{
    public sealed class Gram: WeightUnit
    {
        public override double SIConversionFactor => 0.001d;
        public override string Symbol => "g";
    }
}