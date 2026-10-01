using System;

namespace Dispatch.Scripts
{
    public class Driver
    {
        public string Id { get; set; } = default!;

        /// <summary>
        /// May not be set depending on the context in which the script is executed.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// May not be set depending on the context in which the script is executed.
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// May not be set depending on the context in which the script is executed.
        /// </summary>
        public int? PayRatePriceListId { get; set; }

        /// <summary>
        /// May not be set depending on the context in which the script is executed.
        /// </summary>
        public int? PayRateFuelSurchargeScheduleId { get; set; }

        /// <summary>
        /// May not be set depending on the context in which the script is executed.
        /// </summary>
        public int? PayRateExtraFeeScheduleId { get; set; }
    }
}