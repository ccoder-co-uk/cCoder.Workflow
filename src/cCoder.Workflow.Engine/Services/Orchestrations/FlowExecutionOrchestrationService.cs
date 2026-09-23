// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Services.Foundations;

namespace cCoder.Workflow.Engine.Services.Orchestrations;

internal sealed partial class FlowExecutionOrchestrationService(
    IScriptService scriptService,
    IFlowInstanceDataService flowInstanceDataService,
    IWorkflowRuntimeService workflowRuntimeService)
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

            PopulateFlowExecution(
                flowExecution: flowExecution,
                instanceData: instanceData);

            instanceData.State = "Executing";
            instanceData.Start = flowExecution.Start;
            instanceData.End = null;
            flowExecution.Result = instanceData;

            await flowInstanceDataService.SaveFlowInstanceDataAsync(
                flowInstanceData: instanceData,
                apiRoot: request.Api,
                authToken: request.AuthToken);

            return await workflowRuntimeService.ExecuteFlowExecutionAsync(
                flowExecution: flowExecution);
        });

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