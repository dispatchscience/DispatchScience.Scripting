#nullable enable
namespace Dispatch.Measures
{
    public static class VolumeExtensions
    {
        public static double CubicMeters(this Volume volume) => volume.ConvertTo(VolumeUnit.CubicMeter).Value;
        public static double CubicFeet(this Volume volume) => volume.ConvertTo(VolumeUnit.CubicFoot).Value;
    }
}