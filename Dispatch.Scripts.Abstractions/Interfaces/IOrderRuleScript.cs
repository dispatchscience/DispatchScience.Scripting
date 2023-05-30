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
    // This line is needed in case you don't need the file values (otherwise you will get an async/await compilation error.
    await Task.CompletedTask;

    return order.ServiceLevelTypeId.Equals(\""rush\"", StringComparison.OrdinalIgnoreCase);
}";
    }
}
