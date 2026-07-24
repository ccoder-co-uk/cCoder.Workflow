// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiDelete<T> : ApiActivity<object>
{
    [IgnoreWhenFlowComplete]
    public T Data { get; set; }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        string dataId = ((dynamic)Data).Id.ToString();

        string requestQuery = Query.Replace(
            oldValue: "[Key]",
            newValue: dataId);

        Log(
            level: WorkflowLogLevel.Info,
            message: $"HTTP DELETE {api.BaseAddress}{requestQuery}");

        await api.DeleteAsync(
            requestUri: requestQuery);
    }
}