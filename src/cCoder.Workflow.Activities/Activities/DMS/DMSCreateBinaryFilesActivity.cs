// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class DMSCreateBinaryFilesActivity : DMSActivity
{
    public IEnumerable<string> Names { get; set; }

    [IgnoreWhenFlowComplete]
    public IEnumerable<byte[]> Contents { get; set; }

    public override async Task ExecuteAsync()
    {
        try
        {
            if (Names != null && Contents != null)
            {
                using HttpClient api = GetHttpClient();
                using IEnumerator<string> n = Names.GetEnumerator();
                using IEnumerator<byte[]> c = Contents.GetEnumerator();

                while (n.MoveNext() && c.MoveNext())
                {
                    if (n.Current != null && c.Current != null)
                    {
                        string path = Path.TrimEnd(
                            trimChar: '/');

                        _ = await api.PostAsync(
                            requestUri: $"DMS/{path}/{n.Current}",
                            content: new ByteArrayContent(
                                content: c.Current));
                    }
                }

                Log(level:WorkflowLogLevel.Info, message:$"File upload complete, files posted to DMS folder {Path}!");
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