// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class MoveActivity : DMSActivity
{
    public string OldPath { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        System.Net.Http.HttpResponseMessage result = await api.PutAsync(requestUri: $"DMS/{OldPath}?moveTo={Path}", content: null);

        try { _ = result.EnsureSuccessStatusCode(); }
        catch (Exception ex)
        {
            Log(level: WorkflowLogLevel.Error, message: $"{ex.Message}\n{result.Content.ReadAsStringAsync()}");
            Log(level: WorkflowLogLevel.Error, message: "Paths in question are ...");
            Log(level: WorkflowLogLevel.Error, message: $"From: {OldPath}");
            Log(level: WorkflowLogLevel.Error, message: $"To  : {Path}");
        }
    }
}

public class MoveAllActivity : DMSActivity
{

    public string[] OldPaths { get; set; }

    public override Task ExecuteAsync()
    {
        using HttpClient api = GetHttpClient();

        foreach (string path in OldPaths)
        {
            Log(level: WorkflowLogLevel.Info, message: $"Moving file: {path} to {Path}...");
            Task<HttpResponseMessage> request = api.PutAsync(requestUri: $"DMS/{path}?moveTo={Path}", content: null);
            request.Wait();
            HttpResponseMessage result = request.Result;

            try
            {
                _ = result.EnsureSuccessStatusCode();
                Log(level: WorkflowLogLevel.Info, message: $"Moved file: {path} to {Path}");
            }
            catch (Exception ex)
            {
                Log(level: WorkflowLogLevel.Warning, message: $"{ex.Message}\n{result.Content.ReadAsStringAsync()}");
                Log(level: WorkflowLogLevel.Error, message: "Paths in question are ...");
                Log(level: WorkflowLogLevel.Error, message: $"From: {path}");
                Log(level: WorkflowLogLevel.Error, message: $"To  : {Path}");
            }
        }

        return Task.CompletedTask;
    }
}