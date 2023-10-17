using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dispatch.Measures;

namespace Dispatch.Scripts.Sample.Examples
{
    internal class AutomaticHubCreationOnOrderUpdate
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (order.FulfillmentType != OrderFulfillmentType.Standard || order.Status != OrderStatus.Received || order.AssignedDriver != null)
            {
                return;
            }

            var pickupZone = (await data.GetZones(ScriptZoneType.Address, order.PickupAddress.Location)).FirstOrDefault();
            var deliveryZone = (await data.GetZones(ScriptZoneType.Address, order.DeliveryAddress.Location)).FirstOrDefault();
            var serviceLevel = order.ServiceLevelTypeId;

            logger.LogInformation($"Zone information. Pickup zone = {pickupZone}, Delivery zone {deliveryZone}.  Service level = {serviceLevel}");

            var hubInfoSheet = await data.GetSheet();
            if (hubInfoSheet == null || !hubInfoSheet.Any())
            {
                logger.LogInformation("No pricing data found");
                return;
            }

            var allHubsConfig = MapSheet<HubData>(hubInfoSheet, null);
            var hubConfigForTheOrder = allHubsConfig.OrderBy(x => x.RowNumber).FirstOrDefault(x => x.IsMatchingOrder(pickupZone, deliveryZone, serviceLevel));
            if (hubConfigForTheOrder == null)
            {
                logger.LogInformation($"No match in the hub configuration file for pickup zone {pickupZone} and delivery zone {deliveryZone}");
                return;
            }

            logger?.LogInformation("Hub config for this order : ");
            logger?.LogInformation(JsonSerializer.Serialize(hubConfigForTheOrder));

            var drivers = new List<string>();
            var activeHubs = await GetActiveHubs(data, logger, (hubConfigForTheOrder.Hub1, hubConfigForTheOrder.Driver1, hubConfigForTheOrder.ServiceLevel1, hubConfigForTheOrder.RoutePlanId1, hubConfigForTheOrder.RouteName1, true, hubConfigForTheOrder.Driver2, hubConfigForTheOrder.ServiceLevel2, hubConfigForTheOrder.RoutePlanId2, hubConfigForTheOrder.RouteName2),
                                                         (hubConfigForTheOrder.Hub2, hubConfigForTheOrder.Driver3, hubConfigForTheOrder.ServiceLevel3, hubConfigForTheOrder.RoutePlanId3, hubConfigForTheOrder.RouteName3, false, null, null, null, null),
                                                         (hubConfigForTheOrder.Hub3, hubConfigForTheOrder.Driver4, hubConfigForTheOrder.ServiceLevel4, hubConfigForTheOrder.RoutePlanId4, hubConfigForTheOrder.RouteName4, false, null, null, null, null),
                                                         (hubConfigForTheOrder.Hub4, hubConfigForTheOrder.Driver5, hubConfigForTheOrder.ServiceLevel5, hubConfigForTheOrder.RoutePlanId5, hubConfigForTheOrder.RouteName5, false, null, null, null, null),
                                                         (hubConfigForTheOrder.Hub5, hubConfigForTheOrder.Driver6, hubConfigForTheOrder.ServiceLevel6, hubConfigForTheOrder.RoutePlanId6, hubConfigForTheOrder.RouteName6, false, null, null, null, null));

            logger.LogInformation($"Configuration contains {activeHubs.Count()} hub(s).");

