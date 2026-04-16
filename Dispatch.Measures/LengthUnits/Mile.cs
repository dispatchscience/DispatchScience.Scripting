#nullable enable
namespace Dispatch.Measures
{
    public sealed class Mile: LengthUnit
    {
        public Mile() : base(1_609.34d, "mi")
        {
        }

        public override double GetConversionFactorTo(LengthUnit otherUnit) => otherUnit switch
        {
            Foot _ => 5_280d,
            _ => base.GetConversionFactorTo(otherUnit)
        };
    }
}