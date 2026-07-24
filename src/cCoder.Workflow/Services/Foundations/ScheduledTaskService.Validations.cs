// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Dependencies;
using cCoder.Data.Models.Planning;
using System.Security;

namespace cCoder.Workflow.Services.Foundations;

internal sealed partial class ScheduledTaskService
{
    private static void ValidateInputs(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScheduledTaskOnAdd(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateScheduledTaskOnUpdate(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateForExecutionOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllForAppScheduledTaskOnDelete(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateAllByAppIdOnDelete(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateExecuteAsUserBelongsToAppOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private static void ValidateFlowBelongsToAppOnGet(params object[] inputs) =>
        ValidationRulesEngine.Validate(inputs: inputs);

    private void ValidateScheduledTaskAccess(ScheduledTask scheduledTask)
    {
        bool isAppAdmin =
            authorizationBroker.IsAdminOfApp(appId: scheduledTask.AppId);

        bool userBelongsToApp =
            scheduledTaskBroker.SelectExecuteAsUserBelongsToApp(
                executeAs: scheduledTask.ExecuteAs,
                appId: scheduledTask.AppId);

        bool flowBelongsToApp =
            scheduledTaskBroker.SelectFlowBelongsToApp(
                flowId: scheduledTask.FlowId,
                appId: scheduledTask.AppId);

        if (!isAppAdmin || !userBelongsToApp || !flowBelongsToApp)
        {
            throw new SecurityException("Access Denied!");
        }
    }

    private void ValidateScheduledTaskExecutionAccess(
        ScheduledTask scheduledTask)
    {
        if (!authorizationBroker.IsAdminOfApp(appId: scheduledTask.AppId))
        {
            throw new SecurityException("Access Denied!");
        }
    }
}