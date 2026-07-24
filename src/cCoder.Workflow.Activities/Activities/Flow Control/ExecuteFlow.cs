// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Support;
using cCoder.Data.Models.Workflow;
using cCoder.Workflow.Activities.Models;
using cCoder.Workflow.Activities.Activities;


namespace cCoder.Workflow.Activities;

public class ExecuteFlow : CoreActivity
{
    public string ProcessName { get; set; }
    public string Name { get; set; }
    public object Data { get; set; }

    public override async Task ExecuteAsync()
    {
        try
        {
            using HttpClient api = GetHttpClient();
            IEnumerable<FlowDefinition> defs = await api.GetODataCollection<FlowDefinition>($"Workflow/FlowDefinition?$filter=AppId eq {AppId} and Process/Name eq '{ProcessName}' and Name eq '{Name}'");
            if (defs?.Any() ?? false)
                _ = await api.PostAsync($"Workflow/FlowDefinition({defs.First().Id})/Execute", new StringContent(Data.ToJson())).ConfigureAwait(false);
            else
                Log(WorkflowLogLevel.Warning, "Flow not found!");
        }
        catch { Log(WorkflowLogLevel.Error, "Access Denied!"); }
    }
}