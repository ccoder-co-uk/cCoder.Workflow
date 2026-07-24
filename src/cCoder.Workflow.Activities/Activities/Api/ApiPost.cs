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

    public bool AutoWrapForOdata { get; set; }

    public bool WaitForResults { get; set; }

    public ApiPost()
    {
        AutoWrapForOdata = true;
        WaitForResults = true;
    }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(level:WorkflowLogLevel.Info, message:$"HTTP POST {BaseUrl}{Query}");

        object payload = AutoWrapForOdata && typeof(T).GetInterface(name:"IEnumerable") != null
            ? new { value = Data }
            : Data;

        if (WaitForResults)
        {
            string body = (Data is string d)
                ? d
                : payload.ToJsonForOdata();

            HttpResponseMessage response = await api.PostAsync(
                requestUri: Query,
                content: new StringContent(
                    content: body,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                Log(level:WorkflowLogLevel.Error, message:$"HTTP POST {BaseUrl}{Query} failed with status code {(int)response.StatusCode}\n");
                string content = await response.Content.ReadAsStringAsync();
                Log(level:WorkflowLogLevel.Error, message:content);
                return;
            }

            if (typeof(TResult) == typeof(string))
            {
                Result = (TResult)(object)await response.Content.ReadAsStringAsync();
            }
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
        else
        {
            Task
                .Run(
                    function: async () =>
                    {
                        using HttpClient api = GetHttpClient();
                        api.Timeout = TimeSpan.FromMinutes(
                            value: 10);

                        string payloadJson = payload.ToJsonForOdata();

                        _ = await api.PostAsync(
                            requestUri: Query,
                            content: new StringContent(
                                content: payloadJson,
                                encoding: Encoding.UTF8,
                                mediaType: "application/json"));
                    })
                .Forget();
        }
    }
}