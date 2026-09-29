// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace cCoder.Workflow.Extensions.OData;

internal static class IEnumerableExtensions
{
    internal static void ForEach<T>(
        this IEnumerable<T> source,
        Action<T> action)
    {
        if (source == null)
        {
            return;
        }

        foreach (T item in source)
        {
            action(obj: item);
        }
    }
}