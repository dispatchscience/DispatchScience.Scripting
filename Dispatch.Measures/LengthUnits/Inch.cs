#nullable enable
namespace Dispatch.Measures
{
    public sealed class Inch: LengthUnit
    {
        public Inch() : base(0.0254d, "in")
        {
        }

        public override double GetConversionFactorTo(LengthUnit otherUnit) => otherUnit switch
        {
            Foot _ => 1/12d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}