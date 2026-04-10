using cCoder.Workflow.Activities.Support;
using cCoder.Workflow.Activities.Activities.Api;

namespace cCoder.Workflow.Activities.Activities;

public abstract class CoreActivity : ApiActivity
{
    [Picker("Core/App")]
    public int AppId { get; set; }

    public override Task ExecuteInternal(IWorkflowContext context)
    {
        AppId = (int)context.Variables["AppId"];
        return base.ExecuteInternal(context);
    }
}



