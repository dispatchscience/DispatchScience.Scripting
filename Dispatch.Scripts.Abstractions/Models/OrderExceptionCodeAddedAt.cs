using System.Runtime.Serialization;
namespace Dispatch.Scripts
{
    public enum OrderExceptionCodeAddedAt
    {
        [EnumMember(Value = "0")]
        Checkpoint = 0,
        [EnumMember(Value = "1")]
        PickedUp,
        [EnumMember(Value = "2")]
        Delivered
    }
}
