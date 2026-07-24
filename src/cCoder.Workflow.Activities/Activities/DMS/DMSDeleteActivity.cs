// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class DMSDeleteActivity : DMSActivity
{
    public IEnumerable<string> Paths { get; set; }

    public override async Task ExecuteAsync()
    {
        try
        {
            if (Paths == null && !string.IsNullOrEmpty(Path))
            {
                Paths = new string[] { Path };
            }

            using System.Net.Http.HttpClient api = GetHttpClient();
            using IEnumerator<string> n = Paths.GetEnumerator();
            while (n.MoveNext())
            {
                _ = await api.DeleteAsync($"DMS/{n.Current}");
            }

            Log(WorkflowLogLevel.Info, $"DMS deletions complete");
        }
        catch (Exception ex)
        {
            Log(WorkflowLogLevel.Error, $"Failed to create DMS file because of exception:\n{ex.Message}");
        }
    }
}