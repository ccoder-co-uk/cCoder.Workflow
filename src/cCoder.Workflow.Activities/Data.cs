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
            return array.SelectMany(item => Flatten(item, path)).ToArray();

        List<ExpandoObject> results = [];
        IDictionary<string, JToken> obj = source as JObject ?? JObject.FromObject(source);
        KeyValuePair<string, JToken>[] values = obj.Where(kv => Primitives.Contains(kv.Value.Type)).ToArray();

        results.AddRange(
            obj.Where(kv => !Primitives.Contains(kv.Value.Type))
               .SelectMany(kv => Flatten(kv.Value, $"{path}_{kv.Key}".Trim('_'))));

        if (!results.Any())
        {
            IDictionary<string, object> current = new ExpandoObject();
            foreach (KeyValuePair<string, JToken> value in values)
                current[$"{path}_{value.Key}".Trim('_')] = value.Value;

            results.Add((ExpandoObject)current);
        }
        else
        {
            foreach (ExpandoObject result in results)
            {
                IDictionary<string, object> current = result;
                foreach (KeyValuePair<string, JToken> value in values)
                    current[$"{path}_{value.Key}".Trim('_')] = value.Value;
            }
        }

        return [.. results];
    }

    public static T ParseXml<T>(string data)
    {
        StringBuilder builder = new();
        JsonSerializer.Create().Serialize(new CleanJsonWriter(new StringWriter(builder)), ParseXml(data));
        return JsonConvert.DeserializeObject<T>(builder.ToString());
    }

    public static XDocument ParseXml(string data) => XDocument.Parse(data);

    public static T ParseJson<T>(string data) => JsonConvert.DeserializeObject<T>(data, ObjectExtensions.GetJSONSettings());

    public static object ParseJson(string data) => JsonConvert.DeserializeObject(data, ObjectExtensions.GetJSONSettings());

    public static IEnumerable<T> ParseCSV<T>(string data, CSVParseConfig config)
        where T : new() => CSVParser<T>.Parse(data, config);
}

internal sealed class CleanJsonWriter(TextWriter writer) : JsonTextWriter(writer)
{
    public override void WritePropertyName(string name)
    {
        string result = name;

        if (name.StartsWith('@') || name.StartsWith('#'))
            result = name[1..];

        if (result.Contains(':'))
            result = result.Split(':').Last();

        base.WritePropertyName(result);
    }
}
