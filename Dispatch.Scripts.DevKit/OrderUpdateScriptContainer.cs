using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dispatch.Scripts.DevKit
{
    internal class OrderUpdateScriptContainer : IOrderUpdateScript
    {
        private readonly IOrderUpdateScript _script;

        public OrderUpdateScriptContainer(IOrderUpdateScript script)
        {
            _script = script;
        }

        public Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger) => _script.OnOrderUpdate(order, data, logger);
    }
}
