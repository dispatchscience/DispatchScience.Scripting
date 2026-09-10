using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.AssignDriver
{
    public class AssignDriver : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var events = await data.GetEventNames();
            var test = await data.GetAccountInfo();
            var test2 = await data.GetScriptData();
            var test3 = await data.GetSheet();

            if (order.AssignedDriver is null)
            {
                await order.AssignDriver("252e9476-23b9-4ab4-be7a-a109453dd861");
            }
        }
    }
}
