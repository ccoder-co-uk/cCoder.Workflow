// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using Newtonsoft.Json;

namespace Workflow.Brokers.WorkflowJson;

internal sealed class WorkflowJsonBroker
    : IWorkflowJsonBroker,
      IUtilityBroker
{
    public T Deserialize<T>(string value) =>
        JsonConvert.DeserializeObject<T>(value: value);
}