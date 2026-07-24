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
            if (Paths == null && !string.IsNullOrEmpty(value:Path))
            {
                Paths = new string[] { Path };
            }

            using System.Net.Http.HttpClient api = GetHttpClient();
            using IEnumerator<string> n = Paths.GetEnumerator();
            while (n.MoveNext())
            {
                _ = await api.DeleteAsync(requestUri:$"DMS/{n.Current}");
            }

            Log(level:WorkflowLogLevel.Info, message:$"DMS deletions complete");
        }
        catch (Exception ex)
        {
            Log(level:WorkflowLogLevel.Error, message:$"Failed to create DMS file because of exception:\n{ex.Message}");
        }
    }
}