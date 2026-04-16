#nullable enable
using System;
using System.Threading.Tasks;

namespace Dispatch.Scripts;

public interface IOrderGroupReader
{
    string Id { get; }
    string Key { get; }
    Task<IOrderGroupDetailReader> GetDetail();
}

public interface IOrderGroupDetailReader
{
    string Id { get; }
    string Key { get; }
    int GroupTypeId { get; }
    DateTimeOffset CreationDate { get; }
    DateTimeOffset ExpiryDate { get; }
    bool IsActive { get; }
    IOrderGroupItemReader[] Items { get; }
}

public interface IOrderGroupItemReader
{
    string OrderId { get; }
    bool IsMasterOrder { get; }
}