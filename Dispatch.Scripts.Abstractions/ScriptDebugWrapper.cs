using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Dispatch.Scripts.Abstractions
{
    public class ScriptDebugWrapper
    {
        public static string ScriptDataProvider = nameof(ScriptDataProvider);
        public static string OrderReader = nameof(OrderReader);
        public static string OrderId = nameof(OrderId);
        public static string MultiSegmentOrderId = nameof(MultiSegmentOrderId);
        public static string SegmentOrderIds = nameof(SegmentOrderIds);

        private IDictionary<string, object>? _debugData;
        private readonly JsonNode? _jsonData;

        public ScriptDebugWrapper()
        {
            _debugData = new Dictionary<string, object>();
        }

        public ScriptDebugWrapper(JsonNode jsonData)
        {
            _jsonData = jsonData;
        }

        public void AddScriptDataCall(string methodName, object value, params object[] args)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            var context = ScriptDataProvider;

            if (!_debugData.TryGetValue(context, out var data))
            {
                data = new ExpandoObject();
                _debugData.TryAdd(context, data);
            }

            var keyParts = new List<string> { methodName };
            if (args is not null)
            {
                keyParts.AddRange(args.Cast<string>());
            }
            var key = string.Join("|", keyParts);

            ((ExpandoObject)data).TryAdd(key, value);
        }

        public void AddProperty(string propertyName, object value)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            _debugData.TryAdd(propertyName, value);
        }

        public void AddOrderReader(string orderId, IOrderReader value)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            _debugData.TryAdd($"{OrderReader}_{orderId}", ToExpandoObject(value));
        }

        public T? GetScriptDataCall<T>(string methodName, params object[] args)
        {
            if (_jsonData is null)
            {
                throw new Exception("Cannot read data in this mode.");
            }

            var data = _jsonData[ScriptDataProvider];
            if (data is null)
            {
                return default;
            }

            var keyParts = new List<string> { methodName };
            keyParts.AddRange(args.Cast<string>());
            var key = string.Join("|", keyParts);

            var value = data[key];
            if (value is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value.ToJsonString());
        }

        public T GetProperty<T>(string propertyName)
        {
            if (_jsonData is null)
            {
                throw new Exception("Cannot read data in this mode.");
            }

            return _jsonData[propertyName]!.GetValue<T>();
        }

        public T? GetOrderReader<T>(string orderId)
        {
            if (_jsonData is null)
            {
                throw new Exception("Cannot read data in this mode.");
            }

            var data = _jsonData[$"{OrderReader}_{orderId}"];
            if (data is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(data.ToJsonString());
        }

        private ExpandoObject ToExpandoObject(object obj)
        {
            var expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                expando.TryAdd(property.Name, property.GetValue(obj));
            }

            return expando;
        }

        public dynamic GetData() => _debugData;
    }
}
