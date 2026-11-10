#nullable enable
using System;

namespace Dispatch.Measures
{
    public readonly struct Volume
    {
        private readonly double _value;
        private readonly VolumeUnit? _unit; // the field may be null if the struct was not created with a constructor

        public Volume(double value, VolumeUnit unit)
        {
            _value = value;
            _unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public double Value => _value;
        public VolumeUnit Unit => _unit ?? VolumeUnit.SIUnit;

        public Volume ConvertTo(VolumeUnit unit)
        {
            return new Volume(Value * Unit.GetConversionFactorTo(unit), unit);
        }

        public override string ToString() => $"{Value:0.##}\u00A0{Unit.Symbol}";

        public static Volume operator +(Volume a, Volume b)
        {
            var newValue = a.Value + b.ConvertTo(a.Unit).Value;
            return new Volume(newValue, a.Unit);
        }

        public static Volume operator -(Volume a, Volume b)
        {
            var newValue = a.Value - b.ConvertTo(a.Unit).Value;
            return new Volume(newValue, a.Unit);
        }

        public static bool operator >(Volume a, Volume b)
        {
            return a.Value > b.ConvertTo(a.Unit).Value;
        }

        public static bool operator <(Volume a, Volume b)
        {
            return a.Value < b.ConvertTo(a.Unit).Value;
        }

        public static Volume operator /(Volume a, int b) => new Volume(a.Value / b, a.Unit);

        public static Volume FromBox(double lengthX, double lengthY, double lengthZ, LengthUnit lengthUnit)
        {
            if (lengthX < 0 || lengthY < 0 || lengthZ < 0)
            {
                throw new ArgumentException("All lengths must be >= 0");
            }

            if (lengthUnit is null)
            {
                throw new ArgumentNullException(nameof(lengthUnit));
            }

            return new Volume(lengthX * lengthY * lengthZ, VolumeUnit.From(lengthUnit));
        }

        public void Deconstruct(out double value, out VolumeUnit unit)
        {
            value = Value;
            unit = Unit;
        }
    }
}