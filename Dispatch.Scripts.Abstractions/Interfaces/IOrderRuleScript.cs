using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderRuleScript
    {
        Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger);

        static string DefaultScript => @"using System.Linq;

public async Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger)
{
    await Task.CompletedTask;

    return order.ServiceLevelTypeId.Equals(\""rush\"", StringComparison.OrdinalIgnoreCase);
}";
    }
}
