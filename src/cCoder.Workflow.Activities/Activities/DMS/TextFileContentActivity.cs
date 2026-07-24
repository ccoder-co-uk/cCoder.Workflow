// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class TextFileContentActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public string Result { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        Result = await GetFileContents(api);
    }
}