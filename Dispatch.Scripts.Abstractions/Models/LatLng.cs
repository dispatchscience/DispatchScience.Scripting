using System;

namespace Dispatch.Scripts
{
    public struct LatLng
    {
        public LatLng(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }

        public double Longitude { get; }

        public bool IsValid
        {
            get
            {
                if (Latitude == 0d && Longitude == 0d)
                {
                    return false;
                }

                return Math.Abs(Latitude) <= 90d && Math.Abs(Longitude) <= 180d;
            }
        }
    }
}