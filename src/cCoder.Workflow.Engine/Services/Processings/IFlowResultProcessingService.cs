// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Engine.Services.Processings;

internal interface IFlowResultProcessingService
{
    T Deserialize<T>(string value);

    string Serialize(object value);

    ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken);
}