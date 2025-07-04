using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text.Json.Nodes;

namespace Dispatch.Scripts.Abstractions
{
    public class ScriptDebugWrapper
    {
        public static string ScriptDataProvider = nameof(ScriptDataProvider);
        public static string OrderReader = nameof(OrderReader);
        public static string OrderScriptInfo = nameof(OrderScriptInfo);
        public static string OrderId = nameof(OrderId);
        public static string MultiSegmentOrderId = nameof(MultiSegmentOrderId);
        public static string SegmentOrderIds = nameof(SegmentOrderIds);

        private IDictionary<string, object>? _debugData;
        private readonly JsonNode? _jsonData;
        private readonly ILogger _logger;

        public ScriptDebugWrapper(ILogger logger)
        {
            _debugData = new Dictionary<string, object>();
            _logger = logger;
        }

        public ScriptDebugWrapper(JsonNode jsonData, ILogger logger)
        {
            _jsonData = jsonData;
            _logger = logger;
        }

        public ILogger Logger => _logger;

        public void AddScriptDataCall(string methodName, object value, params object[] args)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            try
            {
                var context = ScriptDataProvider;

                if (!_debugData.TryGetValue(context, out var data))
                {
                    data = new ExpandoObject();
                    _debugData.TryAdd(context, data);
                }

                var keyParts = new List<string> { methodName };
                if (args is not null)
                {
                    keyParts.AddRange(args.Select(x => x?.ToString()).Cast<string>().ToArray());
                }
                var key = string.Join("|", keyParts);

                ((ExpandoObject)data).TryAdd(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred when trying to AddScriptDataCall of {methodName}");
            }
        }

        public void AddProperty(string propertyName, object value)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            try
            {
                _debugData.TryAdd(propertyName, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred when trying to AddProperty of {propertyName}");
            }
        }

        public void AddOrderReader(string orderId, IOrderReader value)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            try
            {
                _debugData.TryAdd($"{OrderReader}_{orderId}", ToExpandoObject(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred when trying to AddOrderReader of {orderId}");
            }
        }

        public void AddOrderScriptInfo(OrderScriptInfo value)
        {
            if (_debugData is null)
            {
                throw new Exception("Cannot write data in this mode.");
            }

            try
            {
                _debugData.TryAdd($"{OrderScriptInfo}", ToExpandoObject(value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred when trying to AddOrderScriptInfo");
            }
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
            if (args is not null)
            {
                keyParts.AddRange(args.Select(x => x?.ToString()).Cast<string>().ToArray());
            }
            var key = string.Join("|", keyParts);

            var value = data[key];
            if (value is null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(value.ToJsonString());
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

            return JsonConvert.DeserializeObject<T>(data.ToJsonString());
        }

        public OrderScriptInfo? GetOrderScriptInfo()
        {
            if (_jsonData is null)
            {
                throw new Exception("Cannot read data in this mode.");
            }

            var data = _jsonData[OrderScriptInfo];
            if (data is null)
            {
                return default;
            }

            return JsonConvert.DeserializeObject<OrderScriptInfo>(data.ToJsonString());
        }

        private ExpandoObject ToExpandoObject(object obj)
        {
            var expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj.GetType()))
            {
                try
                {
                    expando.TryAdd(property.Name, property.GetValue(obj));
                }
                catch (Exception)
                {
                    // property probably throws an exception for this configuration, ignore it
                }
            }

            return expando;
        }

        public dynamic GetData() => _debugData;
    }
}
