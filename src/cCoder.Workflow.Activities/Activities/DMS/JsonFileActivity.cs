using cCoder.Workflow.Activities.Models;

namespace cCoder.Workflow.Activities.Activities.DMS;

public class JsonFileActivity : DMSActivity
{
    [IgnoreWhenFlowComplete]
    public object Result { get; set; }

    public override async Task ExecuteAsync()
    {
        using System.Net.Http.HttpClient api = GetHttpClient();
        string results = await GetFileContents(api);
        Result = cCoder.Workflow.Activities.Support.Data.ParseJson(results);
    }
}