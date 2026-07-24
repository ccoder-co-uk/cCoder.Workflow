// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Support;

internal static class TypeNameExtensions
{
    public static string GetCSharpTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        IEnumerable<string> genericNames = type.GenericTypeArguments.Select(GetCSharpTypeName);
        return $"{type.Name.Split('`')[0]}<{string.Join(",", genericNames)}>".Replace("System.Object", "dynamic", StringComparison.Ordinal);
    }
}