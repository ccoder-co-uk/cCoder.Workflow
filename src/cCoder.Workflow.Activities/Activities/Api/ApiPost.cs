// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiPost<T, TResult> : ApiActivity<TResult>
{
    [IgnoreWhenFlowComplete]
    public T Data { get; set; }

    public bool AutoWrapForOdata { get; set; } = true;

    public bool WaitForResults { get; set; } = true;

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(level:WorkflowLogLevel.Info, message:$"HTTP POST {BaseUrl}{Query}");

        object payload = AutoWrapForOdata && typeof(T).GetInterface(name:"IEnumerable") != null
            ? new { value = Data }
            : Data;

        if (WaitForResults)
        {
            // wait for the results to come back
            string body = (Data is string d)
                ? d
                : payload.ToJsonForOdata();

            HttpResponseMessage response = await api.PostAsync(requestUri:Query, content:new StringContent(body, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                Log(level:WorkflowLogLevel.Error, message:$"HTTP POST {BaseUrl}{Query} failed with status code {(int)response.StatusCode}\n");
                string content = await response.Content.ReadAsStringAsync();
                Log(level:WorkflowLogLevel.Error, message:content);
                return;
            }

            if (typeof(TResult) == typeof(string))
                Result = (TResult)(object)await response.Content.ReadAsStringAsync();
            else
            {
                try
                {
                    Result = await response.Content.ReadAsAsync<TResult>();
                }
                catch (Exception ex)
                {
                    Log(level:WorkflowLogLevel.Error, message:$"Exception {ex.Message}");
                    Log(level:WorkflowLogLevel.Error, message:await response.Content.ReadAsStringAsync());
                }
            }
        }
        else // fire and forget
        {
            Task.Run(function:async () =>
            {
                using HttpClient api = GetHttpClient();
                api.Timeout = TimeSpan.FromMinutes(10);
                _ = await api.PostAsync(Query, new StringContent(payload.ToJsonForOdata(), Encoding.UTF8, "application/json"));
            }).Forget();
        }
    }
}