// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Extensions;
using Newtonsoft.Json;

namespace cCoder.Workflow.Brokers;

internal class JsonBroker : IJsonBroker
{
    public object ParseJson(string json) =>
        JsonConvert.DeserializeObject(
            value: json,
            settings: new JsonSerializerSettings().ConfigureForWorkflow());

    public T ParseJson<T>(string json) =>
        JsonConvert.DeserializeObject<T>(
            value: json,
            settings: new JsonSerializerSettings().ConfigureForWorkflow());

    public string Serialize(object value) =>
        JsonConvert.SerializeObject(
            value: value,
            settings: new JsonSerializerSettings().ConfigureForWorkflow());
}