using System;

namespace Dispatch.Scripts
{
    public interface IRouteScriptInfo
    {
        string Name { get;}
        string Id { get; }
        string ContainerId { get; }
        int RoutePlanId { get; }
        string? ZoneId { get;}
        DateTimeOffset StartTime { get; }
        DateTimeOffset OrderCreationCutoffTime { get; }
        DateTimeOffset PickupCutoffTime { get; }
        bool OrdersCanBeAdded { get; }
        bool Exists { get; }
    }
}