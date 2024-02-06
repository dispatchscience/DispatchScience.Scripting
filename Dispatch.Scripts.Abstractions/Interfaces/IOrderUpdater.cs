using System;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderUpdater : IOrderReader
    {
        /// <summary>
        /// Adds an attribute to the order if it exists.
        /// </summary>
        /// <param name="attributeId">The attributeId to add.</param>
        Task AddAttribute(string attributeId);

        /// <summary>
        /// Remove an attribute from the order if it exists.
        /// </summary>
        /// <param name="attributeId">The attributeId to add.</param>
        Task RemoveAttribute(string attributeId);

        /// <summary>
        /// Put the order on hold.
        /// </summary>
        /// <param name="exceptionCodeId">The exception code Id to add.</param>
        /// <param name="notes">A note explaining why the order was put on hold (optional).</param>
        /// <remarks>
        /// This method will throw if no exception code matches the provided <paramref name="exceptionCodeId"/> or if the order is delivered or cancelled.
        /// </remarks>
        Task PutOnHold(string exceptionCodeId, string? notes = null);

        /// <summary>
        /// Release the order from hold.
        /// </summary>
        Task Release();

        /// <summary>
        /// Marks the order as "Ready for invoicing".
        /// </summary>
        Task MarkAsReadyForInvoicing();

        /// <summary>
        /// Marks the order as "Ready for settlement".
        /// </summary>
        Task MarkAsReadyForSettlement();

        /// <summary>
        /// Marks the order as "Verify before invoicing".
        /// </summary>
        Task MarkAsVerifyBeforeInvoicing();

        /// <summary>
        /// Marks the order as "Verify before settlement".
        /// </summary>
        Task MarkAsVerifyBeforeSettlement();

        /// <summary>
        /// Override the delivery charge.
        /// </summary>
        /// <remarks>
        /// If the order doesn't have a delivery charge, one will be automatically added.
        /// </remarks>
        /// <param name="basePrice">The new delivery charge amount, excluding fuel surcharges.</param>
        Task OverrideDeliveryCharge(decimal basePrice);

        /// <summary>
        /// Add an extra fee charge.
        /// </summary>
        /// <param name="extraFeeTypeId">The ExtraFeeType id to add.</param>
        /// <param name="quantity">The quantity (may be ignored if the ExtraFeeType is configured as 'Scripted')</param>
        /// <param name="unitPrice">The unit price. If a value is provided, it will override any logic in the ExtraFeeType configuration.</param>
        /// <param name="totalPrice">The total price. If a value is provided, it will override any logic in the ExtraFeeType configuration.</param>
        Task AddExtraFee(string extraFeeTypeId, decimal quantity, decimal? unitPrice = null, decimal? totalPrice = null);

        /// <summary>
        /// Update an existing extra fee charge.
        /// </summary>
        /// <param name="chargeId">The id of the charge to update (not the ExtraFeeTypeId).</param>
        /// <param name="quantity">The quantity (may be ignored if the ExtraFeeType is configured as 'Scripted'). If the provided value is null, the existing quantity will be used.</param>
        /// <param name="unitPrice">The unit price. If a value is provided, it will override any logic in the ExtraFeeType configuration.</param>
        /// <param name="totalPrice">The total price. If a value is provided, it will override any logic in the ExtraFeeType configuration.</param>
        Task UpdateExtraFee(string chargeId, decimal? quantity, decimal? unitPrice = null, decimal? totalPrice = null);

        /// <summary>
        /// Remove an existing extra fee charge.
        /// </summary>
        /// <param name="chargeId">The id of the charge to remove (not the ExtraFeeTypeId).</param>
        Task RemoveExtraFee(string chargeId);

        /// <summary>
        /// Add or update an order user field
        /// </summary>
        /// <param name="userFieldId">The id of the user field to update</param>
        /// <param name="userFieldValue">The user field value </param>
        /// <returns></returns>
        Task AddOrUpdateOrderUserField(string userFieldId, string userFieldValue);

        /// <summary>
        /// Add or update an item user field
        /// </summary>
        /// <param name="itemId">The id of the item the userfield belongs to</param>
        /// <param name="userFieldId">The id of the user field to update</param>
        /// <param name="userFieldValue">The user field value </param>
        /// <returns></returns>
        Task AddOrUpdateItemUserField(string itemId, string userFieldId, string userFieldValue);

        /// <summary>
        /// Enable or disable proof of delivery generation on order delivery.
        /// </summary>
        /// <remarks>
        /// If no Proof of Delivery report template is associated to the order's account, this method will have no effect.
        /// </remarks>
        /// <param name="isEnabled">true to enable proof of generation on order delivery, false to disable it.</param>
        Task SetGenerateProofOfDeliveryOnDelivery(bool isEnabled);

        /// <summary>
        /// Add one or more hubs to a Standard order by using hub ids. This method cannot be called more than once, because once a hub is added,
        /// it's FulfillmentType will be MultiSegment and attempting to call it will throw an exception.
        /// </summary>
        /// <param name="hubIds">The ids of the hub to add.</param>
        /// <returns></returns>
        Task AddHubs(params string[] hubIds);

        /// <summary>
        /// Add one or more hubs to a Standard order. This method cannot be called more than once, because once a hub is added, 
        /// it's FulfillmentType will be MultiSegment and attempting to call it will throw an exception.
        /// </summary>
        /// <param name="hubInfos">The list of hubs you want to add.</param>
        Task AddHubs(params HubInfo[] hubInfos);

        /// <summary>
        /// Update name of the person that has signed the proof of delivery.
        /// </summary>
        Task UpdateProofOfDeliveryReceivedBy(string receivedBy);

        /// <summary>
        /// Update name of the person that has signed the proof of pickup.
        /// </summary>
        Task UpdateProofOfPickupReceivedFrom(string receivedFrom);

        /// <summary>
        /// Update one of the reference number value.
        /// </summary>
        Task UpdateReferenceNumber(string referenceNumberValue, ReferenceNumberIndex referenceNumberIndex);

        /// <summary>
        /// Update the notes associated with the pickup information
        /// </summary>
        /// <param name="pickupNotes">The new notes information</param>
        Task UpdatePickupNotes(string pickupNotes);

        /// <summary>
        /// Update the notes associated with the delivery information
        /// </summary>
        /// <param name="deliveryNotes">The new notes information</param>
        Task UpdateDeliveryNotes(string deliveryNotes);

        /// <summary>
        /// Update the internal notes of the order
        /// </summary>
        /// <param name="internalNotes">The new notes information</param>
        Task UpdateInternalNotes(string internalNotes);

        /// <summary>
        /// Update the notes of the order
        /// </summary>
        /// <param name="notes">The new notes information</param>
        Task UpdateNotes(string notes);

        // Everything related to driver payout. Will throw an exception if called on a multisegment order. 
        new IPayoutUpdater Payout { get; }

        /// <summary>
        /// Returns the multisegment order if the current order is a segment (FulfillmentType.Segment), otherwise throws an exception.
        /// </summary>
        new Task<IOrderUpdater> GetMultiSegmentOrder();

        /// <summary>
        /// Returns the segments if the current order is multisegment (FulfillmentType.MultiSegment), otherwise throws an exception.
        /// Note: Segment orders might not exist yet, you can check the HasSegmentsPendingCreation property to know if all segments are created/existing.
        /// </summary>
        new Task<IOrderUpdater[]> GetSegmentOrders();

        /// <summary>
        /// Update the note and includeWithInvoice flag of an attachment
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <param name="note"></param>
        /// <param name="includeWithInvoice"></param>
        /// <returns></returns>
        Task UpdateAttachment(string attachmentId, string note, bool includeWithInvoice);

        /// <summary>
        /// Update the pickup window of the order
        /// </summary>
        /// <param name="newPickupWindow">The new pickup window</param>
        /// <remarks>
        /// This method will throw if the changes result in the order having invalid time windows.
        /// </remarks>
        Task UpdatePickupWindow(TimeWindow newPickupWindow);

        /// <summary>
        /// Update the delivery window of the order
        /// </summary>
        /// <param name="newDeliveryWindow">The new delivery window</param>
        /// <remarks>
        /// This method will throw if the changes result in the order having invalid time windows.
        /// </remarks>
        Task UpdateDeliveryWindow(TimeWindow newDeliveryWindow);

        /// <summary>
        /// Update both pickup and delivery windows of the order
        /// </summary>
        /// <param name="newPickupWindow">The new pickup window</param>
        /// <param name="newDeliveryWindow">The new delivery window</param>
        /// <remarks>
        /// This method will throw if the changes result in the order having invalid time windows.
        /// </remarks>
        Task UpdateTimeWindows(TimeWindow newPickupWindow, TimeWindow newDeliveryWindow);

        /// <summary>
        /// Update the pickup load duration of the order
        /// </summary>
        /// <param name="newDuration">The new pickup load duration. Value must be between 0 and 120 minutes.</param>
        /// <param name="preventRelatedSegmentUpdate">In the case of a MultiSegment/Segment order, if you don't want your change to have an impact on other segments or the multisegment order, set this to true.</param>
        Task UpdatePickupDuration(TimeSpan newDuration, bool preventRelatedSegmentUpdate = false);

        /// <summary>
        /// Update the delivery unload duration of the order
        /// </summary>
        /// <param name="newDuration">The new delivery unload duration</param>
        /// <param name="preventRelatedSegmentUpdate">In the case of a MultiSegment/Segment order, if you don't want your change to have an impact on other segments or the multisegment order, set this to true.</param>
        Task UpdateDeliveryDuration(TimeSpan newDuration, bool preventRelatedSegmentUpdate = false);

        /// <summary>
        /// Assign the order to a driver
        /// </summary>
        /// <param name="driverId">The driver id to assign the order to</param>
        /// <param name="configureOptions">The action used to configure the options</param>
        Task AssignDriver(string driverId, Action<DriverAssignationOptions>? configureOptions = null);

        /// <summary>
        /// Converts a routed order to On-Demand
        /// </summary>
        Task ConvertToOnDemand();

        /// <summary>
        /// Move an order to a route. Use the IRouteScriptInfo object returned from IScriptDataProvider route methods.
        /// </summary>
        Task MoveToRoute(IRouteScriptInfo routeInfo);

        new IWorkflowUpdater? PickupWorkflow { get; }
        new IWorkflowUpdater? DeliveryWorkflow { get; }
        new IWorkflowUpdater? AddItemWorkflow { get; }
    }
}
