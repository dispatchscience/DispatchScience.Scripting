#nullable enable
using System;

namespace Dispatch.Measures
{
    /// <summary>
    /// Unit of mass. This is called WeightUnit for convenience because in everyday usage, mass and weight are used interchangeably
    /// </summary>
    public abstract class WeightUnit: UnitOfMeasure<WeightUnit>
    {
        static WeightUnit()
        {
            Kilogram = new Kilogram();
            Gram = new Gram();
            Pound = new Pound();
            Ounce = new Ounce();
        }
        public static WeightUnit SIUnit  => Kilogram;
        public static WeightUnit Kilogram { get; }
        public static WeightUnit Gram { get; }
        public static WeightUnit Pound { get; }
        public static WeightUnit Ounce { get; }
    }
}