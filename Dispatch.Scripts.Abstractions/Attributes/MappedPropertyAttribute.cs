using System;

namespace Dispatch.Scripts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MappedClassAttribute : Attribute
    {
        public required string[] StringsToIgnoreInColumnNames { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class MappedPropertyAttribute : Attribute
    {
        public char ListSeparator { get; set; }
        public string? ColumnName { get; set; }
    }
}
