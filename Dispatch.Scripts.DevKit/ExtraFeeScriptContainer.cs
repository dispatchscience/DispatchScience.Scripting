using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit
{
    internal class ExtraFeeScriptContainer : IExtraFeeScript
    {
        private readonly IExtraFeeScript _script;

        public ExtraFeeScriptContainer(IExtraFeeScript script)
        {
            _script = script;
        }

        public Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger) => _script.GetExtraFeePriceInfo(order, data, logger);
    }
}
