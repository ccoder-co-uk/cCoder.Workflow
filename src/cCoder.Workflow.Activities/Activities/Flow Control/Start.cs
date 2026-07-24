using cCoder.Workflow.Activities.Support;
using cCoder.Data.Models.CMS;
using Newtonsoft.Json;
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
            Log(WorkflowLogLevel.Debug, JsonConvert.SerializeObject(Data, ObjectExtensions.GetODataJsonSettings()));

        if (Context.Variables.ContainsKey("AppId"))
        {
            using HttpClient api = GetHttpClient();
            App app = await api.GetAsync<App>($"ContentManagement/App({Context.Variables["AppId"]})");
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
}









