using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IOrderUpdateScript
    {
        Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger);
    }
}
