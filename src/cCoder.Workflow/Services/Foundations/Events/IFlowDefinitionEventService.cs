// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data.Models.Workflow;


namespace cCoder.Workflow.Services.Foundations.Events;

public interface IFlowDefinitionEventService
{
    ValueTask RaiseFlowDefinitionAddEventAsync(FlowDefinition entity);

    ValueTask RaiseFlowDefinitionUpdateEventAsync(FlowDefinition entity);

    ValueTask RaiseFlowDefinitionDeleteEventAsync(FlowDefinition entity);
}