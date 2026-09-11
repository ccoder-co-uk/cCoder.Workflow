// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Activities.Brokers;

internal static class JsonBroker
{
    internal static object Deserialize(string value) =>
        JsonConvert.DeserializeObject(
            value: value,
            settings: GetJsonSettings());

    internal static T Deserialize<T>(string value) =>
        JsonConvert.DeserializeObject<T>(
            value: value,
            settings: GetJsonSettings());

    internal static string Serialize(object value) =>
        JsonConvert.SerializeObject(
            value: value,
            formatting: Formatting.None,
            settings: GetJsonSettings());

    internal static string SerializeForOData(object value) =>
        JsonConvert.SerializeObject(
            value: value,
            formatting: Formatting.None,
            settings: GetODataJsonSettings());

    private static JsonSerializerSettings GetJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Objects,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = true
            }
        };

    private static JsonSerializerSettings GetODataJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = true
            },
            MaxDepth = 4
        };
}