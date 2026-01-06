using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit
{
    internal class OrderUpdateScriptContainer : IOrderUpdateScript
    {
        private readonly Type _scriptType;

        public OrderUpdateScriptContainer(Type scriptType)
        {
            _scriptType = scriptType;
        }

        public Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger) => ((IOrderUpdateScript)Activator.CreateInstance(_scriptType)!).OnOrderUpdate(order, data, logger);
    }
}
