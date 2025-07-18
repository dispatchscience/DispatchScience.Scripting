using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dispatch.Scripts.Sample.Examples
{
    public class AddHubBasedOnPostalCode : IOrderUpdateScript
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

            var accountNumbers = scriptData["Comptes"].Split(",");
            var serviceLevels = scriptData["Niveau de service valide"].Split(",");
            var postalCodes = scriptData["Codes postals"].Split(",");
            var lat = double.Parse(scriptData["HubLocation"].Split(',')[0]);
            var lng = double.Parse(scriptData["HubLocation"].Split(',')[1]);

            var warehouse = new HubInfo
            {
                Address = new Address
                {
                    AddressLine1 = scriptData["HubAddressLine1"],
                    AddressLine2 = scriptData["HubAddressLine2"],
                    City = scriptData["HubCity"],
                    ZipPostalCode = scriptData["HubPostalCode"],
                    StateProvince = scriptData["HubProvice"],
                    Company = scriptData["HubCompany"],
                    Location = new LatLng(lat, lng)
                },
                Contact = new ContactInfo { Name = "Alex", Email = "alex@dsapp.io" }
            };

            if (!serviceLevels.Any(x => string.Equals(x?.Trim(), order.ServiceLevelTypeId, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            var orderPostalCode = string.Concat(order.DeliveryAddress.ZipPostalCode.Where(c => !char.IsWhiteSpace(c)));
            if (!postalCodes.Select(x => string.Concat(x.Where(c => !char.IsWhiteSpace(c)))).Any(x => orderPostalCode.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            var account = await data.GetAccountInfo();

            if (!accountNumbers.Any(x => string.Equals(x?.Trim(), account.Number, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            await order.AddHubs(warehouse);
        }
    }
}
