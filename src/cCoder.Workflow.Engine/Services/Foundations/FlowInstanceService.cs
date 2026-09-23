// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Activities;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Engine.Brokers;
using cCoder.Workflow.Engine.Models;
using cCoder.Workflow.Engine.Models.Exceptions;

namespace cCoder.Workflow.Engine.Services.Foundations;

internal sealed partial class FlowInstanceService(
    IScriptService scriptService,
    IWorkflowContextBroker workflowContextBroker,
    IWorkflowHttpClientBroker workflowHttpClientBroker,
    IJsonBroker jsonBroker,
    IReflectionBroker reflectionBroker)
    : IFlowInstanceService
{
    public ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            WorkflowRequest request = flowExecution.Request;
            flowExecution.Start = DateTimeOffset.UtcNow;
            flowExecution.Script = scriptService;

            string rawInstance = await workflowHttpClientBroker.GetStringAsync(
                apiRoot: request.Api,
                authToken: request.AuthToken,
                requestUri:
                    $"Workflow/FlowInstanceData({request.InstanceId})"
                    + "?$expand=FlowDefinition($expand=App)");

            FlowInstanceData instanceData =
                await DeserializeFlowInstanceDataAsync(
                    flowExecution: flowExecution,
                    rawInstance: rawInstance);

            PopulateFlowExecution(
                flowExecution: flowExecution,
                instanceData: instanceData);

            instanceData.State = "Executing";
            instanceData.Start = flowExecution.Start;
            instanceData.End = null;
            flowExecution.Result = instanceData;

            await SaveFlowInstanceDataAsync(
                flowInstanceData: instanceData,
                apiRoot: request.Api,
                authToken: request.AuthToken);

            WorkflowContext dataContext =
                await DeserializeWorkflowContextAsync(
                    flowExecution: flowExecution,
                    rawContext: instanceData.ContextString);

            flowExecution.Flow = dataContext.Flow
                ?? throw new InvalidOperationException(
                    "Flow instance context did not contain a workflow.");

            await StitchFlowExecutionAsync(
                flowExecution: flowExecution);

            flowExecution.Context =
                workflowContextBroker.CreateWorkflowExecutionContext(
                    flowExecution: flowExecution);

            await workflowContextBroker
                .ExecuteWorkflowExecutionContextAsync(
                    workflowExecutionContext: flowExecution.Context,
                    apiRoot: request.Api,
                    authToken: request.AuthToken);

            flowExecution.Result = CompleteFlowExecution(
                flowExecution: flowExecution);

            return flowExecution;
        });

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

    private async ValueTask SaveFlowInstanceDataAsync(
        FlowInstanceData flowInstanceData,
        string apiRoot,
        string authToken)
    {
        string payload = jsonBroker.SerializeForOData(
            value: new
            {
                flowInstanceData.Id,
                flowInstanceData.FlowDefinitionId,
                flowInstanceData.Name,
                flowInstanceData.State,
                flowInstanceData.ReportingComponentName,
                flowInstanceData.Caller,
                flowInstanceData.ContextString,
                flowInstanceData.Start,
                flowInstanceData.End
            });

        WorkflowHttpResult response =
            await workflowHttpClientBroker.PutJsonAsync(
                apiRoot: apiRoot,
                authToken: authToken,
                requestUri:
                    $"Workflow/FlowInstanceData({flowInstanceData.Id})",
                payload: payload);

        if (!response.IsSuccess)
        {
            throw new WorkflowEngineServiceException(
                $"Workflow state save failed with status "
                + $"{response.StatusCode} ({response.Status})."
                + Environment.NewLine
                + response.Body);
        }
    }

    private async Task<FlowInstanceData>
        DeserializeFlowInstanceDataAsync(
            FlowExecution flowExecution,
            string rawInstance)
    {
        try
        {
            return jsonBroker.Deserialize<FlowInstanceData>(
                value: rawInstance)
                ?? throw new InvalidOperationException(
                    "Workflow instance response was empty.");
        }
        catch
        {
            await LogFlowExecutionAsync(
                flowExecution: flowExecution,
                level: WorkflowLogLevel.Error,
                message:
                    $"Failed to deserialize flow instance:"
                    + $"{Environment.NewLine}{rawInstance}");

            throw;
        }
    }

    private async Task<WorkflowContext>
        DeserializeWorkflowContextAsync(
            FlowExecution flowExecution,
            string rawContext)
    {
        try
        {
            return jsonBroker.Deserialize<WorkflowContext>(
                value: rawContext)
                ?? throw new InvalidOperationException(
                    "Workflow context response was empty.");
        }
        catch
        {
            await LogFlowExecutionAsync(
                flowExecution: flowExecution,
                level: WorkflowLogLevel.Error,
                message:
                    $"Failed to deserialize flow context:"
                    + $"{Environment.NewLine}{rawContext}");

            throw;
        }
    }

    private FlowInstanceData CompleteFlowExecution(
        FlowExecution flowExecution)
    {
        foreach (Activity activity in
            flowExecution.Context.Flow.Activities)
        {
            PropertyInfo[] properties = activity.GetType()
                .GetProperties()
                .Where(predicate: property =>
                    reflectionBroker.HasAttribute<
                        IgnoreWhenFlowCompleteAttribute>(property: property))
                .ToArray();

            foreach (PropertyInfo property in properties)
            {
                reflectionBroker.SetValue(
                    property: property,
                    instance: activity,
                    value: default);
            }
        }

        return new FlowInstanceData
        {
            Id = flowExecution.Id,
            Name = flowExecution.Name,
            Caller = flowExecution.Caller,
            FlowDefinitionId = flowExecution.FlowDefinitionId,
            ContextString = jsonBroker.Serialize(
                value: flowExecution.Context),
            State = flowExecution.Context.ExecutionState,
            Start = flowExecution.Start,
            End = DateTimeOffset.UtcNow
        };
    }

    private static async Task StitchFlowExecutionAsync(
        FlowExecution flowExecution)
    {
        foreach (Activity activity in flowExecution.Flow.Activities)
        {
            try
            {
                string[] links = flowExecution.Flow.Links
                    .Where(predicate: link =>
                        link.Destination == activity.Ref)
                    .Select(selector: link => link.Source)
                    .ToArray();

                activity.Previous = flowExecution.Flow.Activities
                    .Where(predicate: candidate =>
                        links.Contains(value: candidate.Ref))
                    .ToArray();
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in previous activity selection for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }
        }

        foreach (Activity activity in flowExecution.Flow.Activities)
        {
            try
            {
                activity.Next = flowExecution.Flow.Activities
                    .Where(predicate: candidate =>
                        candidate.Previous?.Contains(
                            value: activity) ?? false)
                    .ToArray();
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in next activity selection for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }

            try
            {
                activity.AssignCode = BuildActivityAssignment(
                    activity: activity,
                    flow: flowExecution.Flow);
            }
            catch (Exception exception)
            {
                await LogFlowExecutionAsync(
                    flowExecution: flowExecution,
                    level: WorkflowLogLevel.Error,
                    message:
                        $"Problem in one or more links for activity "
                        + $"{activity.Ref}:{Environment.NewLine}"
                        + $"{exception.Message}{Environment.NewLine}"
                        + exception.StackTrace);
            }
        }
    }

    private static string BuildActivityAssignment(
        Activity activity,
        Flow flow)
    {
        string[] assignments = activity.Previous?
            .Select(selector: source =>
            {
                Link link = flow.Links.First(
                    predicate: found =>
                        found.Source == source.Ref
                        && found.Destination == activity.Ref);

                string sourceType =
                    cCoder.Workflow.Engine.Extensions.TypeExtensions.GetCSharpTypeName(
                        type: source.GetType());

                string destinationType =
                    cCoder.Workflow.Engine.Extensions.TypeExtensions.GetCSharpTypeName(
                        type: activity.GetType());

                return string.IsNullOrWhiteSpace(
                    value: link.Expression)
                    ? null
                    : $"//LINK:: {source.Ref} => {activity.Ref}"
                        + Environment.NewLine
                        + link.Expression
                            .Replace(
                                oldValue: "destination.",
                                newValue:
                                    $"(({destinationType})activity).",
                                comparisonType:
                                    StringComparison.Ordinal)
                            .Replace(
                                oldValue: "source.",
                                newValue:
                                    $"flow.GetActivity<{sourceType}>"
                                    + $"(\"{source.Ref}\").",
                                comparisonType:
                                    StringComparison.Ordinal);
            })
            .Where(predicate: item => item is not null)
            .ToArray()
            ?? [];

        if (assignments.Length == 0)
        {
            return null;
        }

        string body = $"\t{string.Join(
            separator: $";{Environment.NewLine}\t",
            value: assignments)}";

        return $"(activity, variables, flow) => "
            + $"{{{Environment.NewLine}{body}"
            + $"{Environment.NewLine}}}";
    }

    private static Task LogFlowExecutionAsync(
        FlowExecution flowExecution,
        WorkflowLogLevel level,
        string message) =>
        flowExecution.Log(
            level: level,
            message: message);

}