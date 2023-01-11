#nullable enable
using System;

namespace Dispatch.Measures
{
    /// <summary>
    /// Represent a quantity of mass. This is called Weight for convenience because in everyday usage, mass and weight are used interchangeably
    /// </summary>
    public readonly struct Weight
    {
        private readonly double _value;
        private readonly WeightUnit? _unit; // the field may be null if the struct was not created with a constructor

        public Weight(double value, WeightUnit unit)
        {
            _value = value;
            _unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public double Value => _value;
        public WeightUnit Unit => _unit ?? WeightUnit.SIUnit;

        public Weight ConvertTo(WeightUnit unit)
        {
            return new Weight(Value * Unit.GetConversionFactorTo(unit), unit);
        }

        public override string ToString() => $"{Value:0.##}\u00A0{Unit.Symbol}";

        public static Weight operator +(Weight a, Weight b)
        {
            var newValue = a.Value + b.ConvertTo(a.Unit).Value;
            return new Weight(newValue, a.Unit);
        }

        public static Weight operator -(Weight a, Weight b)
        {
            var newValue = a.Value - b.ConvertTo(a.Unit).Value;
            return new Weight(newValue, a.Unit);
        }

        public static Weight operator /(Weight a, int b) => new Weight(a.Value / b, a.Unit);

        public static Weight FromKilograms(double kilograms) => new Weight(kilograms, WeightUnit.Kilogram);

        public void Deconstruct(out double value, out WeightUnit unit)
        {
            value = Value;
            unit = Unit;
        }
    }
}