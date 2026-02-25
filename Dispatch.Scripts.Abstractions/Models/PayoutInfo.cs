using System;

namespace Dispatch.Scripts
{
    public class PayoutInfo
    {
        [Obsolete("Will be removed eventually, use the new properties instead.")]
        public decimal? PredefinedPayoutValue => DeliveryPredefinedPayoutValue;

        [Obsolete("Will be removed eventually, use the new properties instead.")]
        public DriverCommissionCalculationType DriverCommissionCalculationType => DeliveryCommissionCalculationType;

        public DriverCommissionCalculationType DeliveryCommissionCalculationType { get; set; }
        public DriverCommissionCalculationType FuelSurchargeCommissionCalculationType { get; set; }
        public DriverCommissionCalculationType ExtraFeesCommissionCalculationType { get; set; }

        public decimal? DeliveryPredefinedPayoutValue { get; set; }
        public decimal? FuelSurchargePredefinedPayoutValue { get; set; }
        public decimal? ExtraFeesPredefinedPayoutValue { get; set; }

        public decimal? DeliveryCommissionPercentage { get; set; }
        public decimal? FuelSurchargeCommissionPercentage { get; set; }
        public decimal? ExtraFeesCommissionPercentage { get; set; }

        public int? DeliveryFixedPayoutScheduleId { get; set; }
        public int? FuelSurchargeFixedPayoutScheduleId { get; set; }
        public int? ExtraFeesFixedPayoutScheduleId { get; set; }

        public bool IsSetManually { get; set; }

        public DriverPayout[] DriverPayouts { get; set; } = Array.Empty<DriverPayout>();

        public class DriverPayout
        {
            public string DriverId { get; set; } = default!;

            /// <summary>
            /// The date/time when the driver was added to the payout of the order.
            /// </summary>
            public DateTimeOffset? AddedDate { get; set; }

            /// <summary>
            /// The payout amount the driver will receive.
            /// </summary>
            public decimal ActualCommissionAmountTotal => ActualCommissionAmountOnDelivery + ActualCommissionAmountOnExtraFees + ActualCommissionAmountOnFuelSurcharge;

            // The commission amounts that makes the total payout amount.
            public decimal ActualCommissionAmountOnDelivery { get; set; }
            public decimal ActualCommissionAmountOnExtraFees { get; set; }
            public decimal ActualCommissionAmountOnFuelSurcharge { get; set; }

            // The commission percentages for the actual payout.
            public decimal ActualCommissionPercentageOnDelivery { get; set; }
            public decimal ActualCommissionPercentageOnExtraFees { get; set; }
            public decimal ActualCommissionPercentageOnFuelSurcharge { get; set; }

            // The commission amounts calculated from the driver's profile commission percentages.
            public decimal CalculatedCommissionAmountOnDelivery { get; set; }
            public decimal CalculatedCommissionAmountOnExtraFees { get; set; }
            public decimal CalculatedCommissionAmountOnFuelSurcharge { get; set; }

            // The commission percentages of the driver when he was added to the order.
            public decimal CalculatedCommissionPercentageOnDelivery { get; set; }
            public decimal CalculatedCommissionPercentageOnExtraFees { get; set; }
            public decimal CalculatedCommissionPercentageOnFuelSurcharge { get; set; }
            public (string ExtraFeeTypeId, decimal CommissionPercentage)[] ExtraFeeCommissionOverrides { get; set; } = Array.Empty<(string, decimal)>();

            public bool PayFuelSurchargeCommissionOnFlatRate { get; set; }

            [Obsolete("Will be removed eventually, use the new properties instead.")]
            public DriverCommissionCalculationType CalculationType => DeliveryCalculationType;

            public DriverCommissionCalculationType DeliveryCalculationType { get; set; }
            public DriverCommissionCalculationType ExtraFeesCalculationType { get; set; }
            public DriverCommissionCalculationType FuelSurchargeCalculationType { get; set; }

            public string? SettlementId { get; set; }

            public bool IsSettled => SettlementId != null;
        }
    }
}