// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiGet<T> : ApiActivity<T>
{
    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(level:WorkflowLogLevel.Info, message:$"HTTP GET {api.BaseAddress}{Query}");

        if (typeof(T) == typeof(string))
        {
            string responseString = await api.GetStringAsync(requestUri:Query);

            GetType()
                .GetProperty(name:"Result")
                .SetValue(obj:this, value:responseString);

        }
        else
        {
            Result = await api.GetAsync<T>(query:Query);
        }

        Log(level:WorkflowLogLevel.Debug, message:Result.ToJson());
    }
}