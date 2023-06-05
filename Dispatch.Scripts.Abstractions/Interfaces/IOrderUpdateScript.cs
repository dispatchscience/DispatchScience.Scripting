using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderUpdateScript
    {
        Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger);

        static string DefaultScript => @"using System.Linq;

public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
{
    var attributeId = \""my_attribute_id\"";
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

public async Task<string?> ValidateFile(ScriptCell[] sheetValues)
{
    // TODO check if sheetValues make sense, if not return an error message, otherwise return null
    return null;
}";
    }
}
