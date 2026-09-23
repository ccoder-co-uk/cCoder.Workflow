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

internal sealed partial class WorkflowRuntimeService(
    IWorkflowContextBroker workflowContextBroker,
    IJsonBroker jsonBroker,
    IReflectionBroker reflectionBroker)
    : IWorkflowRuntimeService
{
    public ValueTask<FlowExecution> ExecuteFlowExecutionAsync(
        FlowExecution flowExecution) =>
        TryCatch(operation: async () =>
        {
            ValidateInputs(inputs: [flowExecution]);

            WorkflowContext dataContext =
                await DeserializeWorkflowContextAsync(
                    flowExecution: flowExecution,
                    rawContext: flowExecution.Result.ContextString);

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
                    apiRoot: flowExecution.Request.Api,
                    authToken: flowExecution.Request.AuthToken);

            flowExecution.Result = CompleteFlowExecution(
                flowExecution: flowExecution);

            return flowExecution;
        });

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

    private async Task StitchFlowExecutionAsync(
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

    private string BuildActivityAssignment(
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

                string sourceType = GetCSharpTypeName(
                    type: source.GetType());

                string destinationType = GetCSharpTypeName(
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

    private string GetCSharpTypeName(Type type)
    {
        if (!reflectionBroker.IsGenericType(type: type))
        {
            return reflectionBroker.GetTypeName(type: type);
        }

        IEnumerable<string> genericNames = reflectionBroker
            .GetGenericTypeArguments(type: type)
            .Select(selector: GetCSharpTypeName);

        string typeName = reflectionBroker.GetTypeName(type: type);

        return ($"{typeName.Split(separator: '`')[0]}"
            + $"<{string.Join(separator: ",", values: genericNames)}>")
            .Replace(
                oldValue: "System.Object",
                newValue: "dynamic",
                comparisonType: StringComparison.Ordinal);
    }

}