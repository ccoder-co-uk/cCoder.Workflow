// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal interface IFlowInstanceDataService
{
    Task<FlowInstanceData> GetFlowInstanceDataAsync(
        string apiRoot,
        string authToken,
        Guid flowInstanceDataId);

    ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken);
}