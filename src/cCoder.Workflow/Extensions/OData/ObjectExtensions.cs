// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace cCoder.Workflow.Extensions.OData;

internal static class ObjectExtensions
{
    internal static string ToJsonForOdata(this object value) =>
        JsonConvert.SerializeObject(
            value: value,
            formatting: Formatting.None,
            settings: GetODataJsonSettings());

    private static JsonSerializerSettings GetODataJsonSettings() =>
        new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.None,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ContractResolver =
                new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                },
            MaxDepth = 4,
        };
}