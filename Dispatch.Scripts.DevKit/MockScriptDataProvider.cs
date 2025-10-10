using Dispatch.Scripts.Abstractions;

namespace Dispatch.Scripts.DevKit
{
    internal class MockScriptDataProvider : IScriptDataProvider
    {
        private readonly ScriptDebugWrapper _scriptDebugWrapper;
        private readonly bool _forceRerunMapSheet;

        public MockScriptDataProvider(ScriptDebugWrapper scriptDebugWrapper, bool forceRerunMapSheet)
        {
            _scriptDebugWrapper = scriptDebugWrapper;
            _forceRerunMapSheet = forceRerunMapSheet;
        }

        public Task<IRouteScriptInfo?> EvaluateRouteForOrder(IOrderReader order)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IRouteScriptInfo>(nameof(EvaluateRouteForOrder), order.OrderId));

        public Task<IRouteScriptInfo?> GetNextRouteInfo(IOrderReader reader, IRouteScriptInfo? referenceRoute = null)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IRouteScriptInfo>(nameof(GetNextRouteInfo), reader.OrderId, referenceRoute?.Id));

        public Task<AccountDiscountInfo?> FindAccountExtraFeeDiscount(string serviceLevelTypeId, string extraFeeTypeId)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<AccountDiscountInfo>(nameof(FindAccountExtraFeeDiscount), serviceLevelTypeId, extraFeeTypeId));

        public Task<AccountDiscountInfo?> FindAccountPriceListDiscount(string serviceLevelTypeId)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<AccountDiscountInfo>(nameof(FindAccountPriceListDiscount), serviceLevelTypeId));

        public Task<IList<Driver>> FindDrivers(string driverNumber)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<Driver>>(nameof(FindDrivers), driverNumber) ?? []);

        public Task<IList<HubInfo>> FindHubs(string hubName)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<HubInfo>>(nameof(FindHubs), hubName) ?? []);

        public Task<ServiceLevelInfo?> FindServiceLevel(int? serviceLevelScheduleId, string? serviceLevelTypeId)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<ServiceLevelInfo>(nameof(FindServiceLevel), serviceLevelScheduleId, serviceLevelTypeId));

        public Task<IDictionary<string, string>> GetAccountFields()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IDictionary<string, string>>(nameof(GetAccountFields)) ?? new Dictionary<string, string>());

        public Task<AccountScriptInfo?> GetAccountInfo()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<AccountScriptInfo>(nameof(GetAccountInfo)));

        public Task<IList<string>> GetAvailableExtraFeeTypes()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<string>>(nameof(GetAvailableExtraFeeTypes)) ?? []);

        public Task<IList<ParcelTypeInfo>> GetAvailableParcelTypes()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<ParcelTypeInfo>>(nameof(GetAvailableParcelTypes)) ?? []);

        public Task<IDictionary<string, string>> GetDriverFields(string driverId)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IDictionary<string, string>>(nameof(GetDriverFields), driverId) ?? new Dictionary<string, string>());

        public Task<IList<string>> GetEventNames()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<string>>(nameof(GetEventNames)) ?? []);

        public Task<HolidayInfo> GetHoliday(DateTimeOffset date)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<HolidayInfo>(nameof(GetHoliday), date) ?? new());

        public Task<MasterAccountScriptInfo?> GetMasterAccountInfo()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<MasterAccountScriptInfo>(nameof(GetMasterAccountInfo)));

        public Task<IRouteScriptInfo?> GetRouteInfo(string routeId)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IRouteScriptInfo>(nameof(GetRouteInfo), routeId));

        public Task<IDictionary<string, string>> GetScriptData()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IDictionary<string, string>>(nameof(GetScriptData)) ?? new Dictionary<string, string>());

        public Task<ScriptCell[]> GetSheet(string? sheetName = null)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<ScriptCell[]>(nameof(GetSheet), sheetName) ?? []);

        public async Task<T[]> GetSheet<T>(string? sheetName = null, Action<(T DataObject, IGrouping<int?, ScriptCell> RowData, (string Name, string RawName, int Number)[] AvailableColumns)>? additionalInitializer = null) where T : IScriptData, new()
        {
            if (!_forceRerunMapSheet)
            {
                var nativeData = _scriptDebugWrapper.GetScriptDataCall<T[]>(nameof(GetSheet), typeof(T).Name, sheetName);
                if (nativeData is not null)
                {
                    return await Task.FromResult(nativeData);
                }
            }

            // Note: when using this workaround, performance is affected and might not represent reality because normally, script data is cached. It will represent a dry run.
            var legacyData = await GetSheet(sheetName);
            return Helpers.MapSheet(legacyData, _scriptDebugWrapper.Logger, additionalInitializer);
        }

        public Task<TimeZoneInfo> GetSystemTimeZone()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<TimeZoneInfo>(nameof(GetSystemTimeZone)) ?? TimeZoneInfo.Local);

        public Task<TimeZoneInfo> GetTimeZoneAtLocation(LatLng location)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<TimeZoneInfo>(nameof(GetTimeZoneAtLocation), location) ?? TimeZoneInfo.Local);

        public Task<IList<string>> GetZones(ScriptZoneType zoneType, LatLng position)
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<IList<string>>(nameof(GetZones), zoneType, position) ?? []);

        public Task<ScriptExecutionContext> GetExecutionContext()
            => Task.FromResult(_scriptDebugWrapper.GetScriptDataCall<ScriptExecutionContext>(nameof(GetExecutionContext)) ?? new ScriptExecutionContext());
    }
}
