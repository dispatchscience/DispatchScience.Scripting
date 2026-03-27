using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IScriptFileValidation
    {
        Task<string?> ValidateFile(ScriptCell[] sheetValues);
    }
}
