using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.Sample.Examples
{
    public class ComplexOffHourOrderUpdate
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            var scriptData = await data.GetScriptData();
            if (scriptData == null || !scriptData.Any())
            {
                return;
            }

            var rulesCount = scriptData.Keys.Where(x => x.Contains('_')).Select(x => x.Split('_')[0]).Max(x => int.Parse(x.Substring(1, x.Length - 1)));
            var orderHoliday = await data.GetHoliday(order.DeliveryWindow.To);
            var orderIsOnAHoliday = orderHoliday.IsHoliday;
            var offHourExtraCharge = order.ExtraFees.FirstOrDefault(e => e.ExtraFeeTypeId == "OffHourExtra");
            decimal offHourExtra = 0;

            for (int i = 1; i <= rulesCount; i++)
            {
                scriptData.TryGetValue($"R{i}_StartTime", out string startTime);
                scriptData.TryGetValue($"R{i}_EndTime", out string endTime);
                scriptData.TryGetValue($"R{i}_DayOfWeek", out string dayOfWeek);
                scriptData.TryGetValue($"R{i}_Holiday", out string holiday);
                scriptData.TryGetValue($"R{i}_Vehicle", out string vehicle);
                scriptData.TryGetValue($"R{i}_ServiceTypes", out string serviceTypes);
                scriptData.TryGetValue($"R{i}_FlatCharge", out string flatCharge);
                scriptData.TryGetValue($"R{i}_PercentOfBase", out string percentOfBase);
                scriptData.TryGetValue($"R{i}_TotalChargeMinimum", out string totalChargeMinimum);
                scriptData.TryGetValue($"R{i}_OrderCode", out string orderCode);

                logger.LogInformation($"Rule #{i}-{startTime}-{endTime}-{dayOfWeek}-{holiday}-{vehicle}-{serviceTypes}-{flatCharge}-{percentOfBase}-{totalChargeMinimum}-{orderCode}.");

                if (string.IsNullOrWhiteSpace(startTime) ||
                    string.IsNullOrWhiteSpace(endTime) ||
                    string.IsNullOrWhiteSpace(dayOfWeek) ||
                    string.IsNullOrWhiteSpace(holiday) ||
                    string.IsNullOrWhiteSpace(vehicle) ||
                    string.IsNullOrWhiteSpace(serviceTypes) ||
                    string.IsNullOrWhiteSpace(flatCharge) ||
                    string.IsNullOrWhiteSpace(percentOfBase) ||
                        string.IsNullOrWhiteSpace(totalChargeMinimum))

                {
                    logger.LogInformation($"Rule #{i} definition is incomplete.");
                }

                logger.LogInformation($"Rule #{i} (Order=RuleValue) : OrderCode{order.ReferenceNumber1}={orderCode}, Holiday {orderIsOnAHoliday.ToString()}={holiday}, Vehilce {order.VehicleTypeId}={vehicle}, ServiceLevel {order.ServiceLevelTypeId}={serviceTypes}, Vehilce {order.VehicleTypeId}={vehicle},, Vehilce {order.VehicleTypeId}={vehicle}");
                logger.LogInformation($"Rule #{i} (Is Time between) : {IsTimeBetween(order.DeliveryWindow.To, startTime, endTime)}, {order.DeliveryWindow.To}, {startTime}, {endTime}");
                logger.LogInformation($"Rule #{i} (IsOneOfTheDay) : {IsOneOfTheDay(order.DeliveryWindow.To, dayOfWeek)}");
               
                if (IsEqual(order.ReferenceNumber1, orderCode) &&
                        IsEqual(orderIsOnAHoliday.ToString(), holiday) &&
                        (IsEqual("ANY", vehicle) || IsEqual(order.VehicleTypeId, vehicle)) &&
                        (IsEqual("ANY", serviceTypes) || IsEqual(order.ServiceLevelTypeId, serviceTypes)) &&
                        IsTimeBetween(order.DeliveryWindow.To, startTime, endTime) &&
                        IsOneOfTheDay(order.DeliveryWindow.To, dayOfWeek))
                {
                    logger.LogInformation($"Rule #{i} matched!");

                    if (decimal.TryParse(flatCharge, out decimal flatChargeExtra))
                    {
                        offHourExtra += flatChargeExtra;
                    }

                    if (decimal.TryParse(percentOfBase, out decimal percentOfBaseExtra))
                    {
                        var totalDelivery = order.DeliveryCharges.Sum(x => x.TotalPrice);
                        offHourExtra += (percentOfBaseExtra / 100) * totalDelivery;
                    }

                    logger.LogInformation(order.TotalOrderPrice.ToString());
                   
                    if (decimal.TryParse(totalChargeMinimum, out decimal totalChargeMinimumExtra))
                    {
                        var offHoursAlreadyApplied = offHourExtraCharge == null ? 0 : offHourExtraCharge.TotalPrice;
                        var totalOrderPriceWithoutOffHourFee = order.TotalOrderPrice - offHoursAlreadyApplied;

                        if (totalOrderPriceWithoutOffHourFee + offHourExtra < totalChargeMinimumExtra)
                        {
                            offHourExtra = totalChargeMinimumExtra - totalOrderPriceWithoutOffHourFee;
                        }
                    }
                    break;
                }
            }
            
            if (offHourExtraCharge == null && offHourExtra > 0)
            {
                await order.AddExtraFee("OffHourExtra", 1, offHourExtra);
            }
            else if (offHourExtraCharge != null)
            {
                await order.UpdateExtraFee(offHourExtraCharge.ChargeId, 1, offHourExtra);
            }
        }

        private bool IsEqual(string a, string b)
        {
            return string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsTimeBetween(DateTimeOffset date, string startTime, string endTime)
        {
            if (!TimeSpan.TryParse(startTime, out TimeSpan startTimeSpan))
            {
                return false;
            }

            if (!TimeSpan.TryParse(endTime, out TimeSpan endTimeSpan))
            {
                return false;
            }
            return (date.TimeOfDay > startTimeSpan && date.TimeOfDay < endTimeSpan);
        }

        private bool IsOneOfTheDay(DateTimeOffset date, string dayOfWeek)
        {
            return dayOfWeek.Contains(date.DayOfWeek.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
