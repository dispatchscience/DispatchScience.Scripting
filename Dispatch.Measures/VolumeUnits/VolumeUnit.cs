#nullable enable
using Newtonsoft.Json;
using System;

namespace Dispatch.Measures
{
    public class VolumeUnit: UnitOfMeasure<VolumeUnit>
    {
        [JsonConstructor]
        protected VolumeUnit(double siConversionFactor, string symbol)
        {
            SIConversionFactor = siConversionFactor;
            Symbol = symbol;
        }

        public override double SIConversionFactor { get; }
        public override string Symbol { get; }

        static VolumeUnit()
        {
            CubicMeter = new CubicMeter();
            Liter = new Liter();
            CubicFoot = new CubicFoot();
            CubicInch = new CubicInch();
        }

        public static VolumeUnit SIUnit  => CubicMeter;
        public static VolumeUnit CubicMeter { get; }
        public static VolumeUnit Liter { get; }
        public static VolumeUnit CubicFoot { get; }
        public static VolumeUnit CubicInch { get; }

        public static VolumeUnit From(LengthUnit lengthUnit)
        {
            return lengthUnit switch
            {
                Meter _ => CubicMeter,
                Foot _ => CubicFoot,
                Inch _ => CubicInch,
                _ => new VolumeUnit(Math.Pow(lengthUnit.SIConversionFactor, 3), lengthUnit.Symbol + "³")
            };
        }
    }
}