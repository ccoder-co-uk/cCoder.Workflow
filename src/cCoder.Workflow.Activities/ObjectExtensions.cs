// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Activities.Support;

public static class ObjectExtensions
{
    public static JsonSerializerSettings GetJSONSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
        };

    public static JsonSerializerSettings GetODataJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableAttribute = true },
            MaxDepth = 4,
        };

    public static string ToJson(this object value) => JsonConvert.SerializeObject(value, Formatting.None, GetJSONSettings());

    public static string ToJsonForOdata(this object value) => JsonConvert.SerializeObject(value, Formatting.None, GetODataJsonSettings());
}

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            return;

        foreach (T item in source)
            action(item);
    }
}