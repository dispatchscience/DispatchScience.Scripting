#nullable enable
using Newtonsoft.Json;

namespace Dispatch.Measures
{
    public class LengthUnit: UnitOfMeasure<LengthUnit>
    {
        [JsonConstructor]
        protected LengthUnit(double siConversionFactor, string symbol)
        {
            SIConversionFactor = siConversionFactor;
            Symbol = symbol;
        }

        public override double SIConversionFactor { get; }
        public override string Symbol { get; }

        static LengthUnit()
        {
            Meter = new Meter();
            Centimeter = new Centimeter();
            Kilometer = new Kilometer();
            Inch = new Inch();
            Foot = new Foot();
            Mile = new Mile();
        }

        public static LengthUnit SIUnit  => Meter;
        public static LengthUnit Meter { get; }
        public static LengthUnit Centimeter { get; }
        public static LengthUnit Kilometer { get; }
        public static LengthUnit Inch { get; }
        public static LengthUnit Foot { get; }
        public static LengthUnit Mile { get; }
    }
}