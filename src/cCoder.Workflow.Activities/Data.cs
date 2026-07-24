// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Dynamic;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cCoder.Workflow.Activities.Support;

public static class Data
{
    private static readonly JTokenType[] Primitives =
    [
        JTokenType.String,
        JTokenType.Guid,
        JTokenType.Boolean,
        JTokenType.Integer,
        JTokenType.Date,
        JTokenType.Float,
        JTokenType.TimeSpan,
        JTokenType.Uri,
    ];

    public static ExpandoObject[] Flatten(object source, string path = "")
    {
        if (source is JArray array)
        {
            return array.SelectMany(selector:item => Flatten(item, path)).ToArray();
        }

        List<ExpandoObject> results = [];
        IDictionary<string, JToken> obj = source as JObject ?? JObject.FromObject(o:source);
        KeyValuePair<string, JToken>[] values = obj.Where(predicate:kv => Primitives.Contains(kv.Value.Type)).ToArray();

        results.AddRange(
collection:            obj.Where(kv => !Primitives.Contains(kv.Value.Type))
               .SelectMany(kv => Flatten(kv.Value, $"{path}_{kv.Key}".Trim('_'))));

        if (!results.Any())
        {
            IDictionary<string, object> current = new ExpandoObject();
            foreach (KeyValuePair<string, JToken> value in values)
            {
                current[$"{path}_{value.Key}".Trim(trimChar:'_')] = value.Value;
            }

            results.Add(item:(ExpandoObject)current);
        }
        else
        {
            foreach (ExpandoObject result in results)
            {
                IDictionary<string, object> current = result;
                foreach (KeyValuePair<string, JToken> value in values)
                {
                    current[$"{path}_{value.Key}".Trim(trimChar:'_')] = value.Value;
                }
            }
        }

        return [.. results];
    }

    public static T ParseXml<T>(string data)
    {
        StringBuilder builder = new();
        JsonSerializer.Create().Serialize(jsonWriter:new CleanJsonWriter(new StringWriter(builder)), value:ParseXml(data));
        return JsonConvert.DeserializeObject<T>(value:builder.ToString());
    }

    public static XDocument ParseXml(string data) => XDocument.Parse(text:data);

    public static T ParseJson<T>(string data) => JsonConvert.DeserializeObject<T>(value:data, settings:ObjectExtensions.GetJSONSettings());

    public static object ParseJson(string data) => JsonConvert.DeserializeObject(value:data, settings:ObjectExtensions.GetJSONSettings());

    public static IEnumerable<T> ParseCSV<T>(string data, CSVParseConfig config)
        where T : new() => CSVParser<T>.Parse(csvData:data, options:config);
}

internal sealed class CleanJsonWriter(TextWriter writer) : JsonTextWriter(writer)
{
    public override void WritePropertyName(string name)
    {
        string result = name;

        if (name.StartsWith(value:'@') || name.StartsWith(value:'#'))
        {
            result = name[1..];
        }

        if (result.Contains(value:':'))
        {
            result = result.Split(separator:':').Last();
        }

        base.WritePropertyName(name:result);
    }
}