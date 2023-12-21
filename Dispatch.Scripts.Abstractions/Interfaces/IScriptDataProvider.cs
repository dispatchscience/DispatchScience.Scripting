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
        Task<IList<Driver>> FindDrivers(string driverNumber);
        Task<IDictionary<string, string>> GetDriverFields(string driverId);

        /// <summary>
        /// Returns the IRouteScriptInfo for a given routeId
        /// </summary>
        Task<IRouteScriptInfo?> GetRouteInfo(string routeId);
        /// <summary>
        /// Returns the IRouteScriptInfo for the next route. You can pass a IRouteScriptInfo to get the route after the one provided.
        /// </summary>
        Task<IRouteScriptInfo?> GetNextRouteInfo(IOrderReader reader, IRouteScriptInfo? referenceRoute = null);
        /// <summary>
        /// Returns the IRouteScriptInfo that matches the order in it's current state
        /// </summary>
        Task<IRouteScriptInfo?> EvaluateRouteForOrder(IOrderReader order);

        /// <summary>
        /// Returns the tenant's configured time zone.
        /// </summary>
        Task<TimeZoneInfo> GetSystemTimeZone();

        /// <summary>
        /// Returns the time zone at location. If location is not valid, returns the system time zone.
        /// </summary>
        Task<TimeZoneInfo> GetTimeZoneAtLocation(LatLng location);

        Task<ScriptCell[]> GetSheet(string? sheetName = null);
    }
}
