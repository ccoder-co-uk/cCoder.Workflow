// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal interface IFlowInstanceDataService
{
    Task<string> GetCallerJsonAsync(string apiRoot, string authToken);

    Task<string> GetPortalAppJsonAsync(string apiRoot, string authToken, int appId);

    Task<FlowInstanceData> GetFlowInstanceDataAsync(
        string apiRoot,
        string authToken,
        Guid flowInstanceDataId);

    ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken);
}