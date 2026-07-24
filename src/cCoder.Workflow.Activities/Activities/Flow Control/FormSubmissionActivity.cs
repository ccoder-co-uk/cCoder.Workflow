// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Workflow.Activities.Activities;

namespace cCoder.Workflow.Activities;

public class FormSubmissionActivity<T> : Activity
{
    public string AuthToken { get; set; }
    public T Data { get; set; }

    public override async Task ExecuteInternal(IWorkflowContext context)
    {
        if (Data != null)
        {
            await base.ExecuteInternal(context: context);
        }
    }
}