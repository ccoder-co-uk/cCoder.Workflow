// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

internal interface IFlowInstanceDataEventService
{
    ValueTask RaiseFlowInstanceDataAddEventAsync(FlowInstanceData flowInstanceData);

    ValueTask RaiseFlowInstanceDataUpdateEventAsync(FlowInstanceData flowInstanceData);

    ValueTask RaiseFlowInstanceDataDeleteEventAsync(FlowInstanceData flowInstanceData);
}