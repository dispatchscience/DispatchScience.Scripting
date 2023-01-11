using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IExtraFeeScriptFile
    {
        Task<ExtraFeeScriptCell[]> GetSheet(string? sheetName = null);
    }
}