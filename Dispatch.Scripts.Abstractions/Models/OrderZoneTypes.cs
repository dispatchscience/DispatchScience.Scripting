using System;

namespace Dispatch.Scripts
{
    [Flags]
    public enum OrderZoneTypes
    {
        None = 0,
        Address = 1,
        AutoDispatch = 2,
        DriverPosition = 4,
        Filtering = 8,
        Taxes = 16,
        Script = 32,
        Other = 64
    }
}
