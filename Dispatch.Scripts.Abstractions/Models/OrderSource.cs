namespace Dispatch.Scripts
{
    public enum OrderSource
    {
        Unknown = -1,
        APISelfServe = 0,
        APICSR,
        UISelfServe,
        UICSR,
        DeliveryApp,
        DriverApp,
        ImportSelfserve,
        ImportCSR,
        RecurringOrders,
        RecurringRoutes
    }
}