// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class CsvFileActivity : DMSActivity
{
    public CSVParseConfig CSVParseConfig { get; set; }

    [IgnoreWhenFlowComplete]
    public dynamic Result { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        Result = cCoder.Workflow.Activities.Support.Data.ParseCSV<dynamic>(data:await GetFileContents(api), config:CSVParseConfig);
    }
}