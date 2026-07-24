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

        Log(level:WorkflowLogLevel.Info, message:$"HTTP DELETE {api.BaseAddress}{Query.Replace("[Key]", ((dynamic)Data).Id.ToString())}");

        await api.DeleteAsync(requestUri:Query);
    }
}