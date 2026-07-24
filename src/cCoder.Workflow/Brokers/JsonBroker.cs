// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;

namespace cCoder.Workflow.Brokers;

internal sealed class JsonBroker(
    IJsonDependency jsonDependency)
    : IJsonBroker
{
    public object ParseJson(string json) =>
        jsonDependency.ParseJson(json: json);

    public T ParseJson<T>(string json) =>
        jsonDependency.ParseJson<T>(json: json);

    public string Serialize(object value) =>
        jsonDependency.Serialize(value: value);
}