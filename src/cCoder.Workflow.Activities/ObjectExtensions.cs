using cCoder.Workflow.Activities.Brokers;

namespace cCoder.Workflow.Activities.Support;

public static class ObjectExtensions
{
    public static string ToJson(this object value) =>
        JsonBroker.Serialize(value: value);

    public static string ToJsonForOdata(this object value) =>
        JsonBroker.SerializeForOData(value: value);
}

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            return;

        foreach (T item in source)
            action(item);
    }
}