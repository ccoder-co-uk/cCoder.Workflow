// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class JsonFileActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public object Result { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        string results = await GetFileContents(api:api);
        Result = cCoder.Workflow.Activities.Support.Data.ParseJson(data:results);
    }
}