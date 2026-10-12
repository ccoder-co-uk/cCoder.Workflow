// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal sealed partial class FlowExecutionOrchestrationService(
    IScriptService scriptService,
    IFlowInstanceDataService flowInstanceDataService,
    IWorkflowRuntimeService workflowRuntimeService,
    cCoder.Workflow.Engine.Brokers.IJsonBroker jsonBroker)
    : IFlowExecutionOrchestrationService
{
    public ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            WorkflowRequest request = flowExecution.Request;
            flowExecution.Start = DateTimeOffset.UtcNow;
            flowExecution.Script = scriptService;

            FlowInstanceData instanceData =
                await GetFlowInstanceDataAsync(
                    flowExecution: flowExecution);

            WorkflowAttemptIdentity identity = ReadAttemptIdentity(flowExecution: flowExecution, instanceData: instanceData);

            if ((request.ExecutionAttemptId.HasValue || identity?.ExecutionAttemptId.HasValue == true)
                && (request.ExecutionAttemptId != identity?.ExecutionAttemptId
                    || instanceData.State != "AwaitingExecution"))
            {
                throw new WorkflowAttemptConflictException();
            }

            PopulateFlowExecution(
                flowExecution: flowExecution,
                instanceData: instanceData);

            if (request.ExecutionAttemptId.HasValue)
            {
                flowExecution.ExecutionOwnerId = Guid.NewGuid();

                instanceData.ContextString = jsonBroker.AssignExecutionOwner(
                    context: instanceData.ContextString, ownerId: flowExecution.ExecutionOwnerId.Value);
            }

            instanceData.State = "Executing";
            instanceData.Start = flowExecution.Start;
            instanceData.End = null;

            try
            {
                await flowInstanceDataService.SaveFlowInstanceDataAsync(
                    flowInstanceData: instanceData, apiRoot: request.Api, authToken: request.AuthToken);
            }
            catch (Exception claimFailure) when (flowExecution.ExecutionOwnerId.HasValue)
            {
                FlowInstanceData persisted;
                WorkflowAttemptIdentity persistedIdentity;

                try
                {
                    persisted = await GetFlowInstanceDataAsync(flowExecution: flowExecution);
                    persistedIdentity = ReadAttemptIdentity(flowExecution: flowExecution, instanceData: persisted);
                }
                catch (Exception reconciliationFailure)
                {
                    throw new AggregateException(innerExceptions: [claimFailure, reconciliationFailure]);
                }

                if (persistedIdentity?.ExecutionAttemptId == request.ExecutionAttemptId && persisted.State == "AwaitingExecution")
                {
                    throw;
                }

                if (persisted.State != "Executing"
                    || persistedIdentity?.ExecutionAttemptId != request.ExecutionAttemptId
                    || persistedIdentity.ExecutionOwnerId != flowExecution.ExecutionOwnerId)
                {
                    throw new WorkflowAttemptConflictException();
                }

                instanceData = persisted;
            }

            flowExecution.Result = instanceData;

            return await workflowRuntimeService.ExecuteFlowExecutionAsync(
                flowExecution: flowExecution);
        });

    private WorkflowAttemptIdentity ReadAttemptIdentity(FlowExecution flowExecution, FlowInstanceData instanceData)
    {
        try
        {
            return jsonBroker.DeserializeAttemptIdentity(value: instanceData.ContextString);
        }
        catch (Exception) when (!flowExecution.Request.ExecutionAttemptId.HasValue)
        {
            return null;
        }
    }

    private async Task<FlowInstanceData> GetFlowInstanceDataAsync(
        FlowExecution flowExecution)
    {
        try
        {
            return await flowInstanceDataService.GetFlowInstanceDataAsync(
                apiRoot: flowExecution.Request.Api,
                authToken: flowExecution.Request.AuthToken,
                flowInstanceDataId: flowExecution.Request.InstanceId);
        }
        catch
        {
            await flowExecution.Log(
                level: WorkflowLogLevel.Error,
                message: "Failed to deserialize flow instance response.");

            throw;
        }
    }

    private static void PopulateFlowExecution(
        FlowExecution flowExecution,
        FlowInstanceData instanceData)
    {
        flowExecution.AppId = instanceData.FlowDefinition.AppId;
        flowExecution.Id = instanceData.Id;
        flowExecution.Name = instanceData.Name;
        flowExecution.Caller = instanceData.Caller;

        flowExecution.FlowDefinitionId =
            instanceData.FlowDefinitionId;
    }
}