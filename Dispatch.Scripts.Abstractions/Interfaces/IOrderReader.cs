using Dispatch.Measures;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderReader
    {
        string OrderId { get; }
        string AccountId { get; }
        string ServiceLevelTypeId { get; }
        string ReferenceNumber1 { get; }
        string ReferenceNumber2 { get; }
        string ReferenceNumber3 { get; }
        string VehicleTypeId { get; }
        
        bool IsReadyForInvoicing { get; }
        bool IsReadyForSettlement { get; }
        bool IsInvoiced { get; }
        bool? GenerateProofOfDeliveryOnDelivery { get; }
        Length Distance { get; }

        string PickupNotes { get; }
        string DeliveryNotes { get; }
        string Notes { get; }
        string InternalNotes { get; }

        Driver? AssignedDriver { get; }

        TimeWindow PickupWindow { get; }
        TimeWindow DeliveryWindow { get; }

        ContactInfo PickupContact { get; }
        ContactInfo DeliveryContact { get; }

        Address PickupAddress { get; }
        Address DeliveryAddress { get; }

        ProofOfFulfillment ProofOfPickup { get; }
        ProofOfFulfillment ProofOfDelivery { get; }

        OrderEventDates EventDates { get; }

        string[] Attributes { get; }

        IList<(string UserFieldId, string Value)> UserFields { get; }

        decimal TotalOrderPriceWithoutFuelSurcharge { get; }
        decimal TotalFuelSurcharge { get; }
        decimal TotalOrderPrice { get; }

        OrderDeliveryChargeInfo[] DeliveryCharges { get; }
        OrderExtraFeeInfo[] ExtraFees { get; }
        OrderTrackedItemInfo[] TrackedItems { get; }
        OrderItemInfo[] OrderItems { get; }
        Attachment[] Attachments { get; }
        IWorkflowReader? PickupWorkflow { get; }
        IWorkflowReader? DeliveryWorkflow { get; }
        IWorkflowReader? AddItemWorkflow { get; }

        OrderZone[] Zones { get; }

        bool IsOnHold { get; }
        string? HoldExceptionCodeId { get; }

        OrderStatus Status { get; }

        OrderFulfillmentType FulfillmentType { get; }

        OrderType Type { get; }

        public bool IsRouted => RouteId is not null;
        public string? RouteId { get; }

        // Everything related to driver payout. Will throw an exception if called on a multisegment order. 
        IPayoutReader Payout { get; }

        bool HasSegmentsPendingCreation { get; }

        /// <summary>
        /// Returns the multisegment order if the current order is a segment (FulfillmentType.Segment), otherwise throws an exception.
        /// </summary>
        Task<IOrderReader> GetMultiSegmentOrder();

        /// <summary>
        /// Returns the segments if the current order is multisegment (FulfillmentType.MultiSegment), otherwise throws an exception.
        /// Note: Segment orders might not exist yet, you can check the HasSegmentsPendingCreation property to know if all segments are created/existing.
        /// </summary>
        Task<IOrderReader[]> GetSegmentOrders();
    }
}
