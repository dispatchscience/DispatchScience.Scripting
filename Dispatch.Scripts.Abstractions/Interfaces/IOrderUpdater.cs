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
    }
}
