using System;
using ServiceStack.Text;

namespace LiveOakApp.Models.Services
{
	public class JsonService
    {
        private readonly DateTimeService _dateTimeService;

        public JsonService(DateTimeService dateTimeService)
        {
            _dateTimeService = dateTimeService;

            Configure();
        }

        private void Configure()
        {
            JsConfig<DateTime>.SerializeFn = time => _dateTimeService.DateTimeToServerString(time);
            JsConfig<DateTime?>.SerializeFn = time => _dateTimeService.DateTimeToServerString(time);

            JsConfig<DateTime>.DeSerializeFn = s => _dateTimeService.ServerStringToDateTime(s) ?? DateTime.MinValue;
            JsConfig<DateTime?>.DeSerializeFn = s => _dateTimeService.ServerStringToDateTime(s);

            JsConfig.ExcludeTypeInfo = true;
        }

        public string Serialize(object obj)
        {
            if (obj == null)
                return null;
            return JsonSerializer.SerializeToString(obj);
        }

        public TResult Deserialize<TResult>(string json)
        {
            if (json == null)
                return default(TResult);
            return JsonSerializer.DeserializeFromString<TResult>(json);
        }
    }
}

