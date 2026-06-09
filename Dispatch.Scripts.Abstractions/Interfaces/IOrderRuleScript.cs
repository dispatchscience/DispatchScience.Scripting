using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderRuleScript
    {
        Task<bool> EvaluateRule(IOrderReader order, IScriptDataProvider data, ILogger logger);
    }
}
