namespace Dispatch.Scripts
{
    public class Address
    {
        public string AddressLine1 { get; set; } = default!;

        public string? AddressLine2 { get; set; }

        public string? Company { get; set; }

        public string City { get; set; } = default!;

        public string ZipPostalCode { get; set; } = default!;

        public string? StateProvince { get; set; }

        public LatLng Location { get; set; }
    }
}