// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Extensions;

internal static class TypeExtensions
{
    public static string GetCSharpTypeName(this Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(selector: GetCSharpTypeName);
        return $"{type.Name.Split(separator: '`')[0]}<{string.Join(separator: ",", values: genericNames)}>".Replace(oldValue: "System.Object", newValue: "dynamic", comparisonType: StringComparison.Ordinal);
    }
}