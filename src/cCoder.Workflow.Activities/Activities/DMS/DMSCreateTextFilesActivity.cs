// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class DMSCreateTextFilesActivity : DMSActivity
{

    public IEnumerable<string> Names { get; set; }

    [IgnoreWhenFlowComplete]
    public IEnumerable<string> Contents { get; set; }

    public override async Task ExecuteAsync()
    {
        try
        {
            if (Names != null && Contents != null)
            {
                using HttpClient api = GetHttpClient();
                using IEnumerator<string> n = Names.GetEnumerator();
                using IEnumerator<string> c = Contents.GetEnumerator();
                while (n.MoveNext() && c.MoveNext())
                {
                    if (n.Current != null && c.Current != null)
                    {
                        _ = await api.PutAsync(requestUri:$"DMS/{Path.TrimEnd('/')}/{n.Current}", content:new StringContent(c.Current));
                    }
                }

                Log(level:WorkflowLogLevel.Info, message:$"File upload complete, {Names.Count()} files posted to DMS folder {Path}");
            }
            else
            {
                Log(level:WorkflowLogLevel.Warning, message:$"No files requested for creation.");
            }
        }
        catch (Exception ex)
        {
            Log(level:WorkflowLogLevel.Error, message:$"Failed to create DMS file because of exception:\n{ex.Message}");
        }
    }
}