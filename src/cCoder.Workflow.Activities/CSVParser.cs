// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        options.FieldNames = GetFieldNames(csvReader: reader, options: options);

        string line = reader.ReadLine();
        while (line != null)
        {
            result.Add(item: ParseLine(csvLine: line, options: options));
            line = reader.ReadLine();
        }

        return result;
    }

    private static string[] GetFieldNames(StringReader csvReader, CSVParseConfig options) =>
        options.FieldNamesInHeader
            ? csvReader.ReadLine().Split(separator: options.Separator)
            : options.FieldNames ?? Enumerable.Range(start: 0, count: 50).Select(selector: i => $"Value{i}").ToArray();

    private static T ParseLine(string csvLine, CSVParseConfig options) =>
        typeof(object) == typeof(T)
            ? ParseDynamicData(csvLine: csvLine, options: options)
            : ParseTypedData(csvLine: csvLine, options: options);

    private static T ParseTypedData(string csvLine, CSVParseConfig options)
    {
        T result = new();
        PropertyInfo[] properties = typeof(T).GetProperties();
        string[] dataItems = csvLine.Split(separator: options.Separator);

        if (!options.FieldNamesInHeader)
        {
            int offset = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (property.CanWrite && (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                {
                    property.SetValue(obj: result, value: dataItems[i - offset]);
                }
            }
        }
        else
        {
            for (int i = 0; i < dataItems.Length; i++)
            {
                SetDataItem(options: options, result: result, properties: properties, dataItems: dataItems, index: i);
            }
        }

        return result;
    }

    private static void SetDataItem(CSVParseConfig options, T result, PropertyInfo[] properties, string[] dataItems, int index)
    {
        string field = options.FieldNames[index];
        string value = dataItems[index];
        PropertyInfo property = properties.FirstOrDefault(predicate: p => p.Name.Equals(value: field, comparisonType: StringComparison.OrdinalIgnoreCase));

        if (property == null || string.IsNullOrEmpty(value: value))
        {
            return;
        }

        try
        {
            if (property.PropertyType == typeof(double))
            {
                property.SetValue(obj: result, value: double.Parse(s: value));
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(obj: result, value: decimal.Parse(s: value));
            }
            else if (property.PropertyType == typeof(bool?) || property.PropertyType == typeof(bool))
            {
                property.SetValue(obj: result, value: bool.Parse(value: value));
            }
            else
            {
                property.SetValue(obj: result, value: value);
            }
        }
        catch
        {
            property.SetValue(obj: result, value: null);
        }
    }

    private static T ParseDynamicData(string csvLine, CSVParseConfig options)
    {
        dynamic result = new ExpandoObject();
        string[] dataItems = csvLine.Split(separator: options.Separator);
        IDictionary<string, object> items = result;

        if (!options.FieldNamesInHeader)
        {
            string[] fieldNames = options.FieldNames;
            for (int i = 0; i < dataItems.Length; i++)
            {
                string field = fieldNames != null && fieldNames.Length > i
                    ? fieldNames[i].Replace(oldValue: " ", newValue: "_")
                    : $"Item{i + 1}";
                items.Add(key: field, value: dataItems[i]);
            }
        }
        else
        {
            for (int i = 0; i < options.FieldNames.Length; i++)
            {
                items.Add(key: options.FieldNames[i], value: i < dataItems.Length ? dataItems[i] : null);
            }
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