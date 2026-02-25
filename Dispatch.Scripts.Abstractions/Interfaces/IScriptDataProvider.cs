using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts
{
    public interface IScriptDataProvider
    {
        [Obsolete("Use GetContext().OrderUpdate.Data instead.")]
        Task<IDictionary<string, string>> GetScriptData();
        Task<IList<string>> GetEventNames();

        Task<IDictionary<string, string>> GetAccountFields();
        Task<AccountScriptInfo?> GetAccountInfo();
        Task<AccountDiscountInfo?> FindAccountExtraFeeDiscount(string serviceLevelTypeId, string extraFeeTypeId);
        Task<AccountDiscountInfo?> FindAccountPriceListDiscount(string serviceLevelTypeId);
        Task<ServiceLevelInfo?> FindServiceLevel(int? serviceLevelScheduleId, string? serviceLevelTypeId);
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

        /// <summary>
        /// Get script sheet data by mapping columns to properties of the user-defined T class.
        /// </summary>
        /// <typeparam name="T">User-defined class that must implement IScriptData</typeparam>
        /// <param name="sheetName">The sheet name to load, if none is provided, it assumes there's only one uploaded and will load it.</param>
        /// <param name="additionalInitializer">If you need to do additional initialization of your data objects, this is where to do it. Please note that the initialized data will be kept in cache, meaning the initializer should be idempotent. If you need to have different initializer, you should use a different user-defined T class.</param>
        /// <returns>The script data.</returns>
        Task<T[]> GetSheet<T>(string? sheetName = null, Action<(T DataObject, IGrouping<int?, ScriptCell> RowData, (string Name, string RawName, int Number)[] AvailableColumns)>? additionalInitializer = null) where T : IScriptData, new();

        /// <summary>
        /// Returns information about the execution of the script such as the script id, the extra fee, etc... Values could be null if not available for script type.
        /// </summary>
        [Obsolete("Use GetContext() instead.")]
        Task<ScriptExecutionContext> GetExecutionContext();

        /// <summary>
        /// Returns information about the execution of the script such as the script id, the extra fee, etc... Values could be null if not available for script type.
        /// </summary>
        Task<IScriptExecutionContext> GetContext();
    }
}
