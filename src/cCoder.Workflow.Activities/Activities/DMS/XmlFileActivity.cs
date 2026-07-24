// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class XmlFileActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public dynamic Result { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        Result = cCoder.Workflow.Activities.Support.Data.ParseXml<dynamic>(
            data: await GetFileContents(
                api: api));
    }
}