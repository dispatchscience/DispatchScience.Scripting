using Dispatch.Measures;
using System.Collections.Generic;

namespace Dispatch.Scripts
{
    public class OrderItemInfo
    {
        public string Id { get; set; } = default!;
        public string ParcelTypeId { get; set; } = default!;
        public string? Description { get; set; }
        public Weight Weight { get; set; }
        public Length Length { get; set; }
        public Length Width { get; set; }
        public Length Height { get; set; }
        public string? BarcodeTemplate { get; set; }
        public IList<(string UserFieldId, string Value)> UserFields = new List<(string UserFieldId, string Value)>();
        public IList<OrderExceptionCodeInfo> ExceptionCodes = new List<OrderExceptionCodeInfo>();
    }
}