// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Workflow.Engine.Brokers;

public interface IJsonBroker
{
    cCoder.Workflow.Activities.Models.WorkflowAttemptIdentity DeserializeAttemptIdentity(string value);

    string AssignExecutionOwner(string context, System.Guid ownerId);

    T Deserialize<T>(string value);
    T DeserializeWithoutTypeInformation<T>(string value);
    string Serialize(object value);
    string SerializeForOData(object value);
}