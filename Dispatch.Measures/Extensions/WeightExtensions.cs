#nullable enable
namespace Dispatch.Measures
{
    public static class WeightExtensions
    {
        public static double Pounds(this Weight weight) => weight.ConvertTo(WeightUnit.Pound).Value;

        public static double Kilograms(this Weight weight) => weight.ConvertTo(WeightUnit.Kilogram).Value;
    }
}