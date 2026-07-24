// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiGetCollection<T> : ApiActivity<IEnumerable<T>>
{
    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(level: WorkflowLogLevel.Info, message: $"HTTP GET {api.BaseAddress}{Query}");
        Result = await api.GetODataCollection<T>(query: Query);
    }
}