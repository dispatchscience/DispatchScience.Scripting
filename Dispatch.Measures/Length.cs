#nullable enable
using System;

namespace Dispatch.Measures
{
    public readonly struct Length
    {
        private readonly double _value;
        private readonly LengthUnit? _unit; // the field may be null if the struct was not created with a constructor

        public Length(double value, LengthUnit unit)
        {
            _value = value;
            _unit = unit ?? throw new ArgumentNullException(nameof(unit));
        }

        public double Value => _value;
        public LengthUnit Unit => _unit ?? LengthUnit.SIUnit;

        public Length ConvertTo(LengthUnit unit)
        {
            return new Length(Value * Unit.GetConversionFactorTo(unit), unit);
        }

        public override string ToString() => $"{Value:0.##}\u00A0{Unit.Symbol}";

        public static Length FromKilometers(double kilometers) => new Length(kilometers, LengthUnit.Kilometer);
        public static Length FromMeters(double meters) => new Length(meters, LengthUnit.Meter);
        public static Length FromCm(double centimeters) => new Length(centimeters, LengthUnit.Centimeter);
        
        public static Length FromMiles(double miles) => new Length(miles, LengthUnit.Mile);
        public static Length FromFeet(double feet) => new Length(feet, LengthUnit.Foot);
        public static Length FromInches(double inches) => new Length(inches, LengthUnit.Inch);

        public static Length operator +(Length a, Length b)
        {
            var newValue = a.Value + b.ConvertTo(a.Unit).Value;
            return new Length(newValue, a.Unit);
        }

        public static Length operator -(Length a, Length b)
        {
            var newValue = a.Value - b.ConvertTo(a.Unit).Value;
            return new Length(newValue, a.Unit);
        }

        public static Length operator /(Length a, int b) => new Length(a.Value / b, a.Unit);

        public void Deconstruct(out double value, out LengthUnit unit)
        {
            value = Value;
            unit = Unit;
        }
    }
}