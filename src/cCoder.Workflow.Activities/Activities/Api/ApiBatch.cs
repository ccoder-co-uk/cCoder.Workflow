// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;


namespace cCoder.Workflow.Activities.Activities.Api;

public class ApiPostBatch : ApiActivity<BatchedResponse[]>
{
    public BatchedRequest[] Data { get; set; }

    public override async Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();
        Log(level:WorkflowLogLevel.Info, message:$"HTTP POST {BaseUrl}{Query}$batch");
        Log(level:WorkflowLogLevel.Info, message:$"Sending a batch of {Data.Length} requests to the API.");

        string body = new { Requests = Data }.ToJsonForOdata();

        HttpResponseMessage response = await api.PostAsync(
            requestUri: Query + "$batch",
            content: new StringContent(
                content: body,
                encoding: Encoding.UTF8,
                mediaType: "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            Log(level:WorkflowLogLevel.Error, message:$"HTTP POST {BaseUrl}{Query}$batch failed with status code {(int)response.StatusCode}\n");
            string content = await response.Content.ReadAsStringAsync();
            Log(level:WorkflowLogLevel.Error, message:content);
            return;
        }

        try
        {
            ResponseBatch responseBatch = await response.Content.ReadAsAsync<ResponseBatch>();
            Result = responseBatch.Responses;

            Log(level:WorkflowLogLevel.Info, message:$"Received {responseBatch.Responses.Length} batched responses");

            int successCount = responseBatch
                .Responses
                .Count(
                    predicate: response => response.Status.StartsWith(
                        value: "2"));

            Log(
                level: WorkflowLogLevel.Info,
                message: $"Received {successCount} successes");

            BatchedResponse[] failures = responseBatch
                .Responses
                .Where(
                    predicate: response => !response.Status.StartsWith(
                        value: "2"))
                .ToArray();

            if (failures.Any())
            {
                Log(level:WorkflowLogLevel.Warning, message:$"Received {failures.Length} failures");

                foreach (BatchedResponse failure in failures)
                {
                    Log(
                        level: WorkflowLogLevel.Error,
                        message: failure.Body.ToJsonForOdata());
                }
            }
        }
        catch (Exception ex)
        {
            Log(level:WorkflowLogLevel.Error, message:$"Exception {ex.Message}");
            Log(level:WorkflowLogLevel.Error, message:await response.Content.ReadAsStringAsync());
        }
    }
}

public class BatchedRequest
{
    public string Id { get; set; }
    public string Method { get; set; }
    public string Url { get; set; }
}

public class ResponseBatch
{
    public BatchedResponse[] Responses { get; set; }
}

public class BatchedResponse
{
    public string Id { get; set; }
    public string Status { get; set; }
    public object Headers { get; set; }
    public object Body { get; set; }
}