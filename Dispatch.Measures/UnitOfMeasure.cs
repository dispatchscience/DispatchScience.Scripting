#nullable enable
using System;

namespace Dispatch.Measures
{
    public abstract class UnitOfMeasure<T> where T: UnitOfMeasure<T>
    {
        public abstract double SIConversionFactor { get; }
        public abstract string Symbol { get; }

        public virtual double GetConversionFactorTo(T otherUnit)
        {
            if (otherUnit is null)
            {
                throw new ArgumentNullException(nameof(otherUnit));
            }

            if (otherUnit.GetType() == GetType())
            {
                return 1;
            }

            return SIConversionFactor / otherUnit.SIConversionFactor;
        }
    }
}