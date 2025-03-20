using System;

namespace Dispatch.Scripts
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MappedPropertyAttribute : Attribute
    {
        public char ListSeparator { get; set; }
        public string? ColumnName { get; set; }
    }
}
