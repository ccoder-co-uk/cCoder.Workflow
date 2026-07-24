using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiGet<T> : ApiActivity<T>
{
    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(WorkflowLogLevel.Info, $"HTTP GET {api.BaseAddress}{Query}");

        if (typeof(T) == typeof(string))
        {
            string responseString = await api.GetStringAsync(Query);

            GetType()
                .GetProperty("Result")
                .SetValue(this, responseString);

        }
        else
            Result = await api.GetAsync<T>(Query);

        Log(WorkflowLogLevel.Debug, Result.ToJson());
    }
}





