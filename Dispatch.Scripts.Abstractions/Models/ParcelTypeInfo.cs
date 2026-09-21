using Dispatch.Measures;
using System;

namespace Dispatch.Scripts
{
    public class ParcelTypeInfo
    {
        public string Id { get; set; } = default!;

        public bool IsCustomizable { get; set; }

        public string NamePrimary { get; set; } = default!;

        public string? NameSecondary { get; set; }

        public bool IsDimensionalWeight { get; set; }

        public Length Length { get; set; } = default!;

        public Length Width { get; set; } = default!;

        public Length Height { get; set; } = default!;

        public Weight Weight { get; set; } = default!;

        public Weight? BaseDimensionalWeight { get; set; }

        public decimal DimensionalFactor { get; set; }

        public decimal TotalUnitFactor { get; set; }

        public Weight? GetDimensionalWeight(OrderItemInfo item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!item.ParcelTypeId.Equals(Id, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Item is not of the same parcel type");
            }

            if (!IsDimensionalWeight)
            {
                return null;
            }

            if (DimensionalFactor <= 0)
            {
                return new Weight(0, Weight.Unit);
            }

            return new Weight((item.Width.Value * item.Height.Value * item.Length.Value) / (double)DimensionalFactor, item.Weight.Unit);
        }
    }
}