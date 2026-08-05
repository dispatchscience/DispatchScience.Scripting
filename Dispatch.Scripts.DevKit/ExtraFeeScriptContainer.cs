using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit
{
    internal class ExtraFeeScriptContainer : IExtraFeeScript
    {
        private readonly Type _scriptType;

        public ExtraFeeScriptContainer(Type scriptType)
        {
            _scriptType = scriptType;
        }

        public Task<ExtraFeeScriptResult> GetExtraFeePriceInfo(OrderScriptInfo order, IScriptDataProvider data, ILogger logger) => ((IExtraFeeScript)Activator.CreateInstance(_scriptType)!).GetExtraFeePriceInfo(order, data, logger);
    }
}
