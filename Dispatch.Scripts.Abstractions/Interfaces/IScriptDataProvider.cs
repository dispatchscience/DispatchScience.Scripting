using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IScriptDataProvider
    {
        Task<IDictionary<string, string>> GetScriptData();
        Task<IList<string>> GetEventNames();

        Task<IDictionary<string, string>> GetAccountFields();
        Task<AccountScriptInfo?> GetAccountInfo();
        Task<MasterAccountScriptInfo?> GetMasterAccountInfo();
        Task<IList<string>> GetZones(ScriptZoneType zoneType, LatLng position);
        Task<IList<string>> GetAvailableExtraFeeTypes();
        Task<IList<ParcelTypeInfo>> GetAvailableParcelTypes();
        Task<HolidayInfo> GetHoliday(DateTimeOffset date);
        Task<IList<HubInfo>> FindHubs(string hubName);
    }
}