            if (activeHubs.Count() == 0)
            {
                if (!string.IsNullOrWhiteSpace(hubConfigForTheOrder.Driver1))
                {
                    var driverId = await GetDriverId(data, hubConfigForTheOrder.Driver1, logger);
                    if (!string.IsNullOrWhiteSpace(driverId))
                    {
                        logger.LogInformation($"Assigning driver {driverId} to order.");
                        await order.AssignDriver(driverId);
                    }
                }
                return;
            }
            else
            {

                logger?.LogInformation("Adding hubs..");
                logger?.LogInformation(JsonSerializer.Serialize(activeHubs));
                try
                {
                    await order.AddHubs(activeHubs);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, $"Can't create hub for order {order.OrderId}, error {ex.Message}");
                }
            }
        }

        private async static Task<HubInfo[]> GetActiveHubs(IScriptDataProvider data, ILogger logger, params (string hubName, string driver1, string serviceLevel1, string routePlanId1, string routeName1,
                                                                                                             bool includeSegment2Info, string driver2, string serviceLevel2, string routePlanId2, string routeName2)[] hubsAndSegmentsInfo)
        {
            var activeHubs = new List<HubInfo>();
            foreach (var hub in hubsAndSegmentsInfo)
            {
                var hubInfo = await GetHub(data, logger, hub.hubName);
                if (hubInfo == null)
                {
                    return activeHubs.ToArray();
                }

                var driverId1 = await GetDriverId(data, hub.driver1, logger);
                if (hub.includeSegment2Info)
                {
                    var driverId2 = await GetDriverId(data, hub.driver2, logger);
                    hubInfo.SegmentOverrideInfos = new SegmentOverrideInfo[] { new SegmentOverrideInfo { DriverId = driverId1, ServiceLevelTypeId = hub.serviceLevel1, RoutePlanId = ParseInt(hub.routePlanId1), RouteName = hub.routeName1 },
                                                                        new SegmentOverrideInfo { DriverId = driverId2, ServiceLevelTypeId = hub.serviceLevel2, RoutePlanId = ParseInt(hub.routePlanId2), RouteName = hub.routeName2 } };
                }
                else
                {
                    hubInfo.SegmentOverrideInfos = new SegmentOverrideInfo[] { new SegmentOverrideInfo { DriverId = driverId1, ServiceLevelTypeId = hub.serviceLevel1, RoutePlanId = ParseInt(hub.routePlanId1), RouteName = hub.routeName1 } };
                }
                activeHubs.Add(hubInfo);
            }
            return activeHubs.ToArray();
        }

        private static int? ParseInt(string routePlanId1)
        {
            if (string.IsNullOrWhiteSpace(routePlanId1) || !int.TryParse(routePlanId1, out int parsedValue))
            {
                return null;
            }
            return parsedValue;
        }

        private static async Task<HubInfo> GetHub(IScriptDataProvider data, ILogger logger, string hubName)
        {
            HubInfo hub = null;
            if (!string.IsNullOrWhiteSpace(hubName))
            {
                var hubInfo = await data.FindHubs(hubName);
                logger.LogInformation($"Searching for hub {hubName}, result :{hubInfo.Count}, first or default {hubInfo.FirstOrDefault()?.HubId}");
                if (hubInfo != null && hubInfo.Count == 1)
                {
                    hub = hubInfo.Single();
                }
            }
            return hub;
        }



        private static async Task<string> GetDriverId(IScriptDataProvider data, string driverNumer, ILogger logger)
        {
            if (data == null || string.IsNullOrWhiteSpace(driverNumer))
            {
                return null;
            }

            var drivers = await data.FindDrivers(driverNumer);
            if (drivers == null || drivers.Count != 1)
            {
                logger?.LogInformation($"No driver or multiple driver found for driver number {driverNumer}");
                return null;
            }
            return drivers.Single().Id;
        }

        public class HubData
        {
            public bool IsMatchingOrder(string orderPickupZone, string orderDeliveryZone, string orderServiceLevel)
            {
                return (string.Equals(PickupZone, orderPickupZone, StringComparison.InvariantCultureIgnoreCase) || PickupZone == "*") &&
                       (string.Equals(DeliveryZone, orderDeliveryZone, StringComparison.InvariantCultureIgnoreCase) || DeliveryZone == "*") &&
                       (string.Equals(ServiceLevel, orderServiceLevel, StringComparison.InvariantCultureIgnoreCase) || ServiceLevel == "*"); ;
            }
            public int RowNumber { get; set; }
            public int ColumnNumber { get; set; }
            public string PickupZone { get; set; }
            public string DeliveryZone { get; set; }
            public string ServiceLevel { get; set; }

            public string Hub1 { get; set; }
            public string Hub2 { get; set; }
            public string Hub3 { get; set; }
            public string Hub4 { get; set; }
            public string Hub5 { get; set; }
            public string Driver1 { get; set; }
            public string Driver2 { get; set; }
            public string Driver3 { get; set; }
            public string Driver4 { get; set; }
            public string Driver5 { get; set; }
            public string Driver6 { get; set; }
            public string ServiceLevel1 { get; set; }
            public string ServiceLevel2 { get; set; }
            public string ServiceLevel3 { get; set; }
            public string ServiceLevel4 { get; set; }
            public string ServiceLevel5 { get; set; }
            public string ServiceLevel6 { get; set; }
            public string RoutePlanId1 { get; set; }
            public string RoutePlanId2 { get; set; }
            public string RoutePlanId3 { get; set; }
            public string RoutePlanId4 { get; set; }
            public string RoutePlanId5 { get; set; }
            public string RoutePlanId6 { get; set; }
            public string RouteName1 { get; set; }
            public string RouteName2 { get; set; }
            public string RouteName3 { get; set; }
            public string RouteName4 { get; set; }
            public string RouteName5 { get; set; }
            public string RouteName6 { get; set; }
        }

        private T[] MapSheet<T>(IEnumerable<ScriptCell> sheet, ILogger logger = null) where T : new()
        {
            var result = new List<T>();
            if (sheet == null || !sheet.Any())
            {
                return result.ToArray();
            }

            var properties = typeof(T).GetProperties();

            foreach (var row in sheet.Where(x => x.RowNumber > 1).GroupBy(x => x.RowNumber))
            {
                var item = new T();
                foreach (var prop in properties)
                {
                    if (string.Equals(prop.Name, "RowNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (row.Key.HasValue)
                        {
                            prop.SetValue(item, row.Key.Value);
                        }
                    }
                    else if (string.Equals(prop.Name, "ColumnNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (row.First().ColumnNumber.HasValue)
                        {
                            prop.SetValue(item, row.First().ColumnNumber.Value);
                        }
                    }
                    else
                    {
                        var val = GetValueFromSheet(row.FirstOrDefault(x => x.ColumnName == prop.Name), prop.PropertyType);
                        if (val != null)
                        {
                            prop.SetValue(item, val);
                        }
                    }
                }
                result.Add(item);
            }

            logger?.LogInformation(JsonSerializer.Serialize(result));
            return result.ToArray();
        }

        private object? GetValueFromSheet(ScriptCell? extraFeeScriptCell, Type propertyType)
        {
            if (extraFeeScriptCell == null)
            {
                return null;
            }
            var converters = new Dictionary<Type, Func<ScriptCell, object>>();
            converters.Add(typeof(System.Int32), x => ConvertToInt(x));
            converters.Add(typeof(System.Decimal), x => ConvertToDecimal(x));
            converters.Add(typeof(System.String), x => x.CellValue);

            if (converters.TryGetValue(propertyType, out Func<ScriptCell, object> converter))
            {
                return converter(extraFeeScriptCell);
            }

            return null;
        }

        private decimal GetItemWeight(OrderItemInfo item, IList<ParcelTypeInfo> ptypes)
        {
            var p = ptypes.FirstOrDefault(x => string.Equals(x.Id, item.ParcelTypeId, StringComparison.InvariantCultureIgnoreCase));
            if (p == null || !p.IsDimensionalWeight)
            {
                return Convert.ToDecimal(item.Weight.Pounds());
            }

            var dimWeight = p.GetDimensionalWeight(item).Value.Pounds();
            if (dimWeight < item.Weight.Pounds())
            {
                return Convert.ToDecimal(item.Weight.Pounds());
            }
            else
            {
                return Convert.ToDecimal(dimWeight);
            }
        }

        private int ConvertToInt(ScriptCell cell)
        {
            string cellValue = cell?.CellValue;
            int result;

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0;
            }

            if (!int.TryParse(cellValue, out result))
            {
                return 0;
            }
            return result;
        }

        private decimal ConvertToDecimal(ScriptCell cell)
        {
            string cellValue = cell?.CellValue;
            decimal result;

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0;
            }

            var cultureInfo = new System.Globalization.CultureInfo("en", true) { NumberFormat = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "." } };
            if (cellValue.Split(',').Length == 2)
            {
                cultureInfo = new System.Globalization.CultureInfo("en", true) { NumberFormat = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = "," } };
            }

            if (!decimal.TryParse(cellValue, System.Globalization.NumberStyles.Any, cultureInfo, out result))
            {
                return 0;
            }
            return result;
        }
    }
}
