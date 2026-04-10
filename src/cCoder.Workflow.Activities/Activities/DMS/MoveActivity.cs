using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class MoveActivity : DMSActivity
{
    public string OldPath { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        System.Net.Http.HttpResponseMessage result = await api.PutAsync($"DMS/{OldPath}?moveTo={Path}", null);

        try { _ = result.EnsureSuccessStatusCode(); }
        catch (Exception ex)
        {
            Log(WorkflowLogLevel.Error, $"{ex.Message}\n{result.Content.ReadAsStringAsync()}");
            Log(WorkflowLogLevel.Error, "Paths in question are ...");
            Log(WorkflowLogLevel.Error, $"From: {OldPath}");
            Log(WorkflowLogLevel.Error, $"To  : {Path}");
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
            Log(WorkflowLogLevel.Info, $"Moving file: {path} to {Path}...");
            Task<HttpResponseMessage> request = api.PutAsync($"DMS/{path}?moveTo={Path}", null);
            request.Wait();
            HttpResponseMessage result = request.Result;

            try
            {
                _ = result.EnsureSuccessStatusCode();
                Log(WorkflowLogLevel.Info, $"Moved file: {path} to {Path}");
            }
            catch (Exception ex)
            {
                Log(WorkflowLogLevel.Warning, $"{ex.Message}\n{result.Content.ReadAsStringAsync()}");
                Log(WorkflowLogLevel.Error, "Paths in question are ...");
                Log(WorkflowLogLevel.Error, $"From: {path}");
                Log(WorkflowLogLevel.Error, $"To  : {Path}");
            }
        }

        return Task.CompletedTask;
    }
}



