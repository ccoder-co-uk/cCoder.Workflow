// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Data.Models.DMS;
using File = cCoder.Data.Models.DMS.File;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Activities;


namespace cCoder.Workflow.Activities.Activities.DMS;

public abstract class DMSActivity : CoreActivity
{
    public string Path { get; set; }

    public override Task ExecuteInternal(IWorkflowContext context)
    {
        AppId = (int)context.Variables["AppId"];
        return base.ExecuteInternal(context:context);
    }

    protected async Task<File[]> GetFiles(HttpClient api) =>
        ParamsAllSet()
            ? (await api.GetODataCollection<Folder>(query:$"DocumentManagement/Folder?$filter=AppId eq {AppId} AND Path eq '{Path.Trim().TrimEnd("/".ToCharArray())}'&$expand=Files"))
                .FirstOrDefault()?
                .Files?
                .ToArray() ?? []
        : [];

    protected async Task<File> GetFile(HttpClient api)
        => ParamsAllSet()
            ? (await api.GetODataCollection<File>(query:$"DocumentManagement/File?$filter=Folder/AppId eq {AppId} AND Path eq '{Path.ToLower()}'")).FirstOrDefault()
            : null;

    protected async Task<string[]> GetFileContents(HttpClient api, IEnumerable<string> paths)
    {
        if (paths != null)
        {
            List<string> results = [];

            foreach (string f in paths)
            {
                results.Add(item:await api.GetStringAsync($"DMS/{f.ToLower()}"));
            }

            return [.. results];
        }
        else
        {
            Log(level:WorkflowLogLevel.Warning, message:"No File paths given to download.");
            return [];
        }
    }

    protected async Task<string> GetFileContents(HttpClient api)
    {
        if (ParamsAllSet())
        {
            try
            {
                Log(level:WorkflowLogLevel.Info, message:$"Fetching file @ ~DMS/{Path.ToLower()}");
                return await api.GetStringAsync(requestUri:$"DMS/{Path.ToLower()}");
            }
            catch { return string.Empty; }
        }
        else
        {
            return string.Empty;
        }
    }

    private bool ParamsAllSet()
    {
        bool result = true;

        if (AppId == 0)
        {
            Log(level:WorkflowLogLevel.Warning, message:$"  Unable to fetch file @ ~DMS/{Path ?? string.Empty} as the AppId has not been specified.");
            result = false;
        }

        if (Path == null || Path.Trim().TrimEnd(trimChars:"/".ToCharArray()).Length == 0)
        {
            Log(level:WorkflowLogLevel.Warning, message:$"  Unable to fetch file @ ~DMS/{Path ?? string.Empty} as the Path appears to be incorrect.");
            result = false;
        }

        return result;
    }
}