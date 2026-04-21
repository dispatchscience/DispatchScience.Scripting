using System.Threading.Tasks;

namespace Dispatch.Scripts;

public interface IOrderGroupUpdater : IOrderGroupReader
{
    new Task<IOrderGroupDetailUpdater> GetDetail();

    /// <summary>
    /// Remove the order from this group.
    /// </summary>
    Task RemoveOrder(string orderId);

    /// <summary>
    /// Set the order as the master order of this group.
    /// The order must already be part of the group.
    /// </summary>
    Task SetAsMasterOrder(string orderId);

    /// <summary>
    /// Close this group. Closed groups cannot receive new orders via automatic assignment, but orders can still be added manually.
    /// </summary>
    Task Close();
}

public interface IOrderGroupDetailUpdater : IOrderGroupDetailReader
{
    new IOrderGroupItemUpdater[] Items { get; }
}

public interface IOrderGroupItemUpdater : IOrderGroupItemReader
{
    Task<IOrderUpdater> GetOrder();
}
