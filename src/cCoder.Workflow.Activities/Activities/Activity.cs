using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Models;
using Newtonsoft.Json;


namespace cCoder.Workflow.Activities.Activities;

/// <summary>
/// Base type for all workflow activities
/// </summary>
public abstract class Activity
{
    public static IDictionary<Activity, Action<Activity, IDictionary<string, object>, Flow>> CompiledLinkCache { get; set; }

    public static IReadOnlyList<string> ScriptImports { get; set; } =
    [
        "cCoder.Workflow.Activities",
        "cCoder.Workflow.Activities.Activities.Api",
        "cCoder.Workflow.Activities.Activities.DMS",
        "cCoder.Workflow.Activities.Activities.Sftp",
        "cCoder.Workflow.Activities.Activities.Templating",
        "cCoder.Workflow.Activities.Activities.Transformation",
        "cCoder.Data",
        "cCoder.Data.Extensions",
        "cCoder.Data.Models",
        "cCoder.Data.Models.CMS",
        "cCoder.Data.Models.DMS",
        "cCoder.Data.Models.Logging",
        "cCoder.Data.Models.Mail",
        "cCoder.Data.Models.Packaging",
        "cCoder.Data.Models.Planning",
        "cCoder.Data.Models.Security",
        "cCoder.Data.Models.Workflow",
        "cCoder.Workflow.Activities.Models",
        "Newtonsoft.Json",
        "Newtonsoft.Json.Linq",
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Xml.Linq"
    ];

    [Required]
    public string Ref { get; set; }

    public ActivityState State { get; protected set; } = ActivityState.NotRun;

    [NotMapped]
    [JsonIgnore]
    public Activity[] Previous { get; set; }

    [NotMapped]
    [JsonIgnore]
    public Activity[] Next { get; set; }

    [JsonIgnore]
    public string AssignCode { get; set; }

    [NotMapped]
    [JsonIgnore]
    public IScriptRunner ScriptRunner { get; set; }

    public virtual Task ExecuteAsync() => Task.FromResult(true);

    public void Skip()
    {
        State = ActivityState.Skipped;
        Next.Where(n => n.Previous.Length == 1).ForEach(n => n.Skip());
    }

    public void Log(WorkflowLogLevel level, string message)
    {
        Context?.Log(level, $"{Ref}:: {message}");
        if (level is WorkflowLogLevel.Error or WorkflowLogLevel.Fatal)
        {
            State = ActivityState.Failed;
        }
    }

    private IWorkflowContext Context;

    public virtual async Task ExecuteInternal(IWorkflowContext context)
    {
        Context = context;

        if (Previous == null || Previous.All(a => a.State == ActivityState.Complete))
        {
            Log(WorkflowLogLevel.Info, "Activity Execution started");
            await ExecuteLinksAsync(context);

            if (State == ActivityState.NotRun)
                await SafeExecuteAndUpdateState(context);
        }
    }

    private async Task SafeExecuteAndUpdateState(IWorkflowContext context)
    {
        try
        {
            State = ActivityState.Running;
            await ExecuteAsync();

            if (State == ActivityState.Running)
            {
                State = ActivityState.Complete;
                Log(WorkflowLogLevel.Info, "Activity Execution Completed");
                await ContinueFlow(context);
                return;
            }

            if (State == ActivityState.Skipped)
            {
                Log(WorkflowLogLevel.Info, "Activity Execution Skipped");
                await ContinueFlow(context);
                return;
            }

            Log(WorkflowLogLevel.Error, "Activity Execution Failed");
        }
        catch (Exception ex)
        {
            Log(WorkflowLogLevel.Error, "Activity Execution Failed:\n" + ex.Message);
            Log(WorkflowLogLevel.Debug, ex.StackTrace);
            State = ActivityState.Failed;
        }
    }

    private async Task ExecuteLinksAsync(IWorkflowContext context)
    {
        if (!string.IsNullOrEmpty(AssignCode))
        {
            try
            {
                if (CompiledLinkCache is not null && CompiledLinkCache.ContainsKey(this))
                    CompiledLinkCache[this](this, context.Variables, context.Flow);
                else
                    (await BuildScript<Action<Activity, IDictionary<string, object>, Flow>>(AssignCode))?.Invoke(this, context.Variables, context.Flow);
            }
            catch (Exception ex)
            {
                State = ActivityState.Failed;
                Log(WorkflowLogLevel.Fatal, $"Link Execution Failed\n{ex.Message}\n{ex.StackTrace}");
                Log(WorkflowLogLevel.Fatal, $"Link Code in question: \n{AssignCode}");
                return;
            }
        }
    }

    private async Task ContinueFlow(IWorkflowContext context)
    {
        if (Next != null)
            foreach (Activity t in Next)
                await t.ExecuteInternal(context);
    }

    protected Task<TFunc> BuildScript<TFunc>(string code) =>
        (ScriptRunner ?? Context.Script).BuildScript<TFunc>(code, (string[])Context?.Variables["Imports"] ?? ScriptImports.ToArray(), Log);

    protected Task<T> ExecuteScript<T>(string code, object args) =>
        (ScriptRunner ?? Context.Script).Run<T>(code, (string[])Context?.Variables["Imports"] ?? ScriptImports.ToArray(), args, Log);

    protected Task ExecuteScript(string code, object args) =>
        (ScriptRunner ?? Context.Script).Run(code, (string[])Context?.Variables["Imports"] ?? ScriptImports.ToArray(), args, Log);
}