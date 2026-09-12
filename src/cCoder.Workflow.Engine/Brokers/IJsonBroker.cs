// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Brokers;

public interface IJsonBroker
{
    T Deserialize<T>(string value);
    T DeserializeWithoutTypeInformation<T>(string value);
    string Serialize(object value);
    string SerializeForOData(object value);
}