// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using cCoder.Data.Models.Workflow;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class FlowInstanceDataService
{
    private void ValidateAttemptUpdate(FlowInstanceData expected, FlowInstanceData replacement)
    {
        if (expected is null)
        {
            throw new InvalidOperationException(message: "Workflow instance no longer exists.");
        }

        cCoder.Workflow.Models.WorkflowAttemptContext expectedIdentity = ReadAttemptContext(context: expected.ContextString);
        cCoder.Workflow.Models.WorkflowAttemptContext suppliedIdentity = ReadAttemptContext(context: replacement.ContextString);
        Guid? expectedAttempt = expectedIdentity?.ExecutionAttemptId;
        Guid? suppliedAttempt = suppliedIdentity?.ExecutionAttemptId;

        if (expectedAttempt != suppliedAttempt)
        {
            throw new cCoder.Workflow.Activities.Models.WorkflowAttemptConflictException();
        }

        if (expectedAttempt is null)
        {
            return;
        }

        bool retry = expected.State == "Failed" && replacement.State == "Queued";
        bool beginning = expected.State == "AwaitingExecution" && replacement.State == "Executing";
        bool finishing = expected.State == "Executing" && replacement.State is "Complete" or "CompletedWithWarnings" or "Failed";
        bool ownerMatches = expectedIdentity.ExecutionOwnerId == suppliedIdentity.ExecutionOwnerId;

        bool beginOwner = beginning && suppliedIdentity.ExecutionOwnerId.HasValue
            && suppliedIdentity.ExecutionOwnerId != Guid.Empty && expectedIdentity.ExecutionOwnerId is null;

        if ((!retry && !(beginOwner || (finishing && ownerMatches)))
            || (retry && !ownerMatches)
            || expected.Caller != replacement.Caller
            || expected.FlowDefinitionId != replacement.FlowDefinitionId)
        {
            throw new cCoder.Workflow.Activities.Models.WorkflowAttemptConflictException();
        }
    }

    private cCoder.Workflow.Models.WorkflowAttemptContext ReadAttemptContext(string context)
    {
        try
        {
            return jsonBroker.ParseAttemptContext(json: context);
        }
        catch (Exception)
        {
            return null;
        }
    }
}