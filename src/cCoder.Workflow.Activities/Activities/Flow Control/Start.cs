using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Brokers;
using cCoder.Data.Models.CMS;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Activities;


namespace cCoder.Workflow.Activities;

public sealed class Start : CoreActivity
{
    public dynamic Data { get; set; }
    private IWorkflowContext Context { get; set; }
    public override async Task ExecuteAsync()
    {
        if (Data != null)
            Log(
                WorkflowLogLevel.Debug,
                JsonBroker.SerializeForOData(value: Data));

        if (Context.Variables.ContainsKey("AppId"))
        {
            int appId = (int)Context.Variables["AppId"];
            App app = Context is IWorkflowPortalContext portal
                ? await portal.GetPortalAppAsync(appId)
                : await GetAppAsync(appId);
            Context.Variables.Add(new KeyValuePair<string, object>("App", app));
            Log(WorkflowLogLevel.Info, "Grabbed app information");
        }

        await base.ExecuteAsync();
    }

    public override async Task ExecuteInternal(IWorkflowContext context)
    {
        Context = context;
        await base.ExecuteInternal(context);
    }

    private async Task<App> GetAppAsync(int appId)
    {
        using HttpClient api = GetHttpClient();
        return await api.GetAsync<App>($"ContentManagement/App({appId})");
    }
}