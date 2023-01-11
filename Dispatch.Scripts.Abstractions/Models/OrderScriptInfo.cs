using Dispatch.Measures;
using System;
using System.Collections.Generic;

namespace Dispatch.Scripts
{
    public class OrderScriptInfo
    {
        public Length Distance { get; set; }

        public OrderType OrderType { get; set; }

        public Address PickupAddress { get; set; } = new Address();

        public Address DeliveryAddress { get; set; } = new Address();

        public DateTimeOffset? PickupFromTime { get; set; }

        public DateTimeOffset? PickupToTime { get; set; }

        public DateTimeOffset? DeliveryFromTime { get; set; }

        public DateTimeOffset? DeliveryToTime { get; set; }       

        public string? ServiceLevelTypeId { get; set; }

        public string? VehicleTypeId { get; set; }

        public TimeSpan PickupDuration { get; set; }

        public TimeSpan DeliveryDuration { get; set; }

        public DeliveryChargeInfo DeliveryCharge { get; set; } = new DeliveryChargeInfo();

        public IList<(string UserFieldId, string Value)> UserFields = new List<(string UserFieldId, string Value)>();

        public IList<OrderItemInfo> OrderItemInfos { get; set; } = new List<OrderItemInfo>();
    }

    public class DeliveryChargeInfo
    {
        public decimal? BasePrice { get; set; }
        public decimal? VehicleSurcharge { get; set; }
        public string? PickupPricingZone { get; set; }
        public string? DeliveryPricingZone { get; set; }
    }
}