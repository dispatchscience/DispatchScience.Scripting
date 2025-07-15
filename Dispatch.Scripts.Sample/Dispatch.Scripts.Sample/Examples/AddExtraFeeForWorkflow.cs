using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class AddExtraFeeForWorkflow : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {    
            var hasSignatureWf = order.DeliveryWorkflow?.Steps.Any(x => x.IsActive && x.StepType == WorkflowStepType.Signature);
            var extraFeeQty = (hasSignatureWf.HasValue && hasSignatureWf.Value) ? 1 : 0;
            var sigantureExtraFee = order.ExtraFees.FirstOrDefault(e => string.Equals(e.ExtraFeeTypeId, "signature_required", StringComparison.InvariantCultureIgnoreCase));
            if (sigantureExtraFee == null)
            {
                await order.AddExtraFee("Signature_Required", extraFeeQty);
            }
            else if (extraFeeQty != sigantureExtraFee.Quantity)
            {
                await order.UpdateExtraFee(sigantureExtraFee.ChargeId, extraFeeQty);
            }
        }
    }
}
