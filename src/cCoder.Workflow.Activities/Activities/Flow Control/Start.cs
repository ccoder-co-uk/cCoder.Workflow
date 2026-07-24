// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            Log(level:WorkflowLogLevel.Debug, message:JsonConvert.SerializeObject(Data, ObjectExtensions.GetODataJsonSettings()));

        if (Context.Variables.ContainsKey(key:"AppId"))
        {
            using HttpClient api = GetHttpClient();
            App app = await api.GetAsync<App>(query:$"ContentManagement/App({Context.Variables["AppId"]})");
            Context.Variables.Add(item:new KeyValuePair<string, object>("App", app));
            Log(level:WorkflowLogLevel.Info, message:"Grabbed app information");
        }

        await base.ExecuteAsync();
    }

    public override async Task ExecuteInternal(IWorkflowContext context)
    {
        Context = context;
        await base.ExecuteInternal(context:context);
    }
}