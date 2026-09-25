using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.AddExtraFeeForWorkflow
{
    public class AddExtraFeeForWorkflow : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var hasSignatureWf = order.DeliveryWorkflow?.Steps.Any(x => x.IsActive && x.StepType == WorkflowStepType.Signature);
            var extraFeeQty = (hasSignatureWf.HasValue && hasSignatureWf.Value) ? 1 : 0;
            var signatureExtraFee = order.ExtraFees.FirstOrDefault(e => string.Equals(e.ExtraFeeTypeId, "signature_required", StringComparison.InvariantCultureIgnoreCase));
            if (signatureExtraFee is null)
            {
                await order.AddExtraFee("Signature_Required", extraFeeQty);
            }
            else if (extraFeeQty != signatureExtraFee.Quantity)
            {
                await order.UpdateExtraFee(signatureExtraFee.ChargeId, extraFeeQty);
            }
        }
    }
}
