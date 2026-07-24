using System.Dynamic;
using System.Reflection;

namespace cCoder.Workflow.Activities.Support;

public static class CSVParser<T>
    where T : new()
{
    public static IEnumerable<T> Parse(string csvData, CSVParseConfig options)
    {
        List<T> result = [];
        options ??= CSVParseConfig.DefaultOptions;

        using StringReader reader = new(csvData);
        options.FieldNames = GetFieldNames(reader, options);

        string line = reader.ReadLine();
        while (line != null)
        {
            result.Add(ParseLine(line, options));
            line = reader.ReadLine();
        }

        return result;
    }

    private static string[] GetFieldNames(StringReader csvReader, CSVParseConfig options) =>
        options.FieldNamesInHeader
            ? csvReader.ReadLine().Split(options.Separator)
            : options.FieldNames ?? Enumerable.Range(0, 50).Select(i => $"Value{i}").ToArray();

    private static T ParseLine(string csvLine, CSVParseConfig options) =>
        typeof(object) == typeof(T)
            ? ParseDynamicData(csvLine, options)
            : ParseTypedData(csvLine, options);

    private static T ParseTypedData(string csvLine, CSVParseConfig options)
    {
        T result = new();
        PropertyInfo[] properties = typeof(T).GetProperties();
        string[] dataItems = csvLine.Split(options.Separator);

        if (!options.FieldNamesInHeader)
        {
            int offset = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (property.CanWrite && (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                    property.SetValue(result, dataItems[i - offset]);
            }
        }
        else
        {
            for (int i = 0; i < dataItems.Length; i++)
                SetDataItem(options, result, properties, dataItems, i);
        }

        return result;
    }

    private static void SetDataItem(CSVParseConfig options, T result, PropertyInfo[] properties, string[] dataItems, int index)
    {
        string field = options.FieldNames[index];
        string value = dataItems[index];
        PropertyInfo property = properties.FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase));

        if (property == null || string.IsNullOrEmpty(value))
            return;

        try
        {
            if (property.PropertyType == typeof(double))
                property.SetValue(result, double.Parse(value));
            else if (property.PropertyType == typeof(decimal))
                property.SetValue(result, decimal.Parse(value));
            else if (property.PropertyType == typeof(bool?) || property.PropertyType == typeof(bool))
                property.SetValue(result, bool.Parse(value));
            else
                property.SetValue(result, value);
        }
        catch
        {
            property.SetValue(result, null);
        }
    }

    private static T ParseDynamicData(string csvLine, CSVParseConfig options)
    {
        dynamic result = new ExpandoObject();
        string[] dataItems = csvLine.Split(options.Separator);
        IDictionary<string, object> items = result;

        if (!options.FieldNamesInHeader)
        {
            string[] fieldNames = options.FieldNames;
            for (int i = 0; i < dataItems.Length; i++)
            {
                string field = fieldNames != null && fieldNames.Length > i
                    ? fieldNames[i].Replace(" ", "_")
                    : $"Item{i + 1}";
                items.Add(field, dataItems[i]);
            }
        }
        else
        {
            for (int i = 0; i < options.FieldNames.Length; i++)
                items.Add(options.FieldNames[i], i < dataItems.Length ? dataItems[i] : null);
        }

        return result;
    }
}

public class CSVParseConfig
{
    public bool FieldNamesInHeader { get; set; }
    public char Separator { get; set; }
    public string[] FieldNames { get; set; }

    public static readonly CSVParseConfig DefaultOptions = new()
    {
        FieldNamesInHeader = false,
        Separator = ',',
    };
}
