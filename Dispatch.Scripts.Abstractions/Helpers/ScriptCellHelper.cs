using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Dispatch.Scripts
{
    public class Helpers
    {
        public static short ConvertToShort(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0;
            }

            if (!short.TryParse(cellValue, out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to short.");
                return 0;
            }

            return result;
        }

        public static int ConvertToInt(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0;
            }

            if (!int.TryParse(cellValue, out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to int.");
                return 0;
            }

            return result;
        }

        public static long ConvertToLong(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0;
            }

            if (!long.TryParse(cellValue, out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to long.");
                return 0;
            }

            return result;
        }

        public static decimal ConvertToDecimal(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0m;
            }

            if (!decimal.TryParse(cellValue, NumberStyles.Any, GetCultureInfoForNumberParsing(cellValue), out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to decimal.");
                return 0m;
            }

            return result;
        }

        public static double ConvertToDouble(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0d;
            }

            if (!double.TryParse(cellValue, NumberStyles.Any, GetCultureInfoForNumberParsing(cellValue), out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to double.");
                return 0d;
            }

            return result;
        }

        public static double ConvertToFloat(string? cellValue, ILogger logger)
        {
            cellValue = KeepNumericValues(cellValue);

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return 0f;
            }

            if (!float.TryParse(cellValue, NumberStyles.Any, GetCultureInfoForNumberParsing(cellValue), out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to float.");
                return 0f;
            }

            return result;
        }

        public static bool ConvertToBoolean(string? cellValue, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return false;
            }

            if (!bool.TryParse(cellValue, out var result))
            {
                logger.LogWarning($"Could not parse value '{cellValue}' to boolean.");
                return false;
            }

            return result;
        }

        public static T[] MapSheet<T>(ScriptCell[] sheetItems, ILogger logger, Action<(T DataObject, IGrouping<int?, ScriptCell> RowData, (string Name, int Number)[] AvailableColumns)>? additionalInitializer = null) where T : IScriptData, new()
        {
            if (sheetItems is null || sheetItems.Length == 0)
            {
                return Array.Empty<T>();
            }

            var result = new List<T>();

            var properties = typeof(T).GetProperties();

            var columns = sheetItems
                .Where(x => !string.IsNullOrWhiteSpace(x.ColumnName))
                .Select(x => (Name: x.ColumnName!, Number: x.ColumnNumber!.Value))
                .Distinct()
                .OrderBy(x => x.Number)
                .ToArray();

            // We only take cells for row > 1, assuming row 1 is the header
            foreach (var row in sheetItems.Where(x => x.RowNumber > 1).GroupBy(x => x.RowNumber))
            {
                var item = new T();

                foreach (var property in properties)
                {
                    var columnName = GetColumnNameToUse(property);

                    if (string.Equals(columnName, nameof(IScriptData.RowNumber), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (row.Key.HasValue)
                        {
                            item.RowNumber = row.Key.Value;
                        }
                    }
                    else
                    {
                        var val = GetValueFromSheet(row.FirstOrDefault(x => columnName.Equals(x.ColumnName, StringComparison.CurrentCultureIgnoreCase))?.CellValue, property.PropertyType, logger, property);
                        if (val is not null)
                        {
                            if (property.SetMethod is not null)
                            {
                                property.SetValue(item, val);
                            }
                        }
                    }
                }

                if (additionalInitializer is not null)
                {
                    additionalInitializer((item, row, columns));
                }

                result.Add(item);
            }

            return result.ToArray();
        }

        private static string GetColumnNameToUse(PropertyInfo property)
        {
            var mappedPropertyAttribute = property.GetCustomAttribute<MappedPropertyAttribute>();
            return mappedPropertyAttribute?.ColumnName ?? property.Name;
        }

        private static CultureInfo GetCultureInfoForNumberParsing(string cellValue)
        {
            var cultureInfo = new CultureInfo("en", useUserOverride: true) { NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "." } };
            if (cellValue.Split(',').Length == 2)
            {
                cultureInfo = new CultureInfo("en", useUserOverride: true) { NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = "," } };
            }

            return cultureInfo;
        }

        private static string? KeepNumericValues(string? value)
        {
            var digits = value?.Where(x => char.IsDigit(x) || x == '.' || x == ',').ToArray();
            if (digits is null || !digits.Any())
            {
                return null;
            }

            return new string(digits);
        }

        private static Dictionary<Type, Func<string?, ILogger, object?>> _converters = new Dictionary<Type, Func<string?, ILogger, object?>>
        {
            { typeof(short), (x, logger) => ConvertToShort(x, logger) },
            { typeof(int), (x, logger) => ConvertToInt(x, logger) },
            { typeof(long), (x, logger) => ConvertToLong(x, logger) },
            { typeof(decimal), (x, logger) => ConvertToDecimal(x, logger) },
            { typeof(double), (x, logger) => ConvertToDouble(x, logger) },
            { typeof(float), (x, logger) => ConvertToFloat(x, logger) },
            { typeof(bool), (x, logger) => ConvertToBoolean(x, logger) },
            { typeof(string), (x, logger) => x }
        };

        private static object? GetValueFromSheet(string? cellValue, Type propertyType, ILogger logger, PropertyInfo? propertyInfo = null)
        {
            if (string.IsNullOrWhiteSpace(cellValue))
            {
                return GetDefault(propertyType, propertyInfo, logger);
            }

            var inputType = propertyType;
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            if (underlyingType is not null)
            {
                propertyType = underlyingType;
            }

            if (propertyType.IsArray)
            {
                var elementType = propertyType.GetElementType()!;
                var values = GetSeparatedObjects(cellValue, elementType, propertyInfo, logger);
                if (values.Length > 0)
                {
                    var array = Array.CreateInstance(elementType, values.Length);
                    for (var index = 0; index < values.Length; index++)
                    {
                        array.SetValue(values.ElementAt(index), index);
                    }
                    return array;
                }
            }

            if (typeof(IList).IsAssignableFrom(propertyType) && propertyType.IsGenericType)
            {
                var elementType = propertyType.GetGenericArguments()[0];
                var values = GetSeparatedObjects(cellValue, elementType, propertyInfo, logger);
                if (values.Length > 0)
                {
                    var list = (IList?)Activator.CreateInstance(propertyType);
                    if (list is not null)
                    {
                        for (var index = 0; index < values.Length; index++)
                        {
                            list.Add(values.ElementAt(index));
                        }
                    }
                    return list;
                }
            }

            if (_converters.TryGetValue(propertyType, out Func<string?, ILogger, object?>? converter))
            {
                return converter(cellValue, logger);
            }

            return GetDefault(inputType, propertyInfo, logger);
        }

        private static object?[] GetSeparatedObjects(string cellValue, Type elementType, PropertyInfo? propertyInfo, ILogger logger)
        {
            return cellValue
                .Split(GetListSeparator(cellValue, propertyInfo), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => GetValueFromSheet(x, elementType, logger))
                .ToArray();
        }

        private static char GetListSeparator(string cellValue, PropertyInfo? propertyInfo)
        {
            char? separator = null;

            if (propertyInfo is not null)
            {
                var attribute = propertyInfo.GetCustomAttribute<MappedPropertyAttribute>();
                if (attribute?.ListSeparator is not null)
                {
                    separator = attribute?.ListSeparator;
                }
            }

            if (separator is null)
            {
                separator = cellValue.Contains("-") ? '-' : ',';
            }

            return separator.Value;
        }

        private static object? GetDefault(Type type, PropertyInfo? propertyInfo, ILogger logger)
        {
            var isNullable = Nullable.GetUnderlyingType(type) is not null;
            if (isNullable)
            {
                return null;
            }

            if (propertyInfo is not null && IsMarkedAsNullable(propertyInfo))
            {
                return null;
            }

            var isArray = type.IsArray;
            if (isArray)
            {
                return Array.CreateInstance(type.GetElementType()!, 0);
            }

            try
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while trying to get default value for {(propertyInfo?.Name ?? type.Name)}, returning null.");
                return null;
            }
        }

        private static bool IsMarkedAsNullable(PropertyInfo property) => new NullabilityInfoContext().Create(property).WriteState is NullabilityState.Nullable;
    }
}
