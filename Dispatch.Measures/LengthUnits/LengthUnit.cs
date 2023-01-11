#nullable enable
using System;

namespace Dispatch.Measures
{
    public abstract class LengthUnit: UnitOfMeasure<LengthUnit>
    {
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