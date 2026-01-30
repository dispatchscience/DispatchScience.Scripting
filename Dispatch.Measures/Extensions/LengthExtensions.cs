#nullable enable
using System;
using static System.Math;

namespace Dispatch.Measures
{
    public static class LengthExtensions
    {
        public static double ToValue(this Length length, LengthUnit unit, int? decimals = null) => decimals.HasValue
            ? Round(length.ConvertTo(unit).Value, decimals.Value, MidpointRounding.AwayFromZero)
            : length.ConvertTo(unit).Value;

        public static double ToValue(this Length length, int decimals) => Round(length.Value, decimals, MidpointRounding.AwayFromZero);

        public static double Meters(this Length length, int? decimals = null) => decimals.HasValue
            ? Round(length.ConvertTo(LengthUnit.Meter).Value, decimals.Value, MidpointRounding.AwayFromZero)
            : length.ConvertTo(LengthUnit.Meter).Value;

        public static double Kilometers(this Length length, int? decimals = null) => decimals.HasValue
            ? Round(length.ConvertTo(LengthUnit.Kilometer).Value, decimals.Value, MidpointRounding.AwayFromZero)
            : length.ConvertTo(LengthUnit.Kilometer).Value;

        public static double Feet(this Length length, int? decimals = null) => decimals.HasValue
            ? Round(length.ConvertTo(LengthUnit.Foot).Value, decimals.Value, MidpointRounding.AwayFromZero)
            : length.ConvertTo(LengthUnit.Foot).Value;

        public static double Miles(this Length length, int? decimals = null) => decimals.HasValue
            ? Round(length.ConvertTo(LengthUnit.Mile).Value, decimals.Value, MidpointRounding.AwayFromZero)
            : length.ConvertTo(LengthUnit.Mile).Value;
    }
}