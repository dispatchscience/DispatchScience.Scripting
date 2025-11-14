using Dispatch.Measures;
using Dispatch.Scripts.Abstractions;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Dispatch.Scripts.DevKit
{
    internal class MockOrderUpdater : IOrderUpdater
    {
        private readonly ScriptDebugWrapper _scriptDebugWrapper;
        private readonly MockOrderReader _orderReader;
        private ILogger _logger;

        public MockOrderUpdater(ScriptDebugWrapper scriptDebugWrapper, ILogger logger, string? orderId = null)
        {
            _scriptDebugWrapper = scriptDebugWrapper;
            _logger = logger;

            orderId ??= _scriptDebugWrapper.GetProperty<string>(ScriptDebugWrapper.OrderId);
            _orderReader = _scriptDebugWrapper.GetOrderReader<MockOrderReader>(orderId)!;
        }

        public void UpdateLogger(ILogger logger)
        {
            _logger = logger;
        }

        public MockOrderReader OrderReader => _orderReader;

        public string OrderId => _orderReader.OrderId;
        public string AccountId => _orderReader.AccountId;
        public string ServiceLevelTypeId => _orderReader.ServiceLevelTypeId;
        public string ReferenceNumber1 => _orderReader.ReferenceNumber1;
        public string ReferenceNumber2 => _orderReader.ReferenceNumber2;
        public string ReferenceNumber3 => _orderReader.ReferenceNumber3;
        public string VehicleTypeId => _orderReader.VehicleTypeId;
        public bool IsReadyForInvoicing => _orderReader.IsReadyForInvoicing;
        public bool IsReadyForSettlement => _orderReader.IsReadyForSettlement;
        public bool IsInvoiced => _orderReader.IsInvoiced;
        public bool? GenerateProofOfDeliveryOnDelivery => _orderReader.GenerateProofOfDeliveryOnDelivery;
        public Length Distance => _orderReader.Distance;
        public string PickupNotes => _orderReader.PickupNotes;
        public string DeliveryNotes => _orderReader.DeliveryNotes;
        public string Notes => _orderReader.Notes;
        public string InternalNotes => _orderReader.InternalNotes;
        public Driver? AssignedDriver => _orderReader.AssignedDriver;
        public DateTimeOffset? ReadyAt => _orderReader.ReadyAt;
        public TimeWindow PickupWindow => _orderReader.PickupWindow;
        public TimeWindow DeliveryWindow => _orderReader.DeliveryWindow;
        public TimeSpan PickupDuration => _orderReader.PickupDuration;
        public TimeSpan DeliveryDuration => _orderReader.DeliveryDuration;
        public ContactInfo PickupContact => _orderReader.PickupContact;
        public ContactInfo DeliveryContact => _orderReader.DeliveryContact;
        public Address PickupAddress => _orderReader.PickupAddress;
        public Address DeliveryAddress => _orderReader.DeliveryAddress;
        public ProofOfFulfillment ProofOfPickup => _orderReader.ProofOfPickup;
        public ProofOfFulfillment ProofOfDelivery => _orderReader.ProofOfDelivery;
        public OrderEventDates EventDates => _orderReader.EventDates;
        public string[] Attributes => _orderReader.Attributes;
        public IList<(string UserFieldId, string Value)> UserFields => _orderReader.UserFields;
        public decimal TotalOrderPriceWithoutFuelSurcharge => _orderReader.TotalOrderPriceWithoutFuelSurcharge;
        public decimal TotalFuelSurcharge => _orderReader.TotalFuelSurcharge;
        public decimal TotalOrderPrice => _orderReader.TotalOrderPrice;
        public Weight TotalWeight => _orderReader.TotalWeight;
        public OrderDeliveryChargeInfo[] DeliveryCharges => _orderReader.DeliveryCharges;
        public OrderExtraFeeInfo[] ExtraFees => _orderReader.ExtraFees;
        public OrderTrackedItemInfo[] TrackedItems => _orderReader.TrackedItems;
        public OrderItemInfo[] OrderItems => _orderReader.OrderItems;
        public OrderExceptionCodeInfo[] ExceptionCodes => _orderReader.ExceptionCodes;
        public Attachment[] Attachments => _orderReader.Attachments;
        public PackageValidationOptions? PackageValidationOptions => _orderReader.PackageValidationOptions;
        public decimal? CollectOnDelivery => _orderReader.CollectOnDelivery;
        public decimal? CollectOnPickup => _orderReader.CollectOnPickup;
        public bool AllowPartialCollectOnDelivery => _orderReader.AllowPartialCollectOnDelivery;
        public bool AllowPartialCollectOnPickup => _orderReader.AllowPartialCollectOnPickup;
        public OrderZone[] Zones => _orderReader.Zones;
        public bool IsOnHold => _orderReader.IsOnHold;
        public string? HoldExceptionCodeId => _orderReader.HoldExceptionCodeId;
        public OrderStatus Status => _orderReader.Status;
        public OrderFulfillmentType FulfillmentType => _orderReader.FulfillmentType;
        public OrderType Type => _orderReader.Type;
        public OrderSource Source => _orderReader.Source;
        public string? RouteId => _orderReader.RouteId;
        public bool HasSegmentsPendingCreation => _orderReader.HasSegmentsPendingCreation;
        IWorkflowReader? IOrderReader.PickupWorkflow => PickupWorkflow;
        IWorkflowReader? IOrderReader.DeliveryWorkflow => DeliveryWorkflow;
        IWorkflowReader? IOrderReader.AddItemWorkflow => AddItemWorkflow;
        IPayoutReader IOrderReader.Payout => Payout;
        async Task<IOrderReader> IOrderReader.GetMultiSegmentOrder() => await GetMultiSegmentOrder();
        async Task<IOrderReader[]> IOrderReader.GetSegmentOrders() => await GetSegmentOrders();

        public async Task<IOrderUpdater> GetMultiSegmentOrder()
        {
            await Task.CompletedTask;
            var multiSegmentOrderId = _scriptDebugWrapper.GetProperty<string>(ScriptDebugWrapper.MultiSegmentOrderId);
            return new MockOrderUpdater(_scriptDebugWrapper, _logger, multiSegmentOrderId);
        }

        public async Task<IOrderUpdater[]> GetSegmentOrders()
        {
            await Task.CompletedTask;
            var segmentOrderIds = _scriptDebugWrapper.GetProperty<string[]>(ScriptDebugWrapper.SegmentOrderIds);
            return segmentOrderIds
                .Select(x => new MockOrderUpdater(_scriptDebugWrapper, _logger, x))
                .ToArray();
        }

        public IPayoutUpdater Payout => new MockPayoutUpdater(_orderReader.Payout, _logger);
        public IWorkflowUpdater? PickupWorkflow => _orderReader.PickupWorkflow is not null ? new MockWorkflowUpdater(_orderReader.PickupWorkflow, _logger) : null;
        public IWorkflowUpdater? DeliveryWorkflow => _orderReader.DeliveryWorkflow is not null ? new MockWorkflowUpdater(_orderReader.DeliveryWorkflow, _logger) : null;
        public IWorkflowUpdater? AddItemWorkflow => _orderReader.AddItemWorkflow is not null ? new MockWorkflowUpdater(_orderReader.AddItemWorkflow, _logger) : null;

        public string? OverriddenTaxScheduleId => _orderReader.OverriddenTaxScheduleId;

        public async Task AddAttribute(string attributeId)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddAttribute)}('{attributeId}') called");
        }

        public async Task AddExtraFee(string extraFeeTypeId, decimal quantity, decimal? unitPrice = null, decimal? totalPrice = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddExtraFee)}('{extraFeeTypeId}', {quantity}, {unitPrice}, {totalPrice})");
        }

        public async Task AddHubs(params string[] hubIds)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddHubs)}('{string.Join(", ", hubIds)}') called");
        }

        public async Task AddHubs(params HubInfo[] hubInfos)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddHubs)}('{string.Join(", ", hubInfos.Select(x => x.HubId))}') called");
        }

        public async Task AddItemExceptionCodes(string[] exceptionCodeTypeIds, string itemId, OrderExceptionCodeAddedAt addedAt)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddItemExceptionCodes)}('{string.Join(", ", exceptionCodeTypeIds)}', '{itemId}', '{addedAt}') called");
        }

        public async Task AddOrderExceptionCodes(string[] exceptionCodeTypeIds, OrderExceptionCodeAddedAt addedAt)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddOrderExceptionCodes)}('{string.Join(", ", exceptionCodeTypeIds)}', '{addedAt}') called");
        }

        public async Task AddOrUpdateItemUserField(string itemId, string userFieldId, string userFieldValue)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddOrUpdateItemUserField)}('{itemId}', '{userFieldId}', '{userFieldValue}') called");
        }

        public async Task AddOrUpdateOrderUserField(string userFieldId, string userFieldValue)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AddOrUpdateOrderUserField)}('{userFieldId}', '{userFieldValue}') called");
        }

        public async Task AssignDriver(string driverId, Action<DriverAssignationOptions>? configureOptions = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(AssignDriver)}('{driverId}') called");
        }

        public async Task ConvertToOnDemand()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(ConvertToOnDemand)}() called");
        }

        public async Task MarkAsReadyForInvoicing()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(MarkAsReadyForInvoicing)}() called");
        }

        public async Task MarkAsReadyForSettlement()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(MarkAsReadyForSettlement)}() called");
        }

        public async Task MarkAsVerifyBeforeInvoicing()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(MarkAsVerifyBeforeInvoicing)}() called");
        }

        public async Task MarkAsVerifyBeforeSettlement()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(MarkAsVerifyBeforeSettlement)}() called");
        }

        public async Task MoveToRoute(IRouteScriptInfo routeInfo)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(MoveToRoute)}('{routeInfo.Id}') called");
        }

        public async Task OverrideDeliveryCharge(decimal basePrice)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(OverrideDeliveryCharge)}({basePrice}) called");
        }

        public async Task OverrideDistance(Length distance)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(OverrideDistance)}({distance}) called");
        }

        public async Task PutOnHold(string exceptionCodeId, string? notes = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(PutOnHold)}('{exceptionCodeId}', '{notes}') called");
        }

        public async Task RecalculateCharges(bool updateDeliveryCharge = true, bool updateWeightExtraFee = true, bool updateNumberOfPiecesExtraFee = true, bool updateMileageAffectedExtraFees = true, bool updateVehicleAffectedExtraFees = true, bool updateScriptedExtraFees = true)
        {
            await RecalculateCharges(x =>
            {
                x.UpdateDeliveryCharge = updateDeliveryCharge;
                x.UpdateWeightExtraFee = updateWeightExtraFee;
                x.UpdateNumberOfPiecesExtraFee = updateNumberOfPiecesExtraFee;
                x.UpdateMileageAffectedExtraFees = updateMileageAffectedExtraFees;
                x.UpdateVehicleAffectedExtraFees = updateVehicleAffectedExtraFees;
                x.UpdateScriptedExtraFees = updateScriptedExtraFees;
            });
        }

        public async Task RecalculateCharges(Action<PricingUpdateOptions>? configureOptions = null)
        {
            await Task.CompletedTask;
            var options = PricingUpdateOptions.Default;
            configureOptions?.Invoke(options);
            _logger.LogInformation($"[{OrderId}] {nameof(RecalculateCharges)}({options.UpdateDeliveryCharge}, {options.UpdateWeightExtraFee}, {options.UpdateNumberOfPiecesExtraFee}, {options.UpdateMileageAffectedExtraFees}, {options.UpdateVehicleAffectedExtraFees}, {options.UpdateScriptedExtraFees}) called");
        }

        public async Task UpdateReadyAt(DateTimeOffset readyAt, bool recalculateWindows = true)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateReadyAt)}({readyAt},{recalculateWindows}) called");
        }

        public async Task RecalculateWindows()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(RecalculateWindows)}() called");
        }

        public async Task Release()
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(Release)}() called");
        }

        public async Task RemoveAttribute(string attributeId)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(RemoveAttribute)}('{attributeId}') called");
        }

        public async Task RemoveExtraFee(string chargeId)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(RemoveExtraFee)}('{chargeId}') called");
        }

        public async Task RemoveItemExceptionCodes(string itemId, string[] exceptionCodeTypeIds)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(RemoveItemExceptionCodes)}('{itemId}', '{string.Join(", ", exceptionCodeTypeIds)}') called");
        }

        public async Task RemoveOrderExceptionCodes(string[] exceptionCodeTypeIds)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(RemoveOrderExceptionCodes)}('{string.Join(", ", exceptionCodeTypeIds)}') called");
        }

        public async Task SetGenerateProofOfDeliveryOnDelivery(bool isEnabled)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(SetGenerateProofOfDeliveryOnDelivery)}({isEnabled}) called");
        }

        public async Task UpdateAttachment(string attachmentId, string note, bool includeWithInvoice, bool visibleForDriver = true)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateAttachment)}('{attachmentId}', '{note}', {includeWithInvoice}, {visibleForDriver}) called");
        }

        public async Task UpdateDeliveryDuration(TimeSpan newDuration, bool preventRelatedSegmentUpdate = false)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateDeliveryDuration)}({newDuration}, {preventRelatedSegmentUpdate}) called");
        }

        public async Task UpdatePickupAddress(Address newPickupAddress, Action<PricingUpdateOptions>? configureOptions = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePickupAddress)}({newPickupAddress}) called");
        }

        public async Task UpdateDeliveryAddress(Address newDeliveryAddress, Action<PricingUpdateOptions>? configureOptions = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateDeliveryAddress)}({newDeliveryAddress}) called");
        }

        public async Task UpdateAddresses(Address newPickupAddress, Address newDeliveryAddress, Action<PricingUpdateOptions>? configureOptions = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateAddresses)}({newPickupAddress}, {newDeliveryAddress}) called");
        }

        public async Task UpdatePickupContact(string? name, string? phoneNumber, string? email, string? language)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePickupContact)}({string.Join(", ", [name, phoneNumber, email, language])}) called");
        }

        public async Task UpdateDeliveryContact(string? name, string? phoneNumber, string? email, string? language)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateDeliveryContact)}({string.Join(", ", [name, phoneNumber, email, language])}) called");
        }

        public async Task UpdateDeliveryNotes(string deliveryNotes)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateDeliveryNotes)}('{deliveryNotes}') called");
        }

        public async Task UpdateDeliveryWindow(TimeWindow newDeliveryWindow)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateDeliveryWindow)}({newDeliveryWindow}) called");
        }

        public async Task UpdateExtraFee(string chargeId, decimal? quantity, decimal? unitPrice = null, decimal? totalPrice = null)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateExtraFee)}('{chargeId}', {quantity}, {unitPrice}, {totalPrice}) called");
        }

        public async Task UpdateInternalNotes(string internalNotes)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateInternalNotes)}('{internalNotes}') called");
        }

        public async Task UpdateNotes(string notes)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateNotes)}('{notes}') called");
        }

        public async Task UpdatePackageValidationOptions(PackageValidationOptions packageValidationOptions)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePackageValidationOptions)}({packageValidationOptions}) called");
        }

        public async Task UpdatePickupDuration(TimeSpan newDuration, bool preventRelatedSegmentUpdate = false)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePickupDuration)}({newDuration}, {preventRelatedSegmentUpdate}) called");
        }

        public async Task UpdatePickupNotes(string pickupNotes)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePickupNotes)}('{pickupNotes}') called");
        }

        public async Task UpdatePickupWindow(TimeWindow newPickupWindow)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdatePickupWindow)}({newPickupWindow}) called");
        }

        public async Task UpdateProofOfDeliveryReceivedBy(string receivedBy)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateProofOfDeliveryReceivedBy)}('{receivedBy}') called");
        }

        public async Task UpdateProofOfPickupReceivedFrom(string receivedFrom)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateProofOfPickupReceivedFrom)}('{receivedFrom}') called");
        }

        public async Task UpdateReferenceNumber(string referenceNumberValue, ReferenceNumberIndex referenceNumberIndex)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateReferenceNumber)}('{referenceNumberValue}', {referenceNumberIndex}) called");
        }

        public async Task UpdateTimeWindows(TimeWindow newPickupWindow, TimeWindow newDeliveryWindow)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(UpdateTimeWindows)}({newPickupWindow}, {newDeliveryWindow}) called");
        }

        public async Task OverrideTaxSchedule(string? taxScheduleId)
        {
            await Task.CompletedTask;
            _logger.LogInformation($"[{OrderId}] {nameof(OverrideTaxSchedule)}({(taxScheduleId is null ? null : $"'{taxScheduleId}'")}) called");
        }

        internal class MockPayoutUpdater : IPayoutUpdater
        {
            private readonly IPayoutReader _payoutReader;
            private readonly ILogger _logger;

            public MockPayoutUpdater(IPayoutReader payoutReader, ILogger logger)
            {
                _payoutReader = payoutReader;
                _logger = logger;
            }

            public ICommission Delivery => new MockCommission(nameof(Delivery), _logger);
            public ICommission ExtraFees => new MockCommission(nameof(ExtraFees), _logger);
            public ICommission FuelSurcharge => new MockCommission(nameof(FuelSurcharge), _logger);
            public PayoutInfo PayoutInfo => _payoutReader.PayoutInfo;

            public async Task AddDriver(string driverId)
            {
                await Task.CompletedTask;
                _logger.LogInformation($"[Payout] {nameof(AddDriver)}('{driverId}') called");
            }

            public async Task OverrideManually(ManualPayout[] payouts)
            {
                await Task.CompletedTask;
                _logger.LogInformation($"[Payout] {nameof(OverrideManually)}('{string.Join(", ", payouts.Select(x => $"{x.DriverId}: {x.Amount}"))}') called");
            }

            public Task ChangeToCommissionPercentage(decimal deliveryCommissionPercentage, decimal extraFeeCommissionPercentage, decimal fuelSurchageCommissionPercentage)
                => throw new NotImplementedException();

            public Task ChangeToFlatRate(decimal amount)
                => throw new NotImplementedException();

            public Task ChangeToPayoutSchedule()
                => throw new NotImplementedException();

            internal class MockCommission : ICommission
            {
                private readonly string _identifier;
                private readonly ILogger _logger;

                public MockCommission(string identifier, ILogger logger)
                {
                    _identifier = identifier;
                    _logger = logger;
                }

                public async Task ChangeToCommissionPercentage(decimal percentage)
                {
                    await Task.CompletedTask;
                    _logger.LogInformation($"[Commission {_identifier}] {nameof(ChangeToCommissionPercentage)}({percentage}) called");
                }

                public async Task ChangeToFixedPayoutSchedule(int payoutScheduleId)
                {
                    await Task.CompletedTask;
                    _logger.LogInformation($"[Commission {_identifier}] {nameof(ChangeToFixedPayoutSchedule)}({payoutScheduleId}) called");
                }

                public async Task ChangeToFlatRate(decimal amount)
                {
                    await Task.CompletedTask;
                    _logger.LogInformation($"[Commission {_identifier}] {nameof(ChangeToFlatRate)}({amount}) called");
                }

                public async Task ChangeToPayoutSchedule()
                {
                    await Task.CompletedTask;
                    _logger.LogInformation($"[Commission {_identifier}] {nameof(ChangeToPayoutSchedule)}() called");
                }
            }
        }

        internal class MockWorkflowUpdater : IWorkflowUpdater
        {
            private readonly IWorkflowReader _workflowReader;
            private readonly ILogger _logger;

            public MockWorkflowUpdater(IWorkflowReader workflowReader, ILogger logger)
            {
                _workflowReader = workflowReader;
                _logger = logger;
            }

            public string Id => _workflowReader.Id;

            public IList<IWorkflowUpdater.IWorkflowStepUpdater> Steps => _workflowReader.Steps.Select(x => new MockWorkflowStepUpdater(x, _logger)).Cast<IWorkflowUpdater.IWorkflowStepUpdater>().ToList();

            IList<IWorkflowReader.IWorkflowStepReader> IWorkflowReader.Steps => _workflowReader.Steps;

            internal class MockWorkflowStepUpdater : IWorkflowUpdater.IWorkflowStepUpdater
            {
                private readonly IWorkflowReader.IWorkflowStepReader _reader;
                private readonly ILogger _logger;

                public MockWorkflowStepUpdater(IWorkflowReader.IWorkflowStepReader reader, ILogger logger)
                {
                    _reader = reader;
                    _logger = logger;
                }

                public string Id => _reader.Id;
                public string TitlePrimary => _reader.TitlePrimary;
                public string? TitleSecondary => _reader.TitleSecondary;
                public bool CanSkip => _reader.CanSkip;
                public WorkflowStepType StepType => _reader.StepType;
                public bool IsActive => _reader.IsActive;
                public string? UserFieldId => _reader.UserFieldId;
                public string? Tag => _reader.Tag;

                public async Task SetIsActive(bool isActive)
                {
                    await Task.CompletedTask;
                    _logger.LogInformation($"[WorkflowStep {TitlePrimary}] {nameof(SetIsActive)}({IsActive}) called");
                }
            }
        }
    }
}
