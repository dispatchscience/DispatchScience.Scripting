using System;

namespace Dispatch.Scripts
{
    public class PayoutInfo
    {
        public decimal? PredefinedPayoutValue { get; set; }
        public DriverCommissionCalculationType DriverCommissionCalculationType { get; set; }
        public decimal? DeliveryCommissionPercentage { get; set; }
        public decimal? FuelSurchargeCommissionPercentage { get; set; }
        public decimal? ExtraCommissionPercentage { get; set; }
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

            public DriverCommissionCalculationType CalculationType { get; set; }

            public string? SettlementId { get; set; }

            public bool IsSettled => SettlementId != null;
        }
    }
}