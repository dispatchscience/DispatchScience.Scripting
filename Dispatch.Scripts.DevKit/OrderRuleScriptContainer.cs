using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit
{
    internal class OrderRuleScriptContainer : IOrderRuleScript
    {
        private readonly IOrderRuleScript _script;

        public OrderRuleScriptContainer(IOrderRuleScript script)
        {
            _script = script;
        }

        public Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger) => _script.EvaluateRule(order, data, logger);
    }
}
