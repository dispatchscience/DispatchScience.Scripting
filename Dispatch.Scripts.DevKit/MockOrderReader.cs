using Dispatch.Measures;

namespace Dispatch.Scripts.DevKit
{
    internal class MockOrderReader : IOrderReader
    {
        public string OrderId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string ServiceLevelTypeId { get; set; } = string.Empty;
        public string ReferenceNumber1 { get; set; } = string.Empty;
        public string ReferenceNumber2 { get; set; } = string.Empty;
        public string ReferenceNumber3 { get; set; } = string.Empty;
        public string VehicleTypeId { get; set; } = string.Empty;
        public bool IsReadyForInvoicing { get; set; }
        public bool IsReadyForSettlement { get; set; }
        public bool IsInvoiced { get; set; }
        public bool? GenerateProofOfDeliveryOnDelivery { get; set; }
        public Length Distance { get; set; }
        public string PickupNotes { get; set; } = string.Empty;
        public string DeliveryNotes { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string InternalNotes { get; set; } = string.Empty;
        public Driver? AssignedDriver { get; set; }
        public DateTimeOffset? ReadyAt { get; set; }
        public TimeWindow PickupWindow { get; set; }
        public TimeWindow DeliveryWindow { get; set; }
        public TimeSpan PickupDuration { get; set; }
        public TimeSpan DeliveryDuration { get; set; }
        public ContactInfo PickupContact { get; set; } = new();
        public ContactInfo DeliveryContact { get; set; } = new();
        public Address PickupAddress { get; set; } = new();
        public Address DeliveryAddress { get; set; } = new();
        public ProofOfFulfillment ProofOfPickup { get; set; } = new();
        public ProofOfFulfillment ProofOfDelivery { get; set; } = new();
        public OrderEventDates EventDates { get; set; } = new();
        public string[] Attributes { get; set; } = [];
        public IList<(string UserFieldId, string Value)> UserFields { get; set; } = [];
        public decimal TotalOrderPriceWithoutFuelSurcharge { get; set; }
        public decimal TotalFuelSurcharge { get; set; }
        public decimal TotalOrderPrice { get; set; }
        public Weight TotalWeight { get; set; }
        public OrderDeliveryChargeInfo[] DeliveryCharges { get; set; } = [];
        public OrderExtraFeeInfo[] ExtraFees { get; set; } = [];
        public OrderTrackedItemInfo[] TrackedItems { get; set; } = [];
        public OrderItemInfo[] OrderItems { get; set; } = [];
        public OrderExceptionCodeInfo[] ExceptionCodes { get; set; } = [];
        public Attachment[] Attachments { get; set; } = [];
        public PackageValidationOptions? PackageValidationOptions { get; set; }
        public decimal? CollectOnDelivery { get; set; }
        public decimal? CollectOnPickup { get; set; }
        public bool AllowPartialCollectOnDelivery { get; set; }
        public bool AllowPartialCollectOnPickup { get; set; }
        public OrderZone[] Zones { get; set; } = [];
        public bool IsOnHold { get; set; }
        public string? HoldExceptionCodeId { get; set; }
        public OrderStatus Status { get; set; }
        public OrderFulfillmentType FulfillmentType { get; set; }
        public OrderType Type { get; set; }
        public OrderSource Source { get; set; }
        public string? RouteId { get; set; }
        public bool HasSegmentsPendingCreation { get; set; }

        public MockWorkflowReader? PickupWorkflow { get; set; }
        public MockWorkflowReader? DeliveryWorkflow { get; set; }
        public MockWorkflowReader? AddItemWorkflow { get; set; }
        public MockPayoutReader Payout { get; set; } = new();

        public string? OverriddenTaxScheduleId { get; set; }

        IWorkflowReader? IOrderReader.PickupWorkflow => PickupWorkflow;
        IWorkflowReader? IOrderReader.DeliveryWorkflow => DeliveryWorkflow;
        IWorkflowReader? IOrderReader.AddItemWorkflow => AddItemWorkflow;
        IPayoutReader IOrderReader.Payout => Payout;

        public Task<IOrderReader> GetMultiSegmentOrder() => throw new NotImplementedException();
        public Task<IOrderReader[]> GetSegmentOrders() => throw new NotImplementedException();
    }

    internal class MockPayoutReader : IPayoutReader
    {
        public PayoutInfo PayoutInfo { get; set; } = new();
    }

    internal class MockWorkflowReader : IWorkflowReader
    {
        public string Id { get; set; } = default!;
        public IList<MockWorkflowStepReader> Steps { get; set; } = default!;

        IList<IWorkflowReader.IWorkflowStepReader> IWorkflowReader.Steps => Steps.Cast<IWorkflowReader.IWorkflowStepReader>().ToList();

        internal class MockWorkflowStepReader : IWorkflowReader.IWorkflowStepReader
        {
            public string Id { get; set; } = default!;
            public string TitlePrimary { get; set; } = default!;
            public string? TitleSecondary { get; set; }
            public bool CanSkip { get; set; }
            public WorkflowStepType StepType { get; set; }
            public bool IsActive { get; set; }
            public string? UserFieldId { get; set; }
            public string? Tag { get; set; }
        }
    }
}
