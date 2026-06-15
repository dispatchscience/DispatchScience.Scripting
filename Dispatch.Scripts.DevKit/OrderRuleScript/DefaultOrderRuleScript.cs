using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderRuleScript
{
    public class DefaultOrderRuleScript : IOrderRuleScript
    {
        public async Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger)
        {
            // This line is needed in case you don't need the file values (otherwise you will get an async/await compilation error.
            await Task.CompletedTask;

            return order.ServiceLevelTypeId.Equals("rush", StringComparison.OrdinalIgnoreCase);
        }
    }
}
