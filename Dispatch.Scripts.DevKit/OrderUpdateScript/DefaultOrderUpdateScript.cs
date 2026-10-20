using Microsoft.Extensions.Logging;
using Dispatch.Measures;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript
{
    public class DefaultOrderUpdateScript : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var attributeId = "my_attribute_id";
            var hasAttribute = order.Attributes.Any(x => x == attributeId);

            if (order.Distance.Kilometers() > 100)
            {
                if (hasAttribute)
                {
                    // order already has attribute
                    return;
                }

                // if the attributeId does not match an existing attribute, it will not be added
                await order.AddAttribute(attributeId);
            }
            else if (hasAttribute)
            {
                await order.RemoveAttribute(attributeId);
            }
        }
    }
}
