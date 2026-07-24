// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Dependencies;

public interface IJsonDependency
{
    object ParseJson(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);
}