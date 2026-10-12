// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Brokers;

public interface IJsonBroker
{
    cCoder.Workflow.Models.WorkflowAttemptContext ParseAttemptContext(string json);

    string SerializeAttemptContext(cCoder.Workflow.Models.WorkflowAttemptContext workflowAttemptContext);

    System.Text.Json.JsonElement SerializeElement(object value);

    object ParseJson(string json);

    T ParseJson<T>(string json);

    string Serialize(object value);

    string SerializeForOData(object value);
}