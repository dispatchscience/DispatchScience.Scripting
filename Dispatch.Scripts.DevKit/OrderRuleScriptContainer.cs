using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit
{
    internal class OrderRuleScriptContainer : IOrderRuleScript
    {
        private readonly Type _scriptType;

        public OrderRuleScriptContainer(Type scriptType)
        {
            _scriptType = scriptType;
        }

        public Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger) => ((IOrderRuleScript)Activator.CreateInstance(_scriptType)!).EvaluateRule(order, data, logger);
    }
}
