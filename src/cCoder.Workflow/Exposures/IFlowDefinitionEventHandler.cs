// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Exposures;

public interface IFlowDefinitionEventHandler
{
    ValueTask HandleFlowDefinitionDeleteAsync(FlowDefinition flowDefinition);
}