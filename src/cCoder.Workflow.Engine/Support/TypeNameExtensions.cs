// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Support;

internal static class TypeNameExtensions
{
    public static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(selector: GetCSharpTypeName);
        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>".Replace(oldValue: "System.Object", newValue: "dynamic", comparisonType: StringComparison.Ordinal);
    }
}