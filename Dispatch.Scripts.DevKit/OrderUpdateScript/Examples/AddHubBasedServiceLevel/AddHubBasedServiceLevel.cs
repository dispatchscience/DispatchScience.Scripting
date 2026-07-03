using Microsoft.Extensions.Logging;

namespace Dispatch.Scripts.DevKit.OrderUpdateScript.Examples.AddHubBasedServiceLevel
{
    public class AddHubBasedServiceLevel : IOrderUpdateScript
    {
        public async Task OnOrderUpdate(IOrderUpdater order, IScriptDataProvider data, ILogger logger)
        {
            if (order.FulfillmentType != OrderFulfillmentType.Standard)
            {
                // We can only add hubs to a Standard order
                return;
            }

            var scriptData = await data.GetScriptData();
            if (!scriptData.Any())
            {
                // This script needs data to work
                return;
            }

            var accountIds = scriptData["AccountIds"].Split(",");
            var serviceLevels = scriptData["ServiceLevelIds"].Split(",");
            var lat = double.Parse(scriptData["HubLocation"].Split(',')[0]);
            var lng = double.Parse(scriptData["HubLocation"].Split(',')[1]);

            var warehouse = new HubInfo
            {
                Address = new Address
                {
                    AddressLine1 = scriptData["HubAddressLine1"],
                    AddressLine2 = scriptData["HubAddressLine2"],
                    City = scriptData["HubCity"],
                    ZipPostalCode = scriptData["HubZip"],
                    StateProvince = scriptData["HubProvice"],
                    Company = scriptData["HubCompany"],
                    Location = new LatLng(lat, lng)
                },
                Contact = new ContactInfo { Name = scriptData["HubContactName"], PhoneNumber = scriptData["HubContactPhone"] }
            };

            if (!serviceLevels.Any(x => string.Equals(x?.Trim(), order.ServiceLevelTypeId, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            if (!accountIds.Any(x => string.Equals(x?.Trim(), order.AccountId, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            await order.AddHubs(warehouse);
        }
    }
}
